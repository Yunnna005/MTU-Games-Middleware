using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class EdController : MonoBehaviour
{
    Animator animator;
    CharacterController controller;
    [SerializeField] private Rig leftHandRig;
    [SerializeField] private Transform ikTarget;
    [SerializeField] private Transform objectToPick;
    [SerializeField] private Transform handBone;

    private float speed = 2f;
    private float jumpHeight = 4f;
    private float gravity = -9.81f;

    Vector2 moveInput;
    Vector3 velocity;
    private bool isJumping;
    private bool isPickingUp = false;

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
        bool isGrounded = controller.isGrounded || velocity.y <= 0.1f;

        if (context.performed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;
            animator.SetBool("isJumping", true);
            animator.SetFloat("JumpVelocity", 1f);
        }
    }
    public void OnWave(InputAction.CallbackContext context)
    {
        if (context.performed)
            animator.SetBool("isWaving", true);
        else if (context.canceled)
            animator.SetBool("isWaving", false);
    }

    public void OnPickUp(InputAction.CallbackContext context)
    {
        if (context.performed && !isPickingUp)
        {
            StartCoroutine(PickUpRoutine());
        }
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

    private IEnumerator PickUpRoutine()
    {
        isPickingUp = true;

        ikTarget.position = objectToPick.position;
        ikTarget.rotation = objectToPick.rotation;

        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime * 2f;
            leftHandRig.weight = Mathf.Lerp(0f, 1f, timer);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        objectToPick.SetParent(handBone);
        objectToPick.localPosition = Vector3.zero;
        objectToPick.localRotation = Quaternion.identity;

        timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime * 2f;
            leftHandRig.weight = Mathf.Lerp(1f, 0f, timer);
            yield return null;
        }

        isPickingUp = false;
    }

}
