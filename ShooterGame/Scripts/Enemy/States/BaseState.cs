public abstract class BaseState
{
    public ArmedEnemy enemy;
    public StateMachine stateMachine;
    public abstract void Enter();
    public abstract void Perform();
    public abstract void Exit();
}