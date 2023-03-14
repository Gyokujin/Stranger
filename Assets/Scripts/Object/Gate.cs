using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : Object
{
    [SerializeField]
    private int targetScene;

    public void UseGate()
    {
        StartCoroutine(GameManager.instance.StageTransition(targetScene));
    }
}