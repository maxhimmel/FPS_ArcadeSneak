using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNothingState : IFSMState
{
    public virtual void Initialize(FSM Fsm) { }
    public virtual void Enter(FSM Fsm, int PrevState, object Context = null) { }
    public virtual void Update(FSM Fsm) { }
    public virtual void LateUpdate(FSM Fsm) { }
    public virtual void Exit(FSM Fsm, int NextState) { }
    public virtual void Shutdown(FSM Fsm) { }
}
