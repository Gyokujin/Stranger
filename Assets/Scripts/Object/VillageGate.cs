using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageGate : Object
{
    [SerializeField]
    private GameObject selectPanel;

    public void UseVillageGate()
    {
        selectPanel.SetActive(true);
    }

    public void StageSelect(int stage)
    {
        StartCoroutine(GameManager.instance.StageTransition(stage));
    }
}