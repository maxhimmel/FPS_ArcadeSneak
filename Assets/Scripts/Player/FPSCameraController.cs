using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCameraController : MonoBehaviour
{
    #region Consts
    private const float MaxUpDownAngle = 90;
    #endregion

    #region Serializable
    [SerializeField] private Transform Avatar;
    [SerializeField] private float RotateAngleSpeed = 60;
    [Range(0, MaxUpDownAngle)]
    [SerializeField] private float MaxUpAngle = -90;
    [Range(0, MaxUpDownAngle)]
    [SerializeField] private float MaxDownAngle = 90;
    [SerializeField] private bool Invert = false;
    [Header("Fun")]
    [SerializeField] private float BackgroundColorTransitionSpeed = 0.35f;
    #endregion

    #region Properties
    public Camera MyCamera { get; private set; }

    private float MyUpDownAngle { get { return this.transform.localEulerAngles.x; } }
    #endregion

    //-------------------------------------------------------------------------------

    #region Mono
    private void Awake()
    {
        if (MyCamera == null) { MyCamera = GetComponent<Camera>(); }
        StartLerpingCameraColor(Random.ColorHSV(), Random.ColorHSV());
    }
    #endregion

    public void RotateCamera(Vector2 InputDir, float Speed = 0)
    {
        // Can't turn without an avatar ...
        if (Avatar == null) { return; }

        float TempSpeed = (Speed == 0) ? RotateAngleSpeed : Speed;
        float UpDownInput = InputDir.y * ((Invert) ? -1 : 1);
        float TurnInput = InputDir.x;

        // Look up/down ...
        this.transform.Rotate(Vector3.right, TempSpeed * UpDownInput * Time.deltaTime);

        // Don't wanna look so far the camera turns upside down ...
        ApplyUpDownRotationClamp();

        // Turn avatar ...
        Avatar.Rotate(Avatar.up, TempSpeed * TurnInput * Time.deltaTime);
    }

    #region Helpers
    public void SetInvert(bool Inverted)
    {
        this.Invert = Inverted;
    }

    void ApplyUpDownRotationClamp()
    {
        if (Avatar == null) { return; }

        // Get directions ...
        Vector3 MyForward = this.transform.forward;
        Vector3 AvatarForward = Avatar.forward;
        Vector3 AvatarUp = Avatar.up;
        Vector3 AvatarRight = Avatar.right;

        // Get up/down angle ...
        float ForwardDotAvatarForward = Vector3.Dot(MyForward, AvatarForward);
        float AngleBetween = Mathf.Acos(ForwardDotAvatarForward) * Mathf.Rad2Deg;

        // Clamping conditions ...
        bool AboveMaxUpAngle = (AngleBetween > MaxUpAngle);
        bool BelowMaxDownAngle = (AngleBetween > MaxDownAngle);

        // No need to clamp ...
        if (!AboveMaxUpAngle && !BelowMaxDownAngle) { return; }

        // Get which direction looking (up::1 OR down::-1) ...
        int LookDir = (int) Mathf.Sign(Vector3.Dot(MyForward, AvatarUp));
        bool LookingDown = (LookDir < 0);

        // Find clamp value ...
        float ClampedAngle = MyUpDownAngle;
        if (LookingDown)    { ClampedAngle = MaxDownAngle; }
        else                { ClampedAngle = -MaxUpAngle; }

        // Apply ...
        float MyLocalEuler_Y = this.transform.localEulerAngles.y;
        float MyLocalEuler_Z = this.transform.localEulerAngles.z;
        this.transform.localEulerAngles = new Vector3(ClampedAngle, MyLocalEuler_Y, MyLocalEuler_Z);
    }
    #endregion

    #region Background Color Lerping
    void StartLerpingCameraColor(Color To, Color From)
    {
        if (MyCamera == null) { return; }

        float Counter = 0;
        Color NewColor = To;

        CoroutineContainer.ConditionPredicate Condition = () =>
        {
            return (NewColor == From);
        };
        System.Action PerConditionCheck = () =>
        {
            Counter += Time.deltaTime * BackgroundColorTransitionSpeed;
            NewColor = Color.Lerp(To, From, Counter);

            if (MyCamera != null) { MyCamera.backgroundColor = NewColor; }
        };

        CoroutineContainer.StartACoroutine(Condition, PerConditionCheck, () =>
        {
            StartLerpingCameraColor(From, Random.ColorHSV());
        });
    }
    #endregion
}
