using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Serializable
    [SerializeField] private Transform PlayerAvatar;
    [SerializeField] private Transform CameraJoint;
    [SerializeField] private Gun CurrentGun;
    #endregion

    #region Properties
    public HumanoidMotor PlayerMotor        { get; private set; }
    public FPSCameraController FPSCamera    { get; private set; }
    #endregion

    #region Private
    #endregion

    //--------------------------------------------------------------------------------------------

    #region Mono
    private void Awake()
    {
        InitFPSCamera();
        InitPlayerMovement();
    }

    private void Update()
    {
        UpdatePlayerMotor();

        if (Input.GetAxis("Trigger_Axis_1") < 0) { ShootGun(); }
    }

    private void LateUpdate()
    {
        UpdateFPSCamera();

        UpdateGuns();
    }
    #endregion

    #region Init
    void InitFPSCamera()
    {
        if (CameraJoint != null && FPSCamera == null)
        {
            FPSCamera = CameraJoint.GetComponentInChildren<FPSCameraController>();
            if (FPSCamera == null)
            {
                FPSCamera = (Camera.main != null) ? Camera.main.GetComponent<FPSCameraController>() : null;
                if (FPSCamera != null)
                {
                    FPSCamera.transform.SetParent(CameraJoint);
                    FPSCamera.transform.localPosition = Vector3.zero;
                    FPSCamera.transform.rotation = CameraJoint.rotation;
                }
            }
        }
    }

    void InitPlayerMovement()
    {
        if (PlayerAvatar != null && PlayerMotor == null)
        {
            PlayerMotor = PlayerAvatar.GetComponent<HumanoidMotor>();
            if (PlayerMotor == null)
            {
                PlayerMotor = PlayerAvatar.gameObject.AddComponent<HumanoidMotor>();
            }
        }
    }
    #endregion

    #region PlayerMotor Helpers
    void UpdatePlayerMotor()
    {
        if (PlayerMotor == null) { return; }

        Vector2 InputDir = new Vector2(Input.GetAxis("L_XAxis_1"), Input.GetAxis("L_YAxis_1"));
        bool Jump = Input.GetButtonDown("A_1");

        if (Jump) { PlayerMotor.Jump(); }
        PlayerMotor.SetHorizontalDirection(InputDir);
        PlayerMotor.ApplyMovement();
    }
    #endregion

    #region FPSCamera Helpers
    void UpdateFPSCamera()
    {
        if (FPSCamera == null) { return; }

        Vector2 InputDir = new Vector2(Input.GetAxis("R_XAxis_1"), Input.GetAxis("R_YAxis_1"));
        FPSCamera.RotateCamera(InputDir);
    }
    #endregion

    #region Gun Helpers
    void ShootGun()
    {
        if (CurrentGun == null) { return; }

        // Add player's velocity to bullet's so the player won't catch-up to the bullet ...
        Vector3 PlayerVelocity = (PlayerMotor != null) ? PlayerMotor.CurrentVelocity : Vector3.zero;

        CurrentGun.Shoot(PlayerVelocity);
    }

    void UpdateGuns()
    {
        if (CurrentGun != null) { CurrentGun.UpdateCounters(); }
    }
    #endregion
}
