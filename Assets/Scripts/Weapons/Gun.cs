using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    #region Serializable
    [Header("Attachments")]
    [SerializeField] private Transform MuzzleJoint;
    [SerializeField] private ParticleSystem MuzzleFlashParticles;
    [Header("Bullet Data")]
    [SerializeField] private Bullet BulletPrefab;
    [SerializeField] private float BulletLifetimeSeconds = 5;
    [Header("Specs")]
    [SerializeField] private float Range = 50;
    [SerializeField] private float BulletForce = 20;
    [SerializeField] private float RateOfFirePerSec = 0.175f;
    [SerializeField] private float HeatingCapacity = 30;
    [SerializeField] private float HeatPerShot = 1;
    [SerializeField] private float HeatingCooldownRatePerSec = 0.15f;
    #endregion

    #region Properties
    public Vector3 ShootForward         { get { return (MuzzleJoint != null) ? MuzzleJoint.forward : this.transform.forward; } }
    public Vector3 ShootOrigin          { get { return (MuzzleJoint != null) ? MuzzleJoint.position : this.transform.position; } }
    public Quaternion ShootRotation     { get { return (MuzzleJoint != null) ? MuzzleJoint.rotation : this.transform.rotation; } }
    public float OverheatingMeterRatio  { get { return Mathf.Clamp01(OverheatingCounter / HeatingCapacity); } }
    #endregion

    #region Private
    private float RateOfFireCounter = 0;
    private bool Overheating = false;
    private float OverheatingCounter = 0;
    #endregion

    //------------------------------------------------------------------------------------

    #region Mono
    private void Update()
    {
        ApplyGunOverheatingEffect();
    }
    #endregion

    public void Shoot(Vector3 BulletVelocityOffset = default(Vector3), int Layer = -1)
    {
        if (!ReadyToShoot()) { return; }

        Bullet BulletInstance = InstantiateBullet(BulletPrefab);
        if (BulletInstance != null)
        {
            Vector3 BulletVelocity = BulletVelocityOffset + GetShootingDirection() * BulletForce;
            BulletInstance.Shoot(BulletVelocity);

            PlayMuzzleFlash();
        }
        
        // Rate of fire ...
        RateOfFireCounter = RateOfFirePerSec;
        // Overheating ...
        IncrementOverheating();
    }

    public void UpdateCounters()
    {
        UpdateRateOfFireCounter();
        UpdateOverheatingCounter();
    }

    #region Rate Of Fire Helpers
    void UpdateRateOfFireCounter()
    {
        if (RateOfFireCounter > 0)
        {
            // Count down counter ...
            RateOfFireCounter -= Time.deltaTime;
            // Clamp ...
            if (RateOfFireCounter < 0) { RateOfFireCounter = 0; }
        }
    }
    #endregion

    #region Overheating Helpers
    void UpdateOverheatingCounter()
    {
        if (OverheatingCounter > 0)
        {
            // Count down counter ...
            OverheatingCounter -= HeatingCooldownRatePerSec * Time.deltaTime;

            if (OverheatingCounter <= 0)
            {
                // Clamp ...
                OverheatingCounter = 0;
                // Recovery from overheating ...
                Overheating = false;
            }
        }
    }

    void IncrementOverheating()
    {
        OverheatingCounter += HeatPerShot;
        if (OverheatingCounter >= HeatingCapacity)
        {
            Overheating = true;
            OverheatingCounter = HeatingCapacity;
        }
    }
    #endregion

    #region Helpers
    bool ReadyToShoot()
    {
        bool RateOfFireFinishedCounting = (RateOfFireCounter <= 0);

        // Other checks?
        // ...

        return RateOfFireFinishedCounting && !Overheating;
    }

    Vector3 GetShootingDirection(bool Normalize = true, int Layer = -1)
    {
        Vector3 ShootingPoint = GetShootingPoint(Layer);

        Vector3 Result = ShootingPoint - ShootOrigin;
        return (Normalize) ? Result.normalized : Result;
    }

    Vector3 GetShootingPoint(int Layer = -1)
    {
        RaycastHit HitInfo;
        Ray NewRay = new Ray(ShootOrigin, ShootForward);
        bool RayHit = Physics.Raycast(NewRay, out HitInfo, Range, Layer, QueryTriggerInteraction.Ignore);

        // No hit? Then the max range of gun ...
        Vector3 Result = ShootOrigin + ShootForward * Range;
        // Hit? Then the point we aimed at ...
        if (RayHit) { Result = HitInfo.point; }

        return Result;
    }
    #endregion

    #region Instantiation Helpers
    Bullet InstantiateBullet(Bullet Prefab, bool SetActive = true)
    {
        if (Prefab == null) { return null; }

        Bullet Result = GameObject.Instantiate(Prefab);
        if (Result != null)
        {
            Result.transform.position = ShootOrigin;
            Result.transform.rotation = ShootRotation;
            Result.gameObject.SetActive(SetActive);

            Result.Init(BulletLifetimeSeconds);
        }
        return Result;
    }
    #endregion

    #region Fun
    void PlayMuzzleFlash()
    {
        if (MuzzleFlashParticles == null) { return; }

        MuzzleFlashParticles.Play();
    }

    void ApplyGunOverheatingEffect()
    {
        Renderer MyRenderer = GetComponentInChildren<Renderer>();
        if (MyRenderer == null) { return; }
        
        if (MyRenderer.material != null)
        {
            Color OverheatingColor = Color.Lerp(Color.white, Color.red, OverheatingMeterRatio);
            MyRenderer.material.color = OverheatingColor;
        }
    }
    #endregion
}
