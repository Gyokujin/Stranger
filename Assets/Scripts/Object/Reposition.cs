using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    [HideInInspector]
    public GameObject[] backgrounds;

    public void ReAssign(int num, Vector2 pos)
    {
        int assignDir = (int)Player.instance.isRight;
        int sequence = 0;

        if (assignDir > 0)
        {
            switch (num)
            {
                case 0:
                    sequence = 2;
                    break;
                case 1:
                    sequence = 3;
                    break;
                case 2:
                    sequence = 4;
                    break;
                case 3:
                    sequence = 0;
                    break;
                case 4:
                    sequence = 1;
                    break;
            }

            GameObject targetObject = backgrounds[sequence];
            targetObject.transform.position = new Vector2(targetObject.transform.position.x + 60, targetObject.transform.position.y);
        }
        else if (assignDir < 0)
        {
            switch (num)
            {
                case 0:
                    sequence = 3;
                    break;
                case 1:
                    sequence = 4;
                    break;
                case 2:
                    sequence = 0;
                    break;
                case 3:
                    sequence = 1;
                    break;
                case 4:
                    sequence = 2;
                    break;
            }

            GameObject targetObject = backgrounds[sequence];
            targetObject.transform.position = new Vector2(targetObject.transform.position.x - 60, targetObject.transform.position.y);
        }
    }
}