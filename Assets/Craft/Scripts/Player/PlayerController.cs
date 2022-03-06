using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Êëàññ îòâå÷àþùèé çà ïåðåäâèæåíèå èãðîêà è 
/// ëþáûå äðóãèå ìàíèïóëÿöèè ñ ìèðîì.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Player Logical")]
    public bool isGrounded;

    [Header("Main References")]
    [SerializeField] private World world;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float sensetivity;  
    private Vector2 lookInput;
    private float cameraPitch;

    [Header("Main Settings")]
    private const float jumpForce = 5.0f;
    private const float walkSpeed = 3.0f;
    private const float gravity = -9.8f;
    private const float playerWidth = 0.3f;
    private const float boundsTolerance = 0.1f;
  

    private float horizontal;
    private float vertical;
    private Vector3 velocity;
    private float verticalMomentum = 0;
    private bool jumpRequest;


    [Header("Tracking Touch")]
    private int leftFingerID;
    private int rightFingerID;
    private float halfScreenWidth;
    [SerializeField] private float touchTimer_r;

    [Header("Movement References")]
    [SerializeField] private FixedJoystick joystick;
    [SerializeField] private float moveInputDeadZone;
    private Vector2 moveTouchStartPosition;
    private Vector2 moveInput;

    /// <summary>
    ///  Block Area
    /// </summary>
    [Header("Block Settings")]
    [SerializeField] private Transform highlightBlock;
    [SerializeField] private Transform placeBlock;
    [SerializeField] private float checkIncrement = 0.1f;
    [SerializeField] private float reach = 8.0f;

    [Header("Block UI")]
    [SerializeField] private Text selectedBlockText;
    [SerializeField] private byte selectedBlockIndex = 1;

    private void Start()
    {
        leftFingerID = -1;
        rightFingerID = -1;

        halfScreenWidth = Screen.width / 2;
        moveInputDeadZone = Mathf.Pow(Screen.height / moveInputDeadZone, 2);
        selectedBlockText.text = $"{world.blockTypes[selectedBlockIndex].name} block selected";
    }

    private void Update()
    {
        GetTouchInput();

        if (rightFingerID != -1)
            CameraControll();

        if (leftFingerID != -1)
            GetPlayerInputs();

        touchTimer_r = rightFingerID != -1 ? touchTimer_r += Time.deltaTime : touchTimer_r = 0.0f;

        PlaceCursorBlocks();
    }

    private void FixedUpdate()
    {
        CalculateVelocity();
        
        if(leftFingerID != -1)
        transform.Translate(velocity, Space.World);
        else
        {
            velocity.x = 0;
            velocity.z = 0;
            transform.Translate(velocity, Space.World);
        }

        if (isGrounded)
            jumpRequest = true;
    }

    private void GetTouchInput()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            switch (touch.phase)
            {
                case TouchPhase.Began:

                    if (touch.position.x < halfScreenWidth - 40 && leftFingerID == -1)
                    {
                        leftFingerID = touch.fingerId;
                        moveTouchStartPosition = touch.position;
                        Debug.Log("Left Finger Touch");
                    }
                    else if (touch.position.x > halfScreenWidth - 40 && rightFingerID == -1)
                    {
                        rightFingerID = touch.fingerId;
                        Debug.Log("Right Finger Touch");
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:

                    if (touch.fingerId == leftFingerID)
                    {
                        leftFingerID = -1;
                        Debug.Log("Left Finger End Touch");
                    }
                    else if (touch.fingerId == rightFingerID)
                    {
                        if (touchTimer_r < 0.15f)
                            world.GetChunkFromVector3(placeBlock.position).EditVoxel(placeBlock.position, selectedBlockIndex);

                        rightFingerID = -1;
                        Debug.Log("Right Finger End Touch");
                    }

                    break;

                case TouchPhase.Moved:

                    if (touch.fingerId == rightFingerID)
                        lookInput = touch.deltaPosition * sensetivity * Time.deltaTime;
                    else if (touch.fingerId == leftFingerID)
                        moveInput = touch.position - moveTouchStartPosition;
                    break;

                case TouchPhase.Stationary:

                    if (touch.fingerId == rightFingerID)
                    {
                        lookInput = Vector2.zero;
                    }

                    break;
            }
        }
    }

    private void CameraControll()
    {

        cameraPitch = Mathf.Clamp(cameraPitch - lookInput.y, -90.0f, 90.0f);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);

        transform.Rotate(transform.up, lookInput.x);
    }

    private void GetPlayerInputs()
    {
         horizontal = joystick.Horizontal;
         vertical = joystick.Vertical;
    }

    private void Jump()
    {

        verticalMomentum = jumpForce;
        isGrounded = false;
        jumpRequest = false;

    }

    private void CalculateVelocity()
    {
        if (verticalMomentum > gravity)
            verticalMomentum += Time.fixedDeltaTime * gravity;

        velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * walkSpeed;
        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        if ((velocity.z > 0 && front) || (velocity.z < 0 && back))
            velocity.z = 0;
        if ((velocity.x > 0 && right) || (velocity.x < 0 && left))
            velocity.x = 0;

        if (velocity.y < 0)
            velocity.y = ÑheckDownSpeed(velocity.y);
        else if (velocity.y > 0)
            velocity.y = ÑheckUpSpeed(velocity.y);

        if (front || back || right || left)
            if (jumpRequest)
                Jump();
    }

    private void PlaceCursorBlocks()
    {
        float step = checkIncrement;
        Vector3 lastPos = new Vector3();

        while (step < reach)
        {
            Vector3 pos = cameraTransform.position + (cameraTransform.forward * step);

            if (world.CheckForVoxel(pos))
            {
                highlightBlock.position = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
                placeBlock.position = lastPos;

                highlightBlock.gameObject.SetActive(true);
                placeBlock.gameObject.SetActive(true);

                return;
            }

            lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
            step += checkIncrement;
        }

        highlightBlock.gameObject.SetActive(false);
        placeBlock.gameObject.SetActive(false);
    }

    private float ÑheckDownSpeed(float downSpeed)
    {
        if (
           world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) ||
           world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) ||
           world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth)) ||
           world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth))
          )
        {
            isGrounded = true;
            return 0;
        }
        else
        {
            isGrounded = false;
            return downSpeed;
        }
    }

    private float ÑheckUpSpeed(float upSpeed)
    {
        if (
            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 2f + upSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 2f + upSpeed, transform.position.z + playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z + playerWidth))
           )
        {
            return 0;
        }
        else
        {
            return upSpeed;
        }

    }

    public bool front
    {

        get
        {
            if (
                world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z + playerWidth)) ||
                world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + playerWidth))
                )
                return true;
            else
                return false;
        }

    }
    public bool back
    {

        get
        {
            if (
                world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z - playerWidth)) ||
                world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth))
                )
                return true;
            else
                return false;
        }

    }
    public bool left
    {

        get
        {
            if (
                world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y, transform.position.z)) ||
                world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z))
                )
                return true;
            else
                return false;
        }

    }
    public bool right
    {

        get
        {
            if (
                world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y, transform.position.z)) ||
                world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z))
                )
                return true;
            else
                return false;
        }

    }
}
