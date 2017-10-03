public interface IFSMState
{
    void Initialize(FSM Fsm);
    void Enter(FSM Fsm, int PrevState, object Context = null);
    void Update(FSM Fsm);
    void LateUpdate(FSM Fsm);
    void Exit(FSM Fsm, int NextState);
    void Shutdown(FSM Fsm);
}