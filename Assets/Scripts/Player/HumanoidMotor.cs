using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class HumanoidMotor : MonoBehaviour
{
    #region Enum Helpers
    public enum EMoveMode
    {
        None,

        Crouch,
        Walk,
        Run
    }
    #endregion

    #region Serializable
    [SerializeField] private float CrouchSpeed = 2.5f;
    [SerializeField] private float WalkSpeed = 5;
    [SerializeField] private float RunSpeed = 8;
    [SerializeField] private float JumpStrength = 5;
    #endregion

    #region Properties
    public Vector3 CurrentVelocity { get { return Velocity; } }
    public Vector3 CurrentMoveDir { get { return Velocity.normalized; } }
    public float CurrentSpeed { get { return Velocity.magnitude; } }
    public EMoveMode MoveMode { get; private set; }
    #endregion

    #region Private
    private CharacterController CharController;
    private Vector3 Velocity;
    private bool JumpPressed = false;
    #endregion

    //----------------------------------------------------------------------

    #region Mono
    private void Awake()
    {
        CharController = GetComponent<CharacterController>();
        MoveMode = EMoveMode.Walk;
    }
    #endregion

    public void ApplyMovement(bool ApplyGravity = true)
    {
        // Reset falling velocity ...
        if (IsGrounded() && !JumpPressed) { Velocity.y = 0; }

        // Gravity ...
        if (ApplyGravity) { Velocity += Physics.gravity * Time.deltaTime; }

        // Apply final movement ...
        if (CharController != null) { CharController.Move(Velocity * Time.deltaTime); }

        // Reset flags ...
        JumpPressed = false;
    }

    public void SetHorizontalDirection(Vector2 InputAxis, EMoveMode Mode = EMoveMode.None, float SpeedScalar = 1, bool UseLocalSpace = true)
    {
        // Cache input values (-1 to 1) ...
        float ForwardInput = InputAxis.y;
        float StrafeInput = InputAxis.x;

        // Calculate local or world space directions ...
        Vector3 ForwardDir = (UseLocalSpace) ? this.transform.forward : Vector3.forward;
        Vector3 RightDir = (UseLocalSpace) ? this.transform.right : Vector3.right;
        
        // Get movement direction ...
        Vector3 MoveDir = ForwardDir * ForwardInput + RightDir * StrafeInput;
        // Clamp direction so diagnols don't exceed a magnitude of 1 ...
        MoveDir = Vector3.ClampMagnitude(MoveDir, 1);

        // Apply ...
        Mode = (Mode == EMoveMode.None) ? MoveMode : Mode;
        float MoveSpeed = GetMoveSpeedByMode(Mode) * SpeedScalar;

        Velocity.x = MoveDir.x * MoveSpeed;
        Velocity.z = MoveDir.z * MoveSpeed;
    }

    public void Jump(float Strength = 0, bool CheckIsGrounded = true)
    {
        // Already jumped ...
        if (JumpPressed) { return; }

        // Can't jump if not on the ground ...
        if (CheckIsGrounded && !IsGrounded()) { return; }

        float TempJumpStrength = (Strength == 0) ? JumpStrength : Strength;
        Velocity.y += TempJumpStrength;
        JumpPressed = true;
    }

    public void Stop(bool StopVerticalVelocity = false)
    {
        float VerticalVelocity = (StopVerticalVelocity) ? 0 : Velocity.y;
        
        Velocity.Set(0, VerticalVelocity, 0);
    }

    public void SetMoveMode(EMoveMode NewMode)
    {
        MoveMode = NewMode;
    }

    #region Helpers
    float GetMoveSpeedByMode(EMoveMode Mode)
    {
        switch (Mode)
        {
            case EMoveMode.Crouch:
                return CrouchSpeed;
            case EMoveMode.Walk:
                return WalkSpeed;
            case EMoveMode.Run:
                return RunSpeed;

            default:
            case EMoveMode.None:
                return 0;
        }
    }
    #endregion

    #region CharacterController Helpers
    public bool IsGrounded()
    {
        return (CharController != null && CharController.isGrounded);
    }
    #endregion
}
