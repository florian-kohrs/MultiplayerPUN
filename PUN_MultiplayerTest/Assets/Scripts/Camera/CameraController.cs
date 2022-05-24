using UnityEngine;

public class CameraController : PunLocalBehaviour, IVector2InputListener
{

    public float sensitivity = 20.0f;
    public float smoothing = 5.0f;

    protected const float IS_IN_FPP_AT = 0.85f;

    public Transform followTarget;

    public Transform horizontalRotate;

    public Transform verticalRotateTarget;

    public float mouseMaxSpeedPerFrame = 10;

    public Vector2 mouseLook;
    private Vector2 smoothedMouseLook;

    public float maxDownRotation = -30;

    public float maxUpRotation = 30;

    public Transform maxCameraScrollAnchor;

    public Transform minCameraScrollAnchor;

    public Transform cameraTransform;

    public float scrollSpeed = 0.01f;

    public float scrollZoomValue = 0;


    protected Vector2 mouseInput;

    public bool IsInFPP => scrollZoomValue > IS_IN_FPP_AT;

    protected override void OnStart()
    {
        float startZEuler = horizontalRotate.localEulerAngles.z;
        if (startZEuler > 180)
        {
            startZEuler -= 360;
        }
        startZEuler *= -1;

        mouseLook = new Vector2(horizontalRotate.localEulerAngles.y, startZEuler);
        smoothedMouseLook = mouseLook;
        GameManager.InputHandler.AddMouseListener(this);
        GameManager.InputHandler.RegisterEvent((i) =>
            i.PlayerMovement.CameraScroll.performed += input => ScrollCamera(input.ReadValue<Vector2>().y));
    }


    protected override void OnNotMine()
    {
        Destroy(gameObject);
    }


    private void ScrollCamera(float delta)
    {
        scrollZoomValue = Mathf.Clamp01(scrollZoomValue + delta * Time.deltaTime * scrollSpeed);
        //if (delta < 0 && IsInFPP)
        //    scrollZoomValue = IS_IN_FPP_AT;
        //else if (delta > 0 && IsInFPP)
        //    scrollZoomValue = 1;

        float z = 1 - Mathf.Cos(scrollZoomValue / 2 * Mathf.PI);
        float y = Mathf.Sin(scrollZoomValue / 2 * Mathf.PI);

        float newZPos = Mathf.Lerp(minCameraScrollAnchor.localPosition.z, maxCameraScrollAnchor.localPosition.z, z);
        float newYPos = Mathf.Lerp(minCameraScrollAnchor.localPosition.y, maxCameraScrollAnchor.localPosition.y, y);

        cameraTransform.localPosition = new Vector3(0, newYPos, newZPos);
        cameraTransform.rotation = Quaternion.Lerp(minCameraScrollAnchor.rotation, maxCameraScrollAnchor.rotation, scrollZoomValue);
    }

    private void LateUpdate()
    {
        if(GameManager.CanCameraMove)
            UpdateCamera();
    }

    public void UpdateCamera()
    {
        RotateCamera();
        //ScrollCamera();
        //transform.position = followTarget.position;
    }

    public void RotateCamera()
    {
        //float sensitivity = PersistentGameDataController.Settings.cameraSensitivity;

        Vector2 mouseMovement = mouseInput;
        mouseMovement *= sensitivity * smoothing * Time.deltaTime;


        mouseMovement = new Vector2(
            Mathf.Clamp(mouseMovement.x, -mouseMaxSpeedPerFrame, mouseMaxSpeedPerFrame),
            Mathf.Clamp(mouseMovement.y, -mouseMaxSpeedPerFrame, mouseMaxSpeedPerFrame)
            );

        mouseLook += mouseMovement;

        mouseLook.y = Mathf.Clamp(mouseLook.y, maxDownRotation, maxUpRotation);
        
        smoothedMouseLook.y = Mathf.Lerp(smoothedMouseLook.y, mouseLook.y, 1 / smoothing);
        smoothedMouseLook.x = Mathf.Lerp(smoothedMouseLook.x, mouseLook.x, 1 / smoothing);

        float yDelta = smoothedMouseLook.y - lastSmothedMouseLook.y;
        float xDelta = smoothedMouseLook.x - lastSmothedMouseLook.x;

        verticalRotateTarget.transform.Rotate(-yDelta, 0, 0, Space.Self);
        horizontalRotate.transform.Rotate(0, xDelta, 0, Space.Self);


        //horizontalRotate.transform.localEulerAngles = new Vector3(0, smoothedMouseLook.x, horizontalRotate.transform.localEulerAngles.z);
        //verticalRotateTarget.transform.localEulerAngles = new Vector3(-smoothedMouseLook.y, 0, 0);

        lastSmothedMouseLook = smoothedMouseLook;

    }

    public void OnVector2Input(Vector2 input)
    {
        mouseInput = input;
    }


    protected Vector2 lastSmothedMouseLook;

}
