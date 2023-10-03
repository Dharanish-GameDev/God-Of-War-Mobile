using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private BaseHealth k_health;
    [SerializeField] private K_Manager k_Manager;
    [SerializeField] private Troll_Manager troll_Manager;
    [SerializeField] private CameraCtrl camCtrl;

    // Properties
    public BaseHealth KratosHealth { get { return k_health; } }
    public K_Manager KratosManager { get { return k_Manager; } }
    public Troll_Manager TrollManager { get { return troll_Manager; } }
    public CameraCtrl CamCtrl { get { return camCtrl; } }

    private void Awake()
    {
        Instance = this;
    }
}
