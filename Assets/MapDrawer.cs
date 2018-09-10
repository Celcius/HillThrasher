using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDrawer : MonoBehaviour {

    [SerializeField]
    float _topPercentage = 0.3f;

    [SerializeField]
    float _height = 50;

    [SerializeField]
    float _zPosition = 0;

    [SerializeField]
    MeshFilter _topMesh;
    [SerializeField]
    MeshFilter _botMesh;

    // Use this for initialization
    void Start () {
        MapController controller = GetComponent<MapController>();
        EdgeCollider2D collider = GetComponent<EdgeCollider2D>();

        Vector2[] line;
        if(controller != null)
        {
            line = controller.getPoints();
        }
        else
        {
            line = new Vector2[collider.points.Length];
            int i = 0;
            foreach (Vector2 point in collider.points)
            {
                Vector3 p = collider.transform.TransformPoint(point);
                line[i] = new Vector2(p.x, p.y);
                i++;
            }
        }


        Vector3[][] vertices = getQuadPointsFromLinePoints(line, _height, _zPosition);

        _topMesh.mesh = createMesh(vertices[0]);
        _botMesh.mesh = createMesh(vertices[1]);

    }

    public Vector3[][] getQuadPointsFromLinePoints(Vector2[] points,float height, float zPos)
    {
        Vector3[] retPointsTop = new Vector3[points.Length * 4-2];
        Vector3[] retPointsBot = new Vector3[points.Length * 4 - 2];


        for (int i = 0; i < points.Length -1; i++)
        {
            int pivotIndex = i * 4;
            Vector3 iVec = new Vector3(points[i].x, points[i].y, zPos);
            Vector3 jVec = new Vector3(points[i+1].x, points[i+1].y, zPos);

            retPointsTop[pivotIndex] = iVec + Vector3.down * height * _topPercentage;
            retPointsTop[pivotIndex + 1] = iVec;
            retPointsTop[pivotIndex + 2] = jVec + Vector3.down * height * _topPercentage;
            retPointsTop[pivotIndex + 3] = jVec;

            retPointsBot[pivotIndex] = retPointsTop[pivotIndex] + Vector3.down * height * (1-_topPercentage);
            retPointsBot[pivotIndex + 1] = iVec;
            retPointsBot[pivotIndex + 2] = retPointsTop[pivotIndex + 2] + Vector3.down * height *(1 - _topPercentage);
            retPointsBot[pivotIndex + 3] = jVec;
            for (int j = pivotIndex; j <= pivotIndex + 4; j++)
                retPointsBot[j].z += 0.1f;
        }
        Vector3[][] ret = { retPointsTop, retPointsBot };

        return ret;


    }

    // Update is called once per frame
    void Update () {
		
	}

    Mesh createMesh(Vector3[] vertices)
    {
 
        Mesh mesh = new Mesh();

        mesh.vertices = vertices;

        int triangleCount = (int)(vertices.Length / 4.0f)*2;
        int squareCount = (int)(triangleCount / 2.0f);
        int[] tri = new int[3 * triangleCount];
        for(int triIndex = 0; triIndex < squareCount; triIndex++)
        {
            int step = triIndex * 6;
            int startIndex = triIndex * 4;
             tri[step] = startIndex;
             tri[step + 1] = startIndex + 1;
             tri[step + 2] = startIndex + 2;
             tri[step + 3] = startIndex + 1;
             tri[step + 4] = startIndex + 3;
             tri[step + 5] = startIndex + 2;
        }
        mesh.subMeshCount = triangleCount;
        mesh.triangles = tri;


        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < normals.Length;i++)
        normals[i] = -Vector3.forward;

        mesh.normals = normals;

        float textureOffset = 0f;
        float tilingFactor = 10; // How long must be the texture?

        Vector2[] uv = new Vector2[vertices.Length];
        
        for (int triIndex = 0; triIndex < squareCount; triIndex++)
        {
            int vertexIndex = triIndex * 4;
            Vector3 finalPoint = vertices[vertexIndex];
            Vector3 startPoint = vertices[vertexIndex+2];

            // This is placed inside the extrusion loop
            float magnitude = (finalPoint - startPoint).magnitude;
            float tiling = (magnitude / tilingFactor) + textureOffset;

        uv[vertexIndex + 0] = new Vector2(textureOffset, 0);
        uv[vertexIndex + 1] = new Vector2(textureOffset, 1);

        uv[vertexIndex + 2] = new Vector2(tiling, 0);
        uv[vertexIndex + 3] = new Vector2(tiling, 1);

        textureOffset = (magnitude / tilingFactor) % 1 + textureOffset;
        }
        mesh.uv = uv;

        return mesh;
    }
}
