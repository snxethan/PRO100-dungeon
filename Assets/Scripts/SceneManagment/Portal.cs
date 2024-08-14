using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : IPlayerTriggerable
{
    public void OnPlyaerTriggered(PlayerController player)
    {
        Debug.Log("Player entered portal");
    }
}
