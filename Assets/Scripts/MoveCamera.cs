using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public static MoveCamera instance = null;

    [SerializeField]
    private Transform target;
    [SerializeField]
    private float speed;

    [SerializeField]
    private Vector2 center;
    [SerializeField]
    private Vector2 size;
    private float height;
    private float width;
    [SerializeField]
    private float offsetX;
    [SerializeField]
    private float offsetY;
    private float[] offsetVowelX = { 1, -4, -0.5f };
    private float[] offsetVowelY = { 0.5f, 1.5f, 0.5f };

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
                Destroy(this.gameObject);
        }
    }

    void Start()
    {
        height = Camera.main.orthographicSize;
        width = height * Screen.width / Screen.height;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, size);
    }

    void LateUpdate()
    {
        Vector3 pos = new Vector3(target.position.x + offsetX, target.position.y + offsetY, -10f);
        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * speed);

        float lx = size.x * 0.5f - width;
        float clampX = Mathf.Clamp(transform.position.x, -lx + center.x, lx + center.x);

        float ly = size.y * 0.5f - height;
        float clampY = Mathf.Clamp(transform.position.y, -ly + center.y, ly + center.y);

        transform.position = new Vector3(clampX, clampY, -10f);
    }

    public void SwitchOffset(int num)
    {
        offsetX = offsetVowelX[num];
        offsetY = offsetVowelY[num];
    }
}