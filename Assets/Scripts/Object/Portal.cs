using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Object
{
    public GameObject targetPortal;

    public void Teleport(GameObject target)
    {
        target.transform.position = targetPortal.transform.position;
    }
}