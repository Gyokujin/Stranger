using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum ObjectType { Zombie, EliteZombie, BurningGhoul }

    [Header("Status")]
    public float atk;
    public float hp;
    public float frontSight;
    public float atkTakeTime;
    public float atkDelay;

    [Header("Action")]
    public float sightDir;
    public bool onDetect;
    public bool onAttack;
    public bool onDamage;
    public bool onDie;

    protected virtual void Start()
    {
        sightDir = transform.localScale.x > 0 ? -1 : 1;
    }

    // #. 플레이어 감지
    public void Detect()
    {
        if (!onAttack && !onDamage && !onDie)
        {
            Debug.DrawRay(transform.position, Vector2.left * (-sightDir) * frontSight, Color.red);
            RaycastHit2D frontCheck = Physics2D.Raycast(transform.position, Vector2.left * (-sightDir), frontSight, LayerMask.GetMask("Player"));

            if (frontCheck.collider != null)
                onDetect = true;
            else
                onDetect = false;
        }
    }
}