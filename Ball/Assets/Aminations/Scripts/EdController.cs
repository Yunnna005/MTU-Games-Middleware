using UnityEngine;
using UnityEngine.InputSystem;

public class EdController : MonoBehaviour
{
    Animator animator;
    CharacterController controller;

    private float speed = 2f;
    private float jumpHeight = 3f;
    private float gravity = -9.81f;

    Vector2 moveInput;
    Vector3 velocity;
    private bool isJumping;
    private bool isRunning;

    private void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        animator.SetBool("isWaving", false);
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log($"Jumpping: {context.performed} - Is Grounded: {controller.isGrounded}");
        bool isGrounded = controller.isGrounded || velocity.y <= 0.1f;

        if (context.performed && isGrounded)
        {
            Debug.Log("Character is supposed to jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;
            animator.SetBool("isJumping", true);
            animator.SetFloat("JumpVelocity", 1f);
        }
    }
    /*public void OnRun(InputAction.CallbackContext context)  //Character runs and waves at the same time
    {
        if (context.performed)
            isRunning = true;
        else if (context.canceled)
            isRunning = false;
    }*/
    public void OnWave(InputAction.CallbackContext context)
    {
        if (context.performed)
            animator.SetBool("isWaving", true);
        else if (context.canceled)
            animator.SetBool("isWaving", false);
    }

    void Update()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        if (controller.isGrounded && isJumping && velocity.y < 0)
        {
            isJumping = false;
            animator.SetBool("isJumping", false);
        }
        //Movement logic
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * speed * Time.deltaTime);

        bool isMoving = moveInput != Vector2.zero;
        animator.SetBool("isWalking", isMoving);
        //animator.SetBool("isWaving", isRunning && isMoving);

        if (Keyboard.current.eKey.isPressed)
            animator.SetBool("isWaving", true);
        else
            animator.SetBool("isWaving", false);

        //Jump logic
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        float velocityX = Mathf.Round(moveInput.x);
        float velocityZ = Mathf.Round(moveInput.y);

        animator.SetBool("isWalking", moveInput != Vector2.zero);
        animator.SetFloat("VelocityX", moveInput.x );
        animator.SetFloat("VelocityZ", moveInput.y);

        if (isJumping)
        {
            float normalizedVelocity = Mathf.Clamp(velocity.y / 10f, -1f, 1f);
            animator.SetFloat("JumpVelocity", normalizedVelocity);
        }
    }
}
