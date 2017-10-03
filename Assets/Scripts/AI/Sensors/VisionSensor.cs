using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class VisionSensor
{
    #region Serializable
    public Transform Head;
    public float VisionRange = 10; // how far can the sensor check?
    [Range(0, 360f)]
    public float FieldOfView = 60;
    #endregion

#pragma warning disable 414 // Disable warnings about privates being assigned to but not used ...
    //private Animal.AnimalBase Owner;
#pragma warning restore 414 // Disable warnings about privates being assigned to but not used ...

    #region Debugging
    [Header("Debugging")]
    public bool DrawSensorGizmo = true;
    public Color GizmoColor = Color.cyan;
    #endregion
    
    //-------------------------------------------------------------------------------------------------------------------------------------------------

    public void Init(Transform HeadJoint)//, Animal.AnimalBase Owner)
    {
        //this.Owner = Owner;
        if (Head == null) { Head = HeadJoint; }
    }
    
    public bool IsGameObjectInSight(Transform Other, Vector3 OtherPos = default(Vector3), LayerMask LayerToCheckFor = default(LayerMask))
    {
        if (Other == null) { return false; }

        // Apply overrides ...
        if (OtherPos == default(Vector3)) { OtherPos = Other.position; }
        if (LayerToCheckFor == default(LayerMask)) { LayerToCheckFor = -1; }

        Vector3 HeadToOther = OtherPos - Head.position;

        // Other object is out of range ...
        if (Vector3.SqrMagnitude(HeadToOther) > VisionRange * VisionRange) { return false; }

        Vector3 DirToOther = HeadToOther.normalized;
        float AngleFromHeadsForwardToOthersPosition = Vector3.Angle(Head.forward, DirToOther);
        
        // Other object is inside my cone of vision ...
        if (AngleFromHeadsForwardToOthersPosition <= FieldOfView / 2f)
        {
            // Make ray pointing directly to my object ...
            Ray ray = new Ray(Head.position, DirToOther);
            RaycastHit hitInfo;

            if (DrawSensorGizmo) { Debug.DrawRay(ray.origin, ray.direction * VisionRange, GizmoColor); }

            // Direct line of sight to my object ...
            if (Physics.Raycast(ray, out hitInfo, VisionRange, LayerToCheckFor))
            {
                if (hitInfo.transform == Other) { return true; }
            }
        }

        return false;
    }

    private void OnValidate()
    {
        if (VisionRange < 0)
        {
            VisionRange = 0;
        }
    }
}

//#if UNITY_EDITOR
//public class VisionSensorGizmoRenderer
//{
//    [DrawGizmo(GizmoType.Active | GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
//    static void DrawVisionSensor(Animal.AnimalBase animal, GizmoType gizmoType)
//    {
//        VisionSensor sensor = animal.visionSensor;

//        if (sensor == null || !sensor.DrawSensorGizmo)
//        {
//            return;
//        }

//        Transform sensorCenter = (sensor.Head == null) ?
//            animal.transform : sensor.Head;

//        // find left and right bounds of field of view
//        float radianFOV = (sensor.FieldOfView / 2f) * Mathf.Deg2Rad;
//        Vector3 rightDir = sensorCenter.forward * Mathf.Cos(radianFOV) + sensorCenter.right * Mathf.Sin(radianFOV);
//        Vector3 leftDir = sensorCenter.forward * Mathf.Cos(radianFOV) - sensorCenter.right * Mathf.Sin(radianFOV);

//        // set rendering color
//        Gizmos.color = sensor.GizmoColor;
//        Handles.color = sensor.GizmoColor;

//        // render!
//        if (sensor.FieldOfView > 0 && sensor.FieldOfView < 360)
//        {
//            Gizmos.DrawRay(sensorCenter.position, leftDir.normalized * sensor.VisionRange);
//            Gizmos.DrawRay(sensorCenter.position, rightDir.normalized * sensor.VisionRange);
//        }
        
//        Handles.DrawWireArc(
//            sensorCenter.position, sensorCenter.up, leftDir.normalized, sensor.FieldOfView, sensor.VisionRange
//        );

//        bool drawVerticalArc = false;
//        if (drawVerticalArc)
//        {
//            Vector3 from = sensorCenter.forward * Mathf.Cos(radianFOV) + sensorCenter.up * Mathf.Sin(radianFOV);
//            Handles.DrawWireArc(
//                sensorCenter.position, sensorCenter.right, from, sensor.FieldOfView, sensor.VisionRange
//            );
//        }
//    }
//}

//[CustomEditor(typeof(VisionSensor))]
//public class VisionSensorEditorMenu : Editor
//{
//    [MenuItem("Custom/Sensors/Vision/Hide All Gizmos")]
//    public static void HideAllSensorGizmos()
//    {
//        Animal.AnimalBase[] animals = GameObject.FindObjectsOfType<Animal.AnimalBase>();
//        foreach (Animal.AnimalBase animal in animals)
//        {
//            if (animal.visionSensor != null)
//            {
//                animal.visionSensor.drawSensorGizmo = false;
//            }
//        }

//        SceneView.RepaintAll();
//    }

//    [MenuItem("Custom/Sensors/Vision/Show All Gizmos")]
//    public static void ShowAllSensorGizmos()
//    {
//        Animal.AnimalBase[] animals = GameObject.FindObjectsOfType<Animal.AnimalBase>();
//        foreach (Animal.AnimalBase animal in animals)
//        {
//            if (animal.visionSensor != null)
//            {
//                animal.visionSensor.drawSensorGizmo = true;
//            }
//        }

//        SceneView.RepaintAll();
//    }
//}
//#endif