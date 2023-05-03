using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : Object
{
    [SerializeField]
    private int targetScene;

    public void UseGate()
    {
        SpawnManager.instance.EnemyClear();
        StartCoroutine(GameManager.instance.StageTransition(targetScene));
    }
}