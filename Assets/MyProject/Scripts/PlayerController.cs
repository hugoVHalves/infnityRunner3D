using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerController : MonoBehaviour
{
    private int pos = 2; // Start in middle lane (position 2)
    float NewXPos = 0f;
    private bool SwipeLeft;
    private bool SwipeRight;
    private bool SwipeUp;
    public float XValue = 2.5f; // Default lane width if not set in Inspector

    [SerializeField] private float smoothTime = 0.3f; // Tempo de suavização (menor = mais rápido)

    [SerializeField] private GameObject characterToMove; // Character to move and rotate, set in inspector
    [SerializeField] private float rotationAmount = 30f; // Maximum rotation amount

    private Vector3 currentVelocity = Vector3.zero; // Velocidade atual (usada pelo SmoothDamp)

    public Animator animator; // Reference to the animator component

    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float jumpDuration = 0.5f;
    private bool isJumping = false;
    private float jumpStartTime;
    private float startY;

    private GameManager GM;

    [Header("Mobile Input Settings")] // Mobile input variables
    [SerializeField] private bool useMobileInput = false;// Toggle between desktop and mobile inputs
    [SerializeField] private float swipeThreshold = 50f; //minimun distance for a swipe to be detected
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private bool isTouching = false;

    void Update()
    {
        if (GM.canMoveRoad)
        {
            if (useMobileInput)
            {
                MobileInputManager();
            }
            else
            {
                DesktopInputManagar();
            }
            Move();

            if (isJumping)
            {
                UpdateJump();
            }

        }
    }

    void Jump()
    {
        if (!isJumping)
        {
            if (animator != null)
            {
                animator.SetTrigger("Jump");
                Debug.Log("Jumo Triggered");
            }
            isJumping = true;
            jumpStartTime = Time.time;
            startY = characterToMove.transform.position.y;
        }
    }

    void UpdateJump()
    {
        float jumpProgress = (Time.time - jumpStartTime) / jumpDuration;
        if (jumpProgress >= 1.0f)
        {
            isJumping = false;

            Vector3 landPos = characterToMove.transform.position;
            landPos.y = startY;
            characterToMove.transform.position = landPos;
            return;
        }
        float normalizedheight = -4 * jumpHeight * (jumpProgress - 0.5f) * (jumpProgress - 0.5f) + jumpHeight;


        Vector3 newPos = characterToMove.transform.position;
        newPos.y = startY + (normalizedheight * jumpHeight);
        characterToMove.transform.position = newPos;
    }
    void Start()
    {
        // Initialize position to middle lane
        pos = 2;
        GM = GameObject.FindAnyObjectByType<GameManager>();

        if (Application.isMobilePlatform)
        {
            useMobileInput = true;
        }

    }


    void InputManager()
    {
        SwipeLeft = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        SwipeRight = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
        SwipeUp = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space);

        if (SwipeLeft)
        {
            SetValue(pos - 1);
        }
        else if (SwipeRight)
        {
            SetValue(pos + 1);
        }
        else if (SwipeUp)
        {
            Jump();
        }

        Debug.Log(pos);
    }

    void DesktopInputManagar()
    {
        SwipeLeft = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        SwipeRight = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
        SwipeUp = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space);

        if (SwipeLeft)
        {
            SetValue(pos - 1);
        }
        else if (SwipeRight)
        {
            SetValue(pos + 1);
        }
        else if (SwipeUp)
        {
            Jump();
        }
    }

    void MobileInputManager()
    {
        //reset swipe flags
        SwipeLeft = false;
        SwipeRight = false;
        SwipeUp = false;

        //handle touch input
        if (Input.touchCount > 0)
        {
            Touch touch = InputManager().GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    isTouching = true;
                    break;

                case TouchPhase.Ended:
                    if (isTouching)
                    {
                        endTouchPosition = touch.position;
                        DetectSwipe();
                        isTouching = false;
                    }
                    break;

                case TouchPhase.Canceled:
                    isTouching = false;
                    break;

            }
        }



        //process detected swipes
        if (SwipeLeft)
        {
            SetValue(pos - 1);
        }
        else if (SwipeRight)
        {

        }
        else if (SwipeUp)
        {
            Jump();
        }
    }


    void Move()
    {
        // Calculate target X position based on lane position (pos)
        // Assuming lanes are at X positions: -XValue, 0, XValue
        if (pos == 1)
            NewXPos = -XValue;
        else if (pos == 2)
            NewXPos = 0f;
        else if (pos == 3)
            NewXPos = XValue;

        // Get current position of the character
        Vector3 currentPos = characterToMove.transform.position;

        // Create target position with only the X value changing
        Vector3 targetPos = new Vector3(NewXPos, currentPos.y, currentPos.z);

        // Calculate how far we are from target X position
        float xDifference = targetPos.x - currentPos.x;

        // Apply rotation in SAME direction as movement
        // When moving right (positive xDifference), rotate right (positive angle)
        float rotationValue = Mathf.Clamp(xDifference * 15f, -rotationAmount, rotationAmount);
        Quaternion targetRotation;

        if (Mathf.Abs(xDifference) > 0.1f)
        {
            // We're moving sideways, apply rotation
            targetRotation = Quaternion.Euler(0, rotationValue, 0);
        }
        else
        {
            // We've reached the target X, return to forward facing
            targetRotation = Quaternion.identity;
        }

        // Apply rotation to the character
        characterToMove.transform.rotation = Quaternion.Slerp(
            characterToMove.transform.rotation,
            targetRotation,
            Time.deltaTime * 5f);

        // Smoothly move the character toward the target X position
        characterToMove.transform.position = Vector3.SmoothDamp(
            currentPos,
            targetPos,
            ref currentVelocity,
            smoothTime);
    }



    void SetValue(int value)
    {
        // Apply boundaries: can't go lower than 1 or higher than 3
        if (value < 1)
            pos = 1;
        else if (value > 3)
            pos = 3;
        else
            pos = value;
    }


    void detectSwipe()
    {
        Vector2 swipeVector = endTouchPosition - startTouchPosition;
        float swipeDistance = swipeVector.magnitude;

        if (swipeDistance < swipeThreshold)
        {
            return;
        }
        swipeVector.Normalize();

        if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
        {
            if (swipeVector.x > 0)
            {
                SwipeRight = true;
            }
            else
            {
                SwipeLeft = true;
            }
        }
        else
        {
            if (swipeVector.y > 0)
            {
                SwipeUp = true;
            }

        }
    }
}
