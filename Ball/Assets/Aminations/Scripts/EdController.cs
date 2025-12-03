using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Audio;
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
    private float throwForce = 2f;

    private float speed = 2f;
    private float jumpHeight = 4f;
    private float gravity = -9.81f;

    Vector2 moveInput;
    Vector3 velocity;
    private bool isJumping;
    private bool isPickingUp = false;
    private bool isHolding = false;
    private float maxReachDistance = 1.5f;

    BallController heldBall;

    private AudioSource footstepAudioSource;

    private void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        animator.SetBool("isWaving", false);
        footstepAudioSource = GetComponent<AudioSource>();

    }
    public void OnAWSD(InputAction.CallbackContext context)
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
    public void OnEWave(InputAction.CallbackContext context)
    {
        if (context.performed)
            animator.SetBool("isWaving", true);
        else if (context.canceled)
            animator.SetBool("isWaving", false);
    }

    public void OnRPickUp(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (isPickingUp || isHolding) return;

        BallController nearest = FindNearestFreeBall();
        if (nearest == null)
        {
            Debug.Log("No reachable ball");
            return;
        }

        float dist = Vector3.Distance(transform.position, nearest.transform.position);
        if (dist > maxReachDistance)
        {
            Debug.Log("Ball out of reach");
            return;
        }

        StartCoroutine(PickUpRoutine(nearest));
    }

    public void OnTThrow(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!isHolding || heldBall == null) return;

        heldBall.Throw(transform.forward, throwForce);
        isHolding = false;
        heldBall = null;
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

    private IEnumerator PickUpRoutine(BallController ball)
    {
        isPickingUp = true;

        ikTarget.position = ball.transform.position;
        ikTarget.rotation = ball.transform.rotation;

        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime * 2f;
            leftHandRig.weight = Mathf.Lerp(0f, 1f, timer);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        bool picked = ball.TryPickUp(transform, handBone);
        if (picked)
        {
            isHolding = true;
            heldBall = ball;
        }

        timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime * 2f;
            leftHandRig.weight = Mathf.Lerp(1f, 0f, timer);
            yield return null;
        }

        isPickingUp = false;
    }
    BallController FindNearestFreeBall()
    {
        // fetch all Ball components in the scene.
        BallController[] balls = FindObjectsOfType<BallController>();
        BallController nearest = null;
        float bestDist = float.MaxValue;

        foreach (BallController b in balls)
        {
            if (b.IsHeld) continue;
            float d = Vector3.Distance(transform.position, b.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                nearest = b;
            }
        }

        if (bestDist <= maxReachDistance) return nearest;
        return null;
    }

    public void Steps()
    {
        Debug.Log("Step sound triggered!");
        footstepAudioSource.Play();
    }

}
