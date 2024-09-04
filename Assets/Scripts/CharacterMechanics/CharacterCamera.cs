using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CharacterCamera : MonoBehaviour
{


    [SerializeField]
    [Tooltip("The transform the camera should be positioned relative to. " +
        "Traditionally the character or a fixed point.")]
    public Transform FocusOn;
    [SerializeField]
    [Tooltip("Shifts the focus relative to the focus transform. " +
        "The camera will still move relative to the original focus, but will " +
        "be looking at a different point.")]
    public Vector3 FocusOffset = Vector3.zero;

    [Space(20)]

    [SerializeField]
    [Tooltip("Whether the camera can move in a fixed radius around " +
    "the character, controlled by the mouse or right stick on a gamepad.")]
    public bool CanOrbit = true;

    [SerializeField]
    [Tooltip("Whether the user can zoom in and out with the scroll wheel.")]
    public bool CanZoom = true;

    [SerializeField]
    [Tooltip("If set, the camera's position will be confined to points within this volume. Useful for 2D " +
    "games where the camera might not want to follow the player to the edge of the playable area.")]
    public Collider LimitingVolume;

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

    [Space(10)]

    [SerializeField]
    [Tooltip("The camera's orbiting sensitivity from moving the mouse.")]
    float MouseRotationSensitivity = 1;

    [SerializeField]
    [Tooltip("The camera's orbiting sensitivity from moving the right stick on a gamepad.")]
    float GamepadRotationSensitivity = 1;

    [SerializeField]
    [Tooltip("The camera's zooming sensitivity from the scroll wheel on a mouse.")]
    float ZoomSensitivity = 1;

    [Space(20)]

    [SerializeField]
    [Tooltip("Whether or not the camera should push inwards to avoid intersecting with level geometry.\n\n" +
        "Note: Clipping will only be avoided on objects with colliders attached.")]
    bool MitigateClipping = true;

    [SerializeField]
    InputAction Rotate;

    [SerializeField]
    InputAction Zoom;


    Vector3 DirectionFromFocus = -Vector3.forward;
    Vector3 CurrentOrbitRotation = Vector3.zero;
    const float Y_LIMIT = 80;

    //TODO clean up
    GameObject emptyGO;


    // Start is called before the first frame update
    void OnEnable()
    {
        CurrentOrbitRotation = transform.rotation.eulerAngles;

        // TODO clean up
        emptyGO = new();
        emptyGO.name = "CameraHelper";

        Rotate.performed += (InputAction.CallbackContext context) =>
        {
            if (!CanOrbit)
            {
                Cursor.lockState = CursorLockMode.None;
                return;
            }

            Cursor.lockState = CursorLockMode.Locked;

            Camera cam = GetComponent<Camera>();
            Vector2 delta = new(-context.ReadValue<Vector2>().y, context.ReadValue<Vector2>().x);

            if (context.control.device.description.deviceClass == "Mouse")
            {
                delta = MouseRotationSensitivity * Vector2.Scale(delta, new Vector2(1f / cam.pixelHeight, 1f / cam.pixelWidth));
            } 
            else
            {
                delta *= GamepadRotationSensitivity;
            }

            AddRotationDelta(360 * delta);
        };

        Zoom.performed += (InputAction.CallbackContext context) =>
        {
            if (!CanZoom)
            {
                return;
            }

            ZoomLevel = Mathf.Clamp(
                ZoomLevel - Vector3.Normalize(context.ReadValue<Vector2>()).y * ZoomSensitivity,
                MinZoom, MaxZoom
            );
        };

        Rotate.Enable();
        Zoom.Enable();
    }

    private void OnDisable()
    {
        Destroy(emptyGO);
    }

    void AddRotationDelta(Vector3 delta)
    {
        // limit rotation around the x axis between Y_LIMIT and -Y_LIMIT
        CurrentOrbitRotation = new Vector3(
            Mathf.Clamp(
                CurrentOrbitRotation.x + delta.x, -Y_LIMIT, Y_LIMIT
            ),
            CurrentOrbitRotation.y + delta.y,
            CurrentOrbitRotation.z + delta.z
        );
    }

    // avoids clipping by placing the camera infront of the wall it would clip into
    void SnapForwardToAvoidClipping(Transform t)
    {
        bool didHit = Physics.Raycast(
            Focus(), t.position - Focus(),
            out RaycastHit hit, ZoomLevel, ControlConstants.RAYCAST_MASK
        );

        if (didHit)
        {
            t.position = hit.point - (t.position - Focus()) * GetComponent<Camera>().nearClipPlane;
        }

    }

    public Vector3 Focus()
    {
        return FocusOn.position + FocusOn.rotation * FocusOffset;
    }

    public Transform GetNextCameraTransform()
    {
        // TODO clean this up so we get the transform from somewhere externally
        Transform newTransform = emptyGO.transform;

        newTransform.position = Focus() + Quaternion.Euler(CurrentOrbitRotation) * DirectionFromFocus * ZoomLevel;
            
        if (MitigateClipping)
        {
            SnapForwardToAvoidClipping(newTransform);
        }

        newTransform.LookAt(Focus(), Vector3.up);

        return newTransform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Transform nextTransform = GetNextCameraTransform();

        Vector3 nextPosition = nextTransform.position;

        if (LimitingVolume != null)
        {
            nextPosition = LimitingVolume.ClosestPoint(nextPosition);
        }

        transform.SetPositionAndRotation(nextPosition, nextTransform.rotation);
    }
}
