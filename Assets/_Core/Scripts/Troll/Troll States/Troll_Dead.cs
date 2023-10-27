using UnityEngine;

public class Troll_Dead : Troll_BaseState
{
    public override void Enter(Troll_Manager manager)
    {
        // stop agent movement
        manager.Agent.speed = 0;

        // play idle anim
        manager.Anim.SetBool(manager.anim_IsDead, true);
    }
}
