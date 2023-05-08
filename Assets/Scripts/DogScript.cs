using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.InputSystem;
using Cinemachine;

public class DogScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip barkClip;
    public CinemachineVirtualCamera vcam;
    public ContactFilter2D movementFilter;
    public DragScript dragScript;
    public SpawnPlayers spawnPlayers;
    public float health;
    public float maxHealth;
    public float moveSpeed = 0.5f;
    public float CollisionOffset = 0.05f;
    bool holding = false;
    Rigidbody2D rgDog;
    Vector2 movementInput;
    PhotonView view;
    SpriteRenderer spriteRenderer;
    Animator animator;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>(); 
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public SliderController slider;
    // Các biến sử dụng cho việc tích slider
    private bool []triggerActive = new bool [6];        
    public float []holdTime = new float [6];

    // Biến cho bone
    GameObject Bone;
    Bone bone;
    
    //Start dashspeed
    public float dashSpeed = 0.5f;
    public float dashLength = 5f, dashCooldown = 5f;
    public float dashCounter;
    public float dashCoolCounter;
    // bark sound
    public int barkCounter;
    public float barkCooler = 0;
    public float barkCoolCounter = 0;
    // Lưu trữ vị trí ban đầu của đối tượng
    private Vector3 initialPosition;
    public float Health {
        set {
            health = value;
                print("Take this!");
            if (health <= 0) {
                Defeated();
            }
        }
        get {
            return health;
        }
    }
    
    private void Start() {
        // dragScript = spawnPlayers.getDragScript();
        Bone =  GameObject.Find("Bone");
        bone = Bone.GetComponent<Bone>();
        view = GetComponent<PhotonView>();
        rgDog = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialPosition = transform.position;
        for (int i = 0; i < 6; i++) {
            triggerActive[i] = false;
            holdTime[i] = 0f;
        }
        // Tắt collid        
        if (!view.IsMine) vcam.enabled = false;

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = barkClip;
    }

    private void Update() {
        // DRAG
        // if (spawnPlayers.getDragScript() != null) {
        // dragScript = spawnPlayers.getDragScript();
        // // print (spawnPlayers.getDragScript());
        // }
        // Thanh máu
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < maxHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }

        // Chức năng sủa
        if (view.IsMine && Input.GetKey(KeyCode.J)) {
            if (barkCounter < 5 && barkCoolCounter <= 0) {
                audioSource.clip = barkClip;
                audioSource.Play();
                animator.SetBool("canMoveAfterBark", false);
                barkCoolCounter = 1f;
                barkCounter++;
                barkCooler += 2f;
            }
        }
        // Hồi số lần sủa
        if (barkCounter > 0)
        {
            barkCooler -= Time.deltaTime;
            if (barkCooler/2 == barkCounter + 1) {
                barkCounter--;
            }
        }
        // Hồi sủa liên tục
        if (barkCoolCounter > 0) {
            barkCoolCounter -= Time.deltaTime;
            animator.SetBool("canMoveAfterBark", true);
        }
        // Chức năng đào
        for (int i = 0; i < 6; i++) {
            // if (view.IsMine)
            {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    animator.SetTrigger("isDigging");
                    if (view.IsMine) animator.SetBool("canMoveAfterDig", false);
                    if (triggerActive[i]) {
                        holdTime[i] += 0.03f;
                        slider.DiggingItem(holdTime[i]);
                        if (view.IsMine) slider.TriggerSlider(true);
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
                }
                else if (!Input.GetKey(KeyCode.Space)) {
                    if (view.IsMine) animator.SetBool("canMoveAfterDig", true);
                    //animator.SetTrigger("isDigging");
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
        // Dash
            if (view.IsMine && Input.GetKey(KeyCode.K)) 
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

    private void FixedUpdate() {
        if (view.IsMine) {
            if (animator.GetBool("canMoveAfterBeenAttack") && animator.GetBool("canMoveAfterDig") && animator.GetBool("canMoveAfterBark")) {

                // Nếu input vào không phải 0 thì sẽ phải di chuyển tương ứng
                if (movementInput != Vector2.zero)
                {
                    bool success = TryMove(movementInput);
                    if (!success) {
                        success = TryMove(new Vector2(movementInput.x, 0));
                    }
                    if (!success){
                        success = TryMove(new Vector2(0, movementInput.y));
                    }
                    animator.SetBool("isMoving", success);
                }
                else {
                    animator.SetBool("isMoving", false);
                }
            }
            if (movementInput.x < 0) {
                spriteRenderer.flipX = true;
            } else if (movementInput.x > 0) {
                spriteRenderer.flipX = false;
            }
        }
        /*// DRAG disabled
        if (spawnPlayers.getDragScript() != null) {
            if (dragScript.getTriggerDrag() && dragScript.getCurrentDog() == view.ViewID) {
                if (view.IsMine == false)
                {
                    view.TransferOwnership(PhotonNetwork.LocalPlayer);        
                }
                BeingDrag();
                // Chỉnh lại layer để khi đi lên nó không bị đè
                if (dragScript.getX() == 0 && dragScript.getY() == 1) {
                    spriteRenderer.sortingOrder = 0;
                } else {
                    spriteRenderer.sortingOrder = 1;
                }
            }
            if (!dragScript.getTriggerDrag() && dragScript.getCurrentDog() == view.ViewID && holding == true) 
            {
                if (view.IsMine == false)
                {
                    view.TransferOwnership(PhotonNetwork.LocalPlayer);        
                }
                ThrowDrag();
            }
        }
        */
    }

    public void TakeDamage(float dame) {
        if (view.IsMine == false)
        {
            view.TransferOwnership(PhotonNetwork.LocalPlayer);
            // Debug.LogError("PhotonView with ID=" + view.ViewID + " does not exist.");
        }
        Health -= dame;
        animator.SetTrigger("isHurt");
        animator.SetBool("canMoveAfterBeenAttack", false);
    }

    public void BeingDrag() {
        transform.position = Vector3.Lerp(transform.position, dragScript.getDragOffset(), 0.5f);
        rgDog.simulated = false;
        holding = true;
        dragScript.StopDrag();
    }

    public void ThrowDrag() {
        this.transform.position = Vector3.Lerp(transform.position, dragScript.getDragOffset(), 0.5f);
        rgDog.simulated = true;
        holding = false;
    }

    public void Defeated() {
        PhotonNetwork.Destroy(gameObject);
    }

    private bool TryMove(Vector2 direction) {
        if (direction != Vector2.zero) {

            // Kiểm tra khả năng va chạm
            int count = rgDog.Cast(
                direction,
                movementFilter,
                castCollisions,
                moveSpeed * Time.fixedDeltaTime + CollisionOffset
            );
            if (count == 0) {
                rgDog.MovePosition(rgDog.position + direction * moveSpeed * Time.fixedDeltaTime);
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

    public void ResetMovement() {
        animator.SetBool("canMoveAfterBeenAttack", true);
    }
}
