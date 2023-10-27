using UnityEngine;

public class K_HealState : K_BaseState
{
    public override void Enter(K_Manager manager)
    {
        // cancel axe returning and shield
        manager.K_Axe.CancelAxeRecall();
        manager.K_Shield.CloseShield();
    }

    public override void Update(K_Manager manager)
    {
        // stop movement while collect healorb
        manager.StopMovement();
    }
}
