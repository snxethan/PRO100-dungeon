using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayer : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask portalLayer;
    [SerializeField] LayerMask enemyLayer;

    public static GameLayer i { get; set; }

    private void Awake()
    {
        i = this;
    }

    public LayerMask SolidObjectsLayer 
    { 
        get => solidObjectsLayer; 
    }

    public LayerMask PlayerLayer 
    { 
        get => playerLayer; 
    }

    public LayerMask PortalLayer 
    { 
        get => portalLayer; 
    }

    public LayerMask EnemyLayer 
    { 
        get => enemyLayer; 
    }

    public LayerMask TriggerableLayers 
    { 
        get => portalLayer; 
    }
}
