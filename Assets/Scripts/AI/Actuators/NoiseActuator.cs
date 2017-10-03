using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseActuator
{
    public delegate void MakeNoiseDelegate(NoiseActuator Self, float NoiseRadius, float AlertFactor);

    #region Serializable
    public float NoiseRadius = 5;
    public float AlertFactor = 5;
    #endregion

    #region Properties
    public Transform Owner { get; private set; }
    #endregion

    #region Private
    private static MakeNoiseDelegate OnMakeNoise;
    #endregion

    //----------------------------------------------------------------------------------------------------------------------------------------------------

    public void Init(Transform Owner, float NoiseRadius = default(float), float AlertFactor = default(float))
    {
        this.Owner = Owner;

        // Apply overrides ...
        if (NoiseRadius != default(float)) { this.NoiseRadius = NoiseRadius; }
        if (AlertFactor != default(float)) { this.AlertFactor = AlertFactor; }
    }

    public void OnMakeNoiseEvent(float AlertFactorOverride = -1, float RadiusOverride = -1)
    {
        if (OnMakeNoise == null)
        {
            Debug.Log("NoiseActuator::OnMakeNoiseEvent::If a tree falls in a forest and no one is around to hear it, does it make a sound?");
            return;
        }

        float Alert = AlertFactorOverride >= 0 ? AlertFactorOverride : AlertFactor;
        float Radius = RadiusOverride >= 0 ? RadiusOverride : NoiseRadius;

        OnMakeNoise(this, Radius, Alert);
    }

    public static void AddListener(MakeNoiseDelegate OnMakeNoise)
    {
        NoiseActuator.OnMakeNoise += OnMakeNoise;
    }

    public static void RemoveListener(MakeNoiseDelegate OnMakeNoise)
    {
        NoiseActuator.OnMakeNoise -= OnMakeNoise;
    }
}
