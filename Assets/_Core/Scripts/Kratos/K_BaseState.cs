public abstract class K_BaseState
{
    public virtual void Enter(K_Manager manager) { }
    public virtual void Update(K_Manager manager) { }
    public virtual void Exit(K_Manager manager) { }
    public virtual void FixedUpdate(K_Manager manager) { }
}
