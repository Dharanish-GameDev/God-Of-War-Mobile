using UnityEngine;

public class Troll_LAttackState : Troll_BaseState
{
    private Vector3 dir;
    private Quaternion rot;
    private bool isStartAttack;

    public override void Enter(Troll_Manager manager)
    {
        //manager.TrollStone.onPlayerHit += GiveDamage;

        // stop agent movement
        manager.Agent.speed = 0;

        // play attack animation
        isStartAttack = false;
        manager.Anim.SetInteger(manager.anim_State, 5);
    }

    //private void GiveDamage(object sender, System.EventArgs e)
    //{
    //    LevelManager.Instance.KratosHealth.GiveDamage(30);
    //}

    public override void Update(Troll_Manager manager)
    {
        // rotate towards player before attacking
        if (!isStartAttack)
        {
            dir = LevelManager.Instance.KratosManager.transform.position - manager.transform.position;
            dir.Normalize();
            rot = Quaternion.Slerp(manager.transform.rotation, Quaternion.LookRotation(dir) * Quaternion.Euler(0, -90, 0), 0.25f);
            manager.transform.rotation = new Quaternion(0, rot.y, 0, rot.w);

            // after rotate towards player then start attack
            if (Vector3.Dot(manager.transform.right, dir) >= 0.98f)
                isStartAttack = true;

            return;
        }

        // handle rotation towards the player while attacking
        HandleRotation(manager);
    }

    public override void Exit(Troll_Manager manager)
    {
        //manager.TrollStone.onPlayerHit -= GiveDamage;
        // reset attack timer
        manager.ResetAttack();
    }

    // Private Methods
    private void HandleRotation(Troll_Manager manager)
    {
        // check current anim is lattack
        if (!manager.Anim.GetCurrentAnimatorStateInfo(0).IsTag("LAttack")) return;

        // face towards player while attacking within the anim range
        if (manager.GetAnimationPercent() >= 0.3f && manager.GetAnimationPercent() < 0.375f)
            manager.RotateTowardsPosition(LevelManager.Instance.KratosManager.transform.position, 0.75f, 1);
    }
}
