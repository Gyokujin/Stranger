using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBox : Object
{
    public GameObject spawnObject;

    public void Spawn()
    {
        spawnObject.gameObject.SetActive(true);
    }
}