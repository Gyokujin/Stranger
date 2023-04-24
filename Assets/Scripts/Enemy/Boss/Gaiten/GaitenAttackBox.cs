using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaitenAttackBox : MonoBehaviour
{
    private Vector2 offset = Vector2.zero;
    private Vector2 size = Vector2.zero;
    private BoxCollider2D collider;

    void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    // #. 정권 지르기
    private Vector2 pistAttackOffset = new Vector2(0.07676777f, -0.05129333f);
    private Vector2 pistAttackSize = new Vector2(0.8464645f, 0.3428841f);

    // #. 내려차기
    private Vector2 kickAttackOffset = new Vector2(-0.02388197f, -0.224397f);
    private Vector2 kickAttackSize = new Vector2(0.645165f, 1.388292f);

    // #. 공중 발차기
    private Vector2 jumpAttackOffset = new Vector2(-0.05973166f, -0.2525557f);
    private Vector2 jumpAttackSize = new Vector2(0.5734656f, 0.3109858f);

    // #. 앉아 발차기
    private Vector2 crouchAttackOffset = new Vector2(-0.06668133f, -0.3185238f);
    private Vector2 crouchAttackSize = new Vector2(0.8422756f, 0.3133334f);

    // #1. 공격종류에 따른 Collider 변경
    public void ChangeCollider(int attackType)
    {
        switch (attackType)
        {
            // #. 정권 지르기
            case 0:
                offset = pistAttackOffset;
                size = pistAttackSize;
                break;
            // #. 내려차기
            case 1:
                offset = kickAttackOffset;
                size = kickAttackSize;
                break;
            // #. 앉아 발차기
            case 2:
                offset = crouchAttackOffset;
                size = crouchAttackSize;
                break;
            // #. 공중 발차기
            case 3:
                offset = jumpAttackOffset;
                size = jumpAttackSize;
                break;
        }

        collider.offset = offset;
        collider.size = size;
    }
}