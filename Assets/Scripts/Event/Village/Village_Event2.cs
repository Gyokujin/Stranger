using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village_Event2 : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTarget;

    void Start()
    {
        MoveCamera.instance.target = cameraTarget;
        StartCoroutine("Cut1");
    }

    IEnumerator Cut1()
    {
        yield return new WaitForSeconds(3f);

        while (cameraTarget.position.x < 90.75f)
        {
            cameraTarget.Translate(new Vector2(5.5f, 0) * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(4f);
        MoveCamera.instance.target = Player.instance.transform;
        StartCoroutine(GameManager.instance.StageTransition(8));
    }
}