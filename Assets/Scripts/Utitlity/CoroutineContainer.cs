using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineContainer : MonoBehaviour
{
    #region Public
    public delegate bool ConditionPredicate();
    #endregion

    #region Private
    private static bool Initialized = false;
    private static CoroutineContainer Instance;
    private static GameObject Container;
    #endregion

    //-----------------------------------------------------------

    static void RequestInstanceCreation()
    {
        if (Initialized) { return; }
        Initialized = true;

        Container = new GameObject("CoroutineContainer");
        Instance = Container.AddComponent<CoroutineContainer>();
    }

    #region Generic Coroutine Helpers
    public static Coroutine StartACoroutine(ConditionPredicate ConditionToSatisfy, System.Action ActionUntilCondtionSatisfied, System.Action OnSatisfied)
    {
        RequestInstanceCreation();

        return Instance.StartCoroutine(GenericRoutine(ConditionToSatisfy, ActionUntilCondtionSatisfied, OnSatisfied));
    }

    static IEnumerator GenericRoutine(ConditionPredicate ConditionToSatisfy, System.Action ActionUntilCondtionSatisfied, System.Action OnSatisfied)
    {
        while (!ConditionToSatisfy())
        {
            if (ActionUntilCondtionSatisfied != null) { ActionUntilCondtionSatisfied(); }
            yield return null;
        }

        if (OnSatisfied != null) { OnSatisfied(); }
    }
    #endregion

    #region Stopping Helpers
    public static void StopACoroutine(Coroutine Routine)
    {
        if (!Initialized) { return; }

        if (Routine != null) { Instance.StopCoroutine(Routine); }
    }

    public static void StopAllMyCoroutines()
    {
        if (Instance != null) { Instance.StopAllCoroutines(); }
    }
    #endregion
}
