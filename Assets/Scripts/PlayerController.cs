using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Photon.Pun;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public ContactFilter2D movementFilter;
    public float moveSpeed = 1f;
    public float CollisionOffset = 0.05f;
    public CinemachineVirtualCamera vcam;
    AudioSource audioSource;
    SpriteRenderer spriteRenderer;
    Vector2 movementInput;
    Rigidbody2D rigid;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>(); 
    Animator animator;
    PhotonView view;

    // Các biến sử dụng cho việc di chuyển và set Animation
    public SwordAttack swordAttack;
    public DragScript dragScript;

    //Start dashspeed
    public float dashSpeed = 1.2f;
    public float dashLength = 2f, dashCooldown = 5f;
    public float dashCounter;
    public float dashCoolCounter;

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Tắt collider của kiếm và chỉnh animator mặc định để collider ở đúng vị trí ban đầu
        swordAttack.StopAttack();
        dragScript.StopDrag();
        animator.SetFloat("moveX", 0);
        animator.SetFloat("moveY", -1);
        if (!view.IsMine) vcam.enabled = false;
        audioSource = GetComponent<AudioSource>();
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
                audioSource.Play();
                SwordAttack();
            }
            
            /*
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
            }*/
            
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
                    moveSpeed = 1f;
                    dashCoolCounter = dashCooldown;
                }
            }
            if (dashCoolCounter > 0)
            {
                dashCoolCounter -= Time.deltaTime;
            }
            //End dash move
        }
    }
    
    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            if (animator.GetBool("canMoveAfterAttack") && animator.GetBool("canMoveAfterDig")) {
                movementInput.x = Input.GetAxisRaw("Horizontal");
                //movementInput.y = Input.GetAxisRaw("Vertical");

                // if (movementInput.x != 0) movementInput.y = 0;

                // Nếu input vào không phải 0 thì sẽ phải di chuyển tương ứng
                if (movementInput != Vector2.zero)
                {
                    animator.SetFloat("moveX", movementInput.x);
                    //animator.SetFloat("moveY", movementInput.y);
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
            if (movementInput.x < 0) {
                spriteRenderer.flipX = true;
                view.RPC("FlipAnimationRPC", RpcTarget.All, true);
            } else if (movementInput.x > 0) {
                spriteRenderer.flipX = false;
                view.RPC("FlipAnimationRPC", RpcTarget.All, false);
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

    public void SwordAttack() {
        swordAttack.setView(view);
        if (animator.GetFloat("moveX") >=1) {
            swordAttack.AttackRight();
        } 
        else if (animator.GetFloat("moveX") <= -1) {
            swordAttack.AttackLeft();
        } 
        // else if (animator.GetFloat("moveX") == 0 && animator.GetFloat("moveY") == 1) {
        //     swordAttack.AttackTop();
        // } 
        // else if (animator.GetFloat("moveX") == 0 && animator.GetFloat("moveY") == -1) {
        //     swordAttack.AttackBot();
        // }
    }

    public void DragingDog() {
        if (animator.GetFloat("moveX") >=1 && animator.GetFloat("moveY") == 0) {
            dragScript.DragRight();
        } 
        else if (animator.GetFloat("moveX") <= -1 && animator.GetFloat("moveY") == 0) {
            dragScript.DragLeft();
        } 
        // else if (animator.GetFloat("moveX") == 0 && animator.GetFloat("moveY") == 1) {
        //     dragScript.DragTop();
        // } 
        // else if (animator.GetFloat("moveX") == 0 && animator.GetFloat("moveY") == -1) {
        //     dragScript.DragBot();
        // }
    }

    public void LockMovement() {
        // animator.SetBool("canMove", false);
    }

    public void ResetMovement() {
        animator.SetBool("canMoveAfterAttack", true);
        swordAttack.StopAttack();
        dragScript.StopDrag();
    }
    [PunRPC]
    private void FlipAnimationRPC(bool check) {
        spriteRenderer.flipX = check;
    }
}
