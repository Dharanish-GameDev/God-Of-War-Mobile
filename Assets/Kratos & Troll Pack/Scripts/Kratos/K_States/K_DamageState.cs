using UnityEngine;

public class K_DamageState : K_BaseState
{
    public override void Enter(K_Manager manager)
    {
        LevelManager.Instance.CamCtrl.SetCameraZDamping(0.0f);
        LevelManager.Instance.CamCtrl.SetCameraFollowDistance(4f);
        Debug.Log("Entered");
    }

    public override void Update(K_Manager manager)
    {
        LevelManager.Instance.CamCtrl.SetCanRotate(false);
        //manager.cameraCtrl.RotateTowardsPoint(manager.Troll.transform);
        manager.Rb.position = manager.DamageMovePos;
    }

    public override void Exit(K_Manager manager)
    {
        Debug.Log("Exited");
        LevelManager.Instance.CamCtrl.SetCameraZDamping(0.4f);
        LevelManager.Instance.CamCtrl.SetCameraFollowDistance(LevelManager.Instance.CamCtrl.DefaultFollowDistance);
        LevelManager.Instance.CamCtrl.SetCanRotate(true);
    }
}
