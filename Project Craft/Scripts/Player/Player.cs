using UnityEngine;

public class Player : MonoBehaviour
{
    private bool isGrounded;

    [Header("Main")]
    [SerializeField] private World world;
    [SerializeField] private Toolbar toolbar;
    [SerializeField] private Transform cameraTransform;
    private float sensetivity = 2.5f;
    private Vector2 lookInput;
    private float cameraPitch;

    [Header("Touch")]
    private int leftFingerID;
    private int rightFingerID;
    private float rightFingerWidthScreen;
    private float toolbarHeightScreen;
    private float touchTimer_r = 0.0f;

    [Header("Block Settings")]
    [SerializeField] private Transform highlightBlock;
    [SerializeField] private Transform placeBlock;
    private float checkIncrement = 0.1f;
    private float reach = 8.0f;

    [Header("BlockUI")]
    [SerializeField] private GameObject breakImage;
    [SerializeField] private UnityEngine.UI.Image breakFill;
    private float timeToDestroyBlock = 2.0f;
    private float breakTimer = 0.0f;
    public byte selectedBlockIndex = 1;

    [Header("Contoll Settigns")]
    private const float jumpForce = 5.0f;
    private const float walkSpeed = 3.0f;
    private const float gravity = -9.8f;
    private const float playerWidth = 0.3f;

    private float horizontal;
    private float vertical;
    private Vector3 velocity;
    private float verticalMomentum = 0;
    private bool jumpRequest;
    public int orientation;

    [Header("Movement References")]
    [SerializeField] private FixedJoystick joystick;
    [SerializeField] private float moveInputDeadZone;

    private void Start()
    {
        leftFingerID = -1;
        rightFingerID = -1;

        rightFingerWidthScreen = Screen.width / 4;
        toolbarHeightScreen = Screen.height / 8;

        joystick.OnJoystickClickHandler.AddListener(() =>
        {
            if (isGrounded && jumpRequest) Jump();
        });

    }

    private void Update()
    {
        GetInputTouches();

        if (rightFingerID != -1)
            CameraControll();

        if (leftFingerID != -1)
            GetPlayerInputs();

        Vector3 XZDirection = transform.forward;
        XZDirection.y = 0;
        if (Vector3.Angle(XZDirection, Vector3.forward) <= 45)
            orientation = 0; // Player is facing forwards.
        else if (Vector3.Angle(XZDirection, Vector3.right) <= 45)
            orientation = 5;
        else if (Vector3.Angle(XZDirection, Vector3.back) <= 45)
            orientation = 1;
        else
            orientation = 4;

        if (breakImage.activeSelf)
        {
            breakFill.fillAmount = Mathf.Clamp01(breakTimer / timeToDestroyBlock);
            breakImage.transform.position = Input.GetTouch(rightFingerID).position;
        }
    }

    private void FixedUpdate()
    {
        CalculateVelocity();

        if (leftFingerID != -1)
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

    private void GetInputTouches()
    {
        for(int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            switch (touch.phase)
            {
                case TouchPhase.Began:

                    if(touch.position.x < rightFingerWidthScreen && leftFingerID == -1)
                    {
                        leftFingerID = touch.fingerId;
                    }
                    else if(touch.position.x > rightFingerWidthScreen && rightFingerID == -1 && touch.position.y > toolbarHeightScreen)
                    {
                        rightFingerID = touch.fingerId;
                        PlaceInputBlocks();
                    }

                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                   
                    breakImage.SetActive(false);

                    if (touch.fingerId == leftFingerID)
                    {
                        leftFingerID = -1;

                    }
                    else if (touch.fingerId == rightFingerID)
                    {
                        if (touchTimer_r < 0.1f && touchTimer_r > 0.05f && toolbar.isNotEmptyChoosedSlot)
                            world.GetChunkFromVector3(placeBlock.position).EditVoxel(placeBlock.position, selectedBlockIndex);

                        rightFingerID = -1;
                        breakTimer = 0.0f;
                        touchTimer_r = 0.0f;
                        breakImage.SetActive(false);

                    }
                    break;
                case TouchPhase.Stationary:
                    if(touch.fingerId == rightFingerID)
                    {
                        lookInput = Vector2.zero;
                        touchTimer_r += Time.deltaTime;
                        if(touchTimer_r > 0.25f && PlaceInputBlocks())
                        {
                            breakImage.SetActive(true);
                            breakTimer += Time.deltaTime;
                            if (breakTimer > timeToDestroyBlock)
                            {
                                world.GetChunkFromVector3(highlightBlock.position).EditVoxel(highlightBlock.position, 0);
                                breakTimer = 0.5f;
                            }
                        }
                    }
                    break;
                case TouchPhase.Moved:
                    if(touch.fingerId == rightFingerID)
                    {
                        lookInput = touch.deltaPosition * sensetivity * Time.deltaTime;
                        breakTimer = 0.0f;
                        touchTimer_r = 0.0f;
                        breakImage.SetActive(false);
                    }
                    break;
                
            }
        }
    }

    private bool PlaceInputBlocks()
    {
        if (rightFingerID == -1)
            return false;

        float step = checkIncrement;
        Vector3 lastPos = new Vector3();

        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(rightFingerID).position);

        while (step < reach)
        {
            Vector3 pos = Camera.main.transform.position + ray.direction * step;
            
            if(world.CheckForVoxel(pos))
            {
                highlightBlock.position = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
                placeBlock.position = lastPos;

                highlightBlock.gameObject.SetActive(true);
                placeBlock.gameObject.SetActive(true);

                return true;
            }

            lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
            step += checkIncrement;
        }
        highlightBlock.gameObject.SetActive(false);
        placeBlock.gameObject.SetActive(false);

        return false;
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
