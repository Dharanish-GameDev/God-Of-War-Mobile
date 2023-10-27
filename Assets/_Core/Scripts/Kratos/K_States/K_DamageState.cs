using UnityEngine;

public class K_DamageState : K_BaseState
{
    public override void Enter(K_Manager manager)
    {
        manager.K_Axe.CancelAxeRecall();
        manager.K_Axe.StopAiming();
        manager.K_Shield.CloseShield();

        LevelManager.Instance.CamCtrl.SetCameraZDamping(0.0f);
        LevelManager.Instance.CamCtrl.SetCameraFollowDistance(4f);
    }

    public override void Update(K_Manager manager)
    {
        LevelManager.Instance.CamCtrl.SetCanRotate(false);
        //LevelManager.Instance.CamCtrl.RotateTowardsPoint(LevelManager.Instance.TrollManager.transform);
    }

    public override void FixedUpdate(K_Manager manager)
    {
        //manager.Rb.position = manager.DamageMovePos;
        manager.Rb.MovePosition(manager.DamageMovePos);
    }

    public override void Exit(K_Manager manager)
    {
        LevelManager.Instance.CamCtrl.SetCameraZDamping(0.4f);
        LevelManager.Instance.CamCtrl.SetCameraFollowDistance(LevelManager.Instance.CamCtrl.DefaultFollowDistance);
        LevelManager.Instance.CamCtrl.SetCanRotate(true);
    }
}
