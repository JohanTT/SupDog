using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Photon.Pun;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public SliderController slider;
    public ContactFilter2D movementFilter;
    public float moveSpeed = 0.5f;
    public float CollisionOffset = 0.05f;
    public CinemachineVirtualCamera vcam;

    SpriteRenderer spriteRenderer;
    Vector2 movementInput;
    Rigidbody2D rigid;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>(); 
    Animator animator;
    PhotonView view;
    GameObject Bone;
    Bone bone;
    
    // Các biến sử dụng cho việc tích slider
    private bool []triggerActive = new bool [6];        
    public float []holdTime = new float [6];
    // Các biến sử dụng cho việc di chuyển và set Animation
    public SwordAttack swordAttack;
    public DragScript dragScript;

    //Start dashspeed
    public float dashSpeed = 0.8f;
    public float dashLength = 5f, dashCooldown = 5f;
    public float dashCounter;
    public float dashCoolCounter;

    // Start is called before the first frame update
    void Start()
    {
        Bone =  GameObject.Find("Bone");
        bone = Bone.GetComponent<Bone>();
        view = GetComponent<PhotonView>();
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        for (int i = 0; i < 6; i++) {
            triggerActive[i] = false;
            holdTime[i] = 0f;
        }
        // Tắt collider của kiếm và chỉnh animator mặc định để collider ở đúng vị trí ban đầu
        swordAttack.StopAttack();
        dragScript.StopDrag();
        animator.SetFloat("moveX", 0);
        animator.SetFloat("moveY", -1);
        if (!view.IsMine) vcam.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            // Chức năng đánh
            if (Input.GetKeyDown(KeyCode.J)) {
                animator.SetBool("canMoveAfterAttack", false);
                animator.SetTrigger("swordAttack");
                SwordAttack();
            }
            
            // Chức năng kéo
            if (Input.GetKeyDown(KeyCode.L)) {
                // nếu chưa có bắt được con nào thì sẽ bắt
                if (!dragScript.getTriggerDrag()) {
                    dragScript.setPlayerTransform(transform.position);
                    animator.SetBool("canMoveAfterAttack", false);
                    animator.SetTrigger("swordAttack");
                    DragingDog();
                }
                // Đã bắt được và thả ra
                if (dragScript.getTriggerDrag()) {
                    dragScript.setTrigger(false);
                    dragScript.Throw(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
                }
                // DragingDog();
                // dragScript.setPlayerTransform(transform.position);
            }
            if (dragScript.getTriggerDrag()) {
                dragScript.setX(animator.GetFloat("moveX"));
                dragScript.setY(animator.GetFloat("moveY"));
                // (animator.GetFloat("moveX"), animator.GetFloat("moveY"));
                dragScript.setPlayerTransform(transform.position);
            }
            
            // Dash
            if (Input.GetKey(KeyCode.K)) 
            {
                if (dashCoolCounter <= 0 && dashCounter <=0)
                {
                    moveSpeed = dashSpeed;
                    dashCounter = dashLength;
                }
            }
            
            // Sử dụng dash xong
            if (dashCounter > 0)
            {
                dashCounter -= Time.deltaTime;
                
                if (dashCounter <= 0)
                {
                    moveSpeed = 0.5f;
                    dashCoolCounter = dashCooldown;
                }
            }
            if (dashCoolCounter > 0)
            {
                dashCoolCounter -= Time.deltaTime;
            }
            //End dash move
        }
        // Chức năng đào
        for (int i = 0; i < 6; i++) {
            // if (view.IsMine)
            {
                if (Input.GetKey(KeyCode.Space) && triggerActive[i]) {
                    holdTime[i] += 0.03f;
                    slider.DiggingItem(holdTime[i]);
                    if (view.IsMine) slider.TriggerSlider(true);
                    if (view.IsMine) animator.SetBool("canMoveAfterDig", false);
                    if (view.IsMine) {
                        GameObject tmp = GameObject.FindWithTag("Hole" + (i+1));
                        PhotonView photonView = tmp.GetComponent<PhotonView>();
                        if (photonView != null) {
                            print(photonView.ViewID);
                            if (!photonView.IsMine) {
                                photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
                                print("Transfer to " + view.ViewID);
                            }
                        }
                    }
                }
                else if (!Input.GetKey(KeyCode.Space)) {
                    if (view.IsMine) animator.SetBool("canMoveAfterDig", true);
                    // Giảm dần hố đã đào
                    // if (holdTime[i] >= 0.0f)  
                    // {
                    //     holdTime[i] -= 0.0009f;
                    //     _itemDiggingController.DiggingItem(holdTime[i]);
                    // }
                }
            }
            if (triggerActive[i] && holdTime[i] >= 3f)
            {
                if (view.IsMine) slider.TriggerSlider(false);
                holdTime[i] = 0f;
                slider.DiggingItem(holdTime[i]);
                GameObject tmp = GameObject.FindWithTag("Hole" + (i+1));
                PhotonView photonView = tmp.GetComponent<PhotonView>();
                bone.addBone();
                if (photonView.IsMine) PhotonNetwork.Destroy(tmp);
            }
        }
    }
    
    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            if (animator.GetBool("canMoveAfterAttack") && animator.GetBool("canMoveAfterDig")) {
                movementInput.x = Input.GetAxisRaw("Horizontal");
                movementInput.y = Input.GetAxisRaw("Vertical");

                // if (movementInput.x != 0) movementInput.y = 0;

                // Nếu input vào không phải 0 thì sẽ phải di chuyển tương ứng
                if (movementInput != Vector2.zero)
                {
                    animator.SetFloat("moveX", movementInput.x);
                    animator.SetFloat("moveY", movementInput.y);
                    bool success = TryMove(movementInput);
                    if (!success) {
                        success = TryMove(new Vector2(movementInput.x, 0));
                    }
                    if (!success){
                        success = TryMove(new Vector2(0, movementInput.y));
                    }
                    animator.SetBool("isMoving", success);
                    // swordAttack.attackDirection = swordAttack.AttackDirection.bot;
                }
                else {
                    animator.SetBool("isMoving", false);
                }
            }
        }
    }

    private bool TryMove(Vector2 direction) {
        if (direction != Vector2.zero) {

            // Kiểm tra khả năng va chạm
            int count = rigid.Cast(
                direction,
                movementFilter,
                castCollisions,
                moveSpeed * Time.fixedDeltaTime + CollisionOffset
            );
            if (count == 0) {
                rigid.MovePosition(rigid.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            }
            else {
                return false;
            }
        }
        else return false;
    }

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        for (int i = 0; i < 6; i++) {
            if (other.gameObject.CompareTag("Hole" + (i+1).ToString()))
            {
                triggerActive[i] = true;
                if (holdTime[i] > 0) {
                    slider.TriggerSlider(true);
                    slider.DiggingItem(holdTime[i]);
                    // _itemDiggingController.TriggerSlider(true);
                    // _itemDiggingController.DiggingItem(holdTime[i]);
                }
            }
        }
    }
 
    public void OnTriggerExit2D(Collider2D other)
    {
        for (int i = 0; i < 6; i++) {
            if (other.gameObject.CompareTag("Hole" + (i+1).ToString()))
            {
                triggerActive[i] = false;
                slider.TriggerSlider(false);
                // _itemDiggingController.TriggerSlider(false);
            }
        }
    
    }

    public void SwordAttack() {
        if (animator.GetFloat("moveX") >=1 && animator.GetFloat("moveY") == 0) {
            swordAttack.AttackRight();
        } else if (animator.GetFloat("moveX") <= -1 && animator.GetFloat("moveY") == 0) {
            swordAttack.AttackLeft();
        } else if (animator.GetFloat("moveX") == 0 && animator.GetFloat("moveY") == 1) {
            swordAttack.AttackTop();
        } else if (animator.GetFloat("moveX") == 0 && animator.GetFloat("moveY") == -1) {
            swordAttack.AttackBot();
        }
    }

    public void DragingDog() {
        if (animator.GetFloat("moveX") >=1 && animator.GetFloat("moveY") == 0) {
            dragScript.DragRight();
        } else if (animator.GetFloat("moveX") <= -1 && animator.GetFloat("moveY") == 0) {
            dragScript.DragLeft();
        } else if (animator.GetFloat("moveX") == 0 && animator.GetFloat("moveY") == 1) {
            dragScript.DragTop();
        } else if (animator.GetFloat("moveX") == 0 && animator.GetFloat("moveY") == -1) {
            dragScript.DragBot();
        }
    }

    public void LockMovement() {
        // animator.SetBool("canMove", false);
    }

    public void ResetMovement() {
        animator.SetBool("canMoveAfterAttack", true);
        swordAttack.StopAttack();

        dragScript.StopDrag();
    }
}
