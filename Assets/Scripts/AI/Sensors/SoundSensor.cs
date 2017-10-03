using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SoundSensor
{
    public delegate void WhenObjectHeardDelegate(NoiseActuator Other, float AlertFactor);

    #region Serializable
    public Transform Head;
    public float HearingRadius = 15f;
    #endregion

    #region Debugging
    [Header("Debugging")]
    public bool DrawSensorGizmo = true;
    public Color GizmoColor = Color.yellow;
    #endregion
    
    #region Private
    private WhenObjectHeardDelegate WhenObjectHeard;
    //private Animal.AnimalBase Owner;
    #endregion

    //-----------------------------------------------------------------------------------------------------------

    public void Init(Transform HeadJoint)//, Animal.AnimalBase Owner)
    {
        //this.Owner = Owner;
        if (Head == null) { Head = HeadJoint; }

        // Each sound sensor will subscribe to the global make noise event.
        // This way, when a noise is made every sound sensor will check if it heard the noise ...
        NoiseActuator.AddListener(HeardGameObject);
    }

    public void HeardGameObject(NoiseActuator Other, float OtherNoiseRadius, float AlertFactor)
    {
        // Do nothing if I heard myself make noise ...
        if (Other == null) { return; }// == Owner.NoiseActuator) { return; }

        if (Other == null || Head == null) { return; }
        if (Other.Owner == null || Head.transform == null) { return; }

        Vector3 HeadToOther = Other.Owner.position - Head.transform.position;
        float DistToOther = HeadToOther.magnitude;

        if (DistToOther <= HearingRadius + OtherNoiseRadius)
        {
            if (WhenObjectHeard != null)
            {
                WhenObjectHeard(Other, AlertFactor);
                return;
            }

            Debug.LogWarning("SoundSensor::HeardGameObject::No 'WhenObjectHeard' callback has been set!");
        }
    }

    public void SetOnHeardAction(WhenObjectHeardDelegate OnHeardCallback)
    {
        WhenObjectHeard = OnHeardCallback;
    }

    private void OnValidate()
    {
        if (HearingRadius < 0)
        {
            HearingRadius = 0;
        }
    }
}

//#if UNITY_EDITOR
//public class SoundSensorGizmoRenderer
//{
//    [DrawGizmo(GizmoType.Active | GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
//    static void DrawSoundSensor(Animal.AnimalBase animal, GizmoType gizmoType)
//    {
//        SoundSensor sensor = animal.soundSensor;

//        if (sensor == null || !sensor.DrawSensorGizmo)
//        {
//            return;
//        }

//        Transform sensorCenter = (sensor.Head == null) ?
//            animal.transform : sensor.Head;

//        // set rendering color
//        Gizmos.color = sensor.GizmoColor;

//        // render!
//        Gizmos.DrawWireSphere(sensorCenter.position, sensor.HearingRadius);
//    }
//}

//[CustomEditor(typeof(SoundSensor))]
//public class SoundSensorEditorMenu : Editor
//{
//    [MenuItem("Custom/Sensors/Sound/Hide All Gizmos")]
//    public static void HideAllSensorGizmos()
//    {
//        Animal.AnimalBase[] animals = GameObject.FindObjectsOfType<Animal.AnimalBase>();
//        foreach (Animal.AnimalBase animal in animals)
//        {
//            if (animal.soundSensor != null)
//            {
//                animal.soundSensor.drawSensorGizmo = false;
//            }
//        }

//        SceneView.RepaintAll();
//    }

//    [MenuItem("Custom/Sensors/Sound/Show All Gizmos")]
//    public static void ShowAllSensorGizmos()
//    {
//        Animal.AnimalBase[] animals = GameObject.FindObjectsOfType<Animal.AnimalBase>();
//        foreach (Animal.AnimalBase animal in animals)
//        {
//            if (animal.soundSensor != null)
//            {
//                animal.soundSensor.drawSensorGizmo = true;
//            }
//        }

//        SceneView.RepaintAll();
//    }
//}
//#endif