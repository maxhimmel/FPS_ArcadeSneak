//====================================================================================================
// Date:        01/25/2017
// Description: Generic Finite State Machine used for GameState/Animation/AI/etc
//====================================================================================================
using System;
using System.Collections.Generic;
using UnityEngine;

public class FSM
{
    #region Properties
    public bool Enable          { get; set; }
    public int QueuedStateID    { get { return GetStateID(QueuedState); } }
    public int CurrentStateID   { get { return GetStateID(CurrentState); } }
    public int PreviousStateID  { get { return GetStateID(PreviousState); } }
    #endregion

    #region Private
    private Dictionary<int, IFSMState> RegisteredStates;
    private IFSMState QueuedState;
    private IFSMState CurrentState;
    private IFSMState PreviousState;
    private object Context  = null;
    private object Owner    = null;
    private bool ForceStateChange   = false;
    private bool WasShutdown        = false;
    #endregion

    //------------------------------------------------------------------------------------------------------

    #region Constructors
    public FSM(object Owner)
    {
        Enable = true;
        this.Owner = Owner;
        QueuedState = null;
        CurrentState = null;
        PreviousState = null;
        RegisteredStates = new Dictionary<int, IFSMState>();
    }
    #endregion

    #region Updaters
    public void Update()
    {
        if (!Enable || WasShutdown) { return; }

        // Do transition to another state
        bool IsQueueStateDifferentThanCurrentState = (QueuedState != CurrentState);
        if (QueuedState != null && (IsQueueStateDifferentThanCurrentState || ForceStateChange))
        {
            if (CurrentState != null && QueuedState != null) { CurrentState.Exit(this, QueuedStateID); }

            PreviousState = CurrentState;   // Store prev state ...
            CurrentState = QueuedState;     // Set to next state ...
            QueuedState = null;             // Remove queued state ...

            CurrentState.Enter(this, PreviousStateID, Context);
        }
        // Queued state is current state ...
        else if (QueuedState == CurrentState) { QueuedState = null; }

        // Update current state ...
        if (CurrentState != null) { CurrentState.Update(this); }
    }

    public void LateUpdate()
    {
        if (!Enable || WasShutdown) { return; }

        if (CurrentState != null) { CurrentState.LateUpdate(this); }
    }
    #endregion

    #region Shutdown
    public virtual void Shutdown()
    {
        WasShutdown = true;
        Enable = false;

        if (RegisteredStates == null || RegisteredStates.Count <= 0) { return; }

        if (CurrentState != null) { CurrentState.Exit(this, -1); }

        foreach(KeyValuePair<int, IFSMState> KVP in RegisteredStates)
        {
            IFSMState ThisState = GetStateByID(KVP.Key);
            if (ThisState == null) { continue; }

            ThisState.Shutdown(this);
        }

        RegisteredStates.Clear();
        RegisteredStates = null;
    }
    #endregion

    #region Queue States
    public void QueueState(int Id)
    {
        QueueState(Id, false);
    }

    public void QueueState<T>(T Id)
    {
        QueueState((int)(object) Id, false);
    }

    public void QueueState<T>(T Id, object Context)
    {
        QueueState((int)(object) Id, Context, false);
    }

    public void QueueState<T>(T Id, object Context, bool ForceStateChange)
    {
        QueueState((int)(object) Id, Context, ForceStateChange);
    }
    
    public void QueueState(int Id, System.Object Context, bool ForceStateChange)
    {
        if (WasShutdown) { return; }

        this.Context = Context;
        this.ForceStateChange = ForceStateChange;
        this.QueuedState = RegisteredStates[Id];
    }
    #endregion

    #region Get States    
    public int GetQueuedState()
    {
        return  GetStateID(QueuedState);
    }
    
    public int GetCurrentState()
    {
        return GetStateID(CurrentState);
    }
    
    public IFSMState GetQueuedStateInstance()
    {
        return QueuedState;
    }
    
    public IFSMState GetCurrentStateInstance()
    {
        return CurrentState;
    }
    
    public int GetStateID(IFSMState StateToFind)
    {
        if (WasShutdown)
        {
            Debug.Log("Unable to get state " + StateToFind + " on a machine that was previously shut down.");
            return -1;
        }

        if (RegisteredStates == null) { return -1; }

        foreach (KeyValuePair<int, IFSMState> Pair in RegisteredStates)
        {
            if (Pair.Value == StateToFind) { return Pair.Key; }
        }
    
        return -1;
    }
    
    public IFSMState GetStateByID(int Id)
    {
        if (WasShutdown) { return null; }

        IFSMState Result = null;
        if (RegisteredStates != null) { RegisteredStates.TryGetValue(Id, out Result); }

        return Result;
    }
    #endregion

    #region Register State
    public void RegisterState<T>(T Id, IFSMState State)
    {
        RegisterState((int)(object) Id, State);
    }

    public void RegisterState(int Id, IFSMState State)
    {
        if (State == null || WasShutdown) { return; }
    
        RegisteredStates[Id] = State;
        State.Initialize(this);
    }
    #endregion

    #region Other Helper
    public bool IsCurrentOrQueuedState<T>(T Id)
    {
        return IsCurrentOrQueuedState((int)(object) Id);
    }
    
    public bool IsCurrentOrQueuedState(int Id)
    {
        if (GetCurrentState() == Id || GetQueuedState() == Id) { return true; }

        return false;
    }
    
    public object GetOwnerObject()
    {
        return Owner;
    }

    public string CurrentStateName()
    {
        if (CurrentState == null) { return null; }

        return CurrentState.GetType().FullName;
    }
    #endregion
}
