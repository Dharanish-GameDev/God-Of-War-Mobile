public class Troll_WaitState : Troll_BaseState
{
    public override void Enter(Troll_Manager manager)
    {
        // play idle anim
        manager.Anim.SetInteger(manager.anim_State, 0);
    }
}
