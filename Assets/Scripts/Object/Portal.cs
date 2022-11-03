using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Object
{
    public GameObject targetPortal;

    public void Teleport(GameObject player)
    {
        player.transform.position = targetPortal.transform.position;
    }
}