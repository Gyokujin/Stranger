using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackBox : MonoBehaviour
{
    private Player player;

    void Awake()
    {
        player.GetComponentInParent<Player>();
    }

    public IEnumerator OnHitBox()
    {
        gameObject.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
    }
}