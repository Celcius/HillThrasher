using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPlayerMinimap : MonoBehaviour {

    Transform _player;

    [SerializeField]
    public int segments;
    [SerializeField]
    public float xradius;
    [SerializeField]
    public float yradius;

    [SerializeField]
    Vector2 offset;
    LineRenderer line;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();

        line.positionCount = segments + 1;
    }


    public void drawPlayer(Vector3 pos)
    {
        float x;
        float y;
        float z = pos.z;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius + pos.x + offset.x;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius + +pos.y + offset.y;

            line.SetPosition(i, new Vector3(x, y, z));

            angle += (360f / segments);
        }

    }
}
