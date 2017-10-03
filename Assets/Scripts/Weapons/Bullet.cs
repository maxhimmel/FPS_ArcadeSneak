using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    #region Serializable
    [SerializeField] private TrailRenderer MyTrailRenderer;
    #endregion

    #region Private
    protected Rigidbody MyRigidbody;
    protected bool DestroyOnCollision = true;
    protected System.Action<Bullet, Collider> OnCollisionEnterAction;
    #endregion

    //------------------------------------------------------

    public virtual void Init(float Lifetime = 0, bool DestroyOnCollision = true, System.Action<Bullet, Collider> OnCollisionEnter = null)
    {
        MyRigidbody = GetComponent<Rigidbody>();
        this.DestroyOnCollision = DestroyOnCollision;

        // Destroy after lifetime expires ...
        if (Lifetime > 0) { Destroy(this.gameObject, Lifetime); }
        // Listen when this bullet collides ...
        if (OnCollisionEnter != null) { OnCollisionEnterAction = OnCollisionEnter; }

        // Randomize trail color ...
        SetRandomTrailColor();
    }

    public virtual void Shoot(Vector3 Velocity)
    {
        if (MyRigidbody == null) { return; }

        MyRigidbody.AddForce(Velocity, ForceMode.VelocityChange);
    }

    #region Trigger / Collision Helpers
    protected virtual void OnTriggerEnter(Collider Other)
    {
        if (Other == null) { return; }

        if (OnCollisionEnterAction != null) { OnCollisionEnterAction(this, Other); }

        if (DestroyOnCollision) { Destroy(this.gameObject); }
    }
    #endregion

    #region TrailRenderer Helpers
    void SetRandomTrailColor()
    {
        if (MyTrailRenderer == null) { return; }

        // Find ...
        Color RandomColor = Random.ColorHSV(
            0, 1, // Hue
            1, 1, // Saturation
            0, 1, // Value
            1, 1  // Alpha
        );

        // Apply ...
        MyTrailRenderer.startColor = RandomColor;
        MyTrailRenderer.endColor = new Color(RandomColor.r, RandomColor.g, RandomColor.b, 0);
    }
    #endregion
}
