public class Troll_KickState : Troll_BaseState
{
    public override void Enter(Troll_Manager manager)
    {
        // stop agent movement
        manager.Agent.speed = 0;

        // play kick animation
        manager.Anim.SetInteger(manager.anim_State, manager.KickSide);
    }

    public override void Exit(Troll_Manager manager)
    {
        // reset attack timer
        manager.ResetAttack();
    }
}
