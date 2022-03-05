using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс отвечающий за передвижение игрока и 
/// любые другие манипуляции с миром.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Camera References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float sensetivity;
    private Vector2 lookInput;
    private float cameraPitch;
    [Space(5.0f)]

    [Header("Movement References")]
    [SerializeField] private DynamicJoystick joystick;
    [SerializeField] private CharacterController controller;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveInputDeadZone;
    private Vector2 moveTouchStartPosition;
    private Vector2 moveInput;
    [Space(5.0f)]

    [Header("Tracking touch")]
    private int leftFingerID;
    private int rightFingerID;
    private float halfScreenWidth;

    [Header("Build Handler")]
    [SerializeField] private GameObject destroyObject;
    [SerializeField] private Image destroyFill;
    [SerializeField] private float timerTouch;

    private void Awake()
    {
        leftFingerID = -1;
        rightFingerID = -1;

        halfScreenWidth = Screen.width / 2;
        moveInputDeadZone = Mathf.Pow(Screen.height / moveInputDeadZone, 2);
    }
    private void Update()
    {
        GetTouchInput();

        if (rightFingerID != -1)
            CameraControll();

        if (leftFingerID != -1)
            MovementControll();

        if (rightFingerID != -1 && leftFingerID == -1)
        {
            BuildingHandler.Instance.HelpBuild();
            if (BuildingHandler.Instance.blockHit != null)
            {
                timerTouch += Time.deltaTime;
                if (timerTouch > 0.1f)
                {
                    if (!destroyObject.activeSelf)
                        destroyObject.SetActive(true);

                    destroyFill.fillAmount = Mathf.Clamp01(timerTouch / 0.5f);
                    destroyObject.transform.position = Input.mousePosition;

                    if (timerTouch >= 0.5f)
                    {
                        BuildingHandler.Instance.DestroyControll();
                        timerTouch = 0.1f;
                    }
                }
            } else
            {
                if (destroyObject.activeSelf)
                {
                    destroyObject.SetActive(false);
                    destroyFill.fillAmount = 0;
                    timerTouch = 0.0f;
                }
            }
        }
        else
        {
            if (destroyObject.activeSelf)
            {
                destroyObject.SetActive(false);
                destroyFill.fillAmount = 0;
                timerTouch = 0.0f;
            }
        }
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
                    }
                    else if (touch.position.x > halfScreenWidth - 40 && rightFingerID == -1)
                    {
                        rightFingerID = touch.fingerId;                 
                    }
                    break;
                
                case TouchPhase.Ended:
                case TouchPhase.Canceled:

                    if (touch.fingerId == leftFingerID)
                    {
                        leftFingerID = -1;
                    }
                    else if (touch.fingerId == rightFingerID)
                    {
                        rightFingerID = -1;

                        if (timerTouch <= 0.1f)
                            BuildingHandler.Instance.BuildControll();

                        timerTouch = 0;
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

    private void MovementControll()
    {
        if (moveInput.sqrMagnitude <= moveInputDeadZone) return;

        Vector2 movementDirection = moveInput.normalized * moveSpeed * Time.deltaTime;
        controller.Move(transform.right * movementDirection.x + transform.forward * movementDirection.y);
    }

}
