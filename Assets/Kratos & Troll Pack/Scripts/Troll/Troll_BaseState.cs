using UnityEngine;

public abstract class Troll_BaseState
{
    public virtual void Enter(Troll_Manager manager) { }
    public virtual void Update(Troll_Manager manager) { }
    public virtual void Exit(Troll_Manager manager) { }
    public virtual void FixedUpdate(Troll_Manager manager) { }
}
