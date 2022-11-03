using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    public enum ObjectType { TreasureBox, PortalRing } // 오브젝트 타입을 선택해서 Player의 코드에 영향을 준다

    public ObjectType objectType;
}