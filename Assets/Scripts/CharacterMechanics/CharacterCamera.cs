using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CharacterCamera : MonoBehaviour
{
    [SerializeField]
    [Tooltip(
        "The transform the camera should be positioned relative to. "
            + "Traditionally the character or a fixed point."
    )]
    public Transform FocusOn;

    [SerializeField]
    [Tooltip(
        "Shifts the focus relative to the focus transform. "
            + "The camera will still move relative to the original focus, but will "
            + "be looking at a different point."
    )]
    public Vector3 FocusOffset = Vector3.zero;

    [Space(20)]
    [SerializeField]
    [Tooltip(
        "Whether the camera can move in a fixed radius around "
            + "the character, controlled by the mouse or right stick on a gamepad."
    )]
    public bool CanOrbit = true;

    [SerializeField]
    [Tooltip("Whether the user can zoom in and out with the scroll wheel.")]
    public bool CanZoom = true;

    [Space(20)]
    [SerializeField]
    [Tooltip("The current distance from the focus with the offset applied.")]
    public float ZoomLevel = 20f;

    [SerializeField]
    [Tooltip("The shortest distance the camera can be from the focus.")]
    public float MinZoom = 10f;

    [SerializeField]
    [Tooltip("The longest distance the camera can be from the focus.")]
    public float MaxZoom = 30f;

    [SerializeField]
    public float ControllerZoomIncrement = 0.5f;

    [Space(10)]
    [SerializeField]
    [Tooltip("The camera's orbiting sensitivity from moving the mouse.")]
    float MouseRotationSensitivity = 1f;

    [SerializeField]
    [Tooltip("The camera's orbiting sensitivity from moving the right stick on a gamepad.")]
    float GamepadRotationSensitivity = 1f;

    [SerializeField]
    [Tooltip("The camera's zooming sensitivity from the scroll wheel on a mouse.")]
    float ZoomSensitivity = 0.1f;

    [Space(20)]
    [SerializeField]
    [Tooltip(
        "Whether or not the camera should push inwards to avoid intersecting with level geometry.\n\n"
            + "Note: Clipping will only be avoided on objects with colliders attached."
    )]
    bool MitigateClipping = true;

    [SerializeField]
    InputAction Rotate;

    [SerializeField]
    InputAction Zoom;

    public Vector3 velocity = Vector3.zero; // Velocity for spring-based movement

    Vector2 RawOrbitDelta;
    string OrbitInput;

    Vector3 DirectionFromFocus = -Vector3.forward;
    Vector3 CurrentOrbitRotation = Vector3.zero;
    const float Y_LIMIT = 80;

    public Transform NextCameraTransform { get; private set; }
    private Transform LastSnapPosition;
    private Transform NextTargetTransform;

    Maid maid = new();

    Camera Camera => GetComponent<Camera>();

    public void SetRotationSensitivity(float sensitivity)
    {
        MouseRotationSensitivity = sensitivity;
        GamepadRotationSensitivity = sensitivity;
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        CurrentOrbitRotation = transform.rotation.eulerAngles;

        NextCameraTransform = maid.GiveTask(new GameObject()).transform;
        NextCameraTransform.name = "NextCameraTranform";

        LastSnapPosition = maid.GiveTask(new GameObject()).transform;
        LastSnapPosition.name = "LastSnappedTransform";

        NextTargetTransform = maid.GiveTask(new GameObject()).transform;
        NextTargetTransform.name = "NextCameraTransformTarget";

        maid.GiveEvent(
            Rotate,
            "performed",
            (InputAction.CallbackContext context) =>
                OrbitInput = context.control.device.description.deviceClass
        );

        maid.GiveEvent(
            Zoom,
            "performed",
            (InputAction.CallbackContext context) =>
            {
                if (!CanZoom)
                {
                    return;
                }

                float zoomDelta =
                    context.ReadValueAsObject() is Vector2
                        ? context.ReadValue<Vector2>().normalized.y
                        : Mathf.Sign(context.ReadValue<float>()) * ControllerZoomIncrement;

                ZoomLevel = Mathf.Clamp(ZoomLevel - zoomDelta * ZoomSensitivity, MinZoom, MaxZoom);
            }
        );

        Rotate.Enable();
        Zoom.Enable();
    }

    private void OnDisable()
    {
        maid.Cleanup();
    }

    Vector2 GetRotationDeltaForFrame()
    {
        if (!CanOrbit)
        {
            return Vector2.zero;
        }

        if (OrbitInput == "Mouse")
        {
            Vector2 mouseDelta = new(-Rotate.ReadValue<Vector2>().y, Rotate.ReadValue<Vector2>().x);
            return MouseRotationSensitivity
                * Vector2.Scale(
                    mouseDelta,
                    new Vector2(1f / Camera.pixelHeight, 1f / Camera.pixelWidth)
                )
                * Time.deltaTime
                * 100;
        }

        Vector2 controllerDelta =
            new(-Rotate.ReadValue<Vector2>().y, Rotate.ReadValue<Vector2>().x);
        return controllerDelta * GamepadRotationSensitivity * Time.deltaTime;
    }

    void AddRotationDelta(Vector3 delta)
    {
        // Limit rotation around the x axis between Y_LIMIT and -Y_LIMIT
        float targetX = Mathf.Clamp(CurrentOrbitRotation.x + delta.x, -Y_LIMIT, Y_LIMIT);
        float targetY = CurrentOrbitRotation.y + delta.y;
        float targetZ = CurrentOrbitRotation.z + delta.z;
        CurrentOrbitRotation = new Vector3(targetX, targetY, targetZ);
    }

    // avoids clipping by placing the camera infront of the wall it would clip into
    void SnapForwardToAvoidClipping(Transform t)
    {
        bool didHit = Physics.Raycast(
            Focus(),
            t.position - Focus(),
            out RaycastHit hit,
            ZoomLevel,
            ControlConstants.RAYCAST_MASK,
            QueryTriggerInteraction.Ignore
        );

        if (didHit)
        {
            //setTimer();
            t.position = hit.point - (t.position - Focus()) * GetComponent<Camera>().nearClipPlane;
            LastSnapPosition.position = t.position;
        }
    }

    bool didHit(Transform t)
    {
        return Physics.Raycast(
            Focus(),
            t.position - Focus(),
            out RaycastHit hit,
            ZoomLevel,
            ControlConstants.RAYCAST_MASK,
            QueryTriggerInteraction.Ignore
        );
    }

    public Vector3 Focus()
    {
        return FocusOn.position + FocusOn.rotation * FocusOffset;
    }

    public Transform GetNextCameraTransform()
    {
        NextCameraTransform.position =
            Focus() + Quaternion.Euler(CurrentOrbitRotation) * DirectionFromFocus * ZoomLevel;

        if (MitigateClipping && didHit(NextCameraTransform))
        {
            SnapForwardToAvoidClipping(NextCameraTransform);
        }

        NextCameraTransform.LookAt(Focus(), Vector3.up);

        return NextCameraTransform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        AddRotationDelta(GetRotationDeltaForFrame() * 360);
        GetNextCameraTransform();

        transform.SetPositionAndRotation(
            NextCameraTransform.position,
            NextCameraTransform.rotation
        );
    }
}
