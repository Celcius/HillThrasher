using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawMinimap : MonoBehaviour {

    MapController _mainMap;
    Transform _player;
    // Use this for initialization
    LineRenderer lineRenderer;
    Vector2[] _points;
    Vector2[] prevPoints;

    [SerializeField]
    float _scale = 0.5f;
    [SerializeField]
    Vector2 _offset;
    Vector2 startPos;
    Vector2 startOffset;
    [SerializeField]
    float thickness;
    float scale = 0;

    [SerializeField]
    float mapRadius = 20;

    Vector3 prevPos = Vector3.zero;
    DrawPlayerMinimap _pMap;

    void setupRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = thickness;
        lineRenderer.endWidth = thickness;
        _pMap = GetComponentInChildren<DrawPlayerMinimap>();
    }
	// Update is called once per frame
	public void UpdateMap (Vector3 p) {
        drawMap(p);
        drawPlayer(p);
        
	}

    public void setMap(MapController c, Transform p)
    {
        _mainMap = c;
        _player = p;
    }

    void drawMap(Vector3 p)
    {
   
        if (_mainMap == null)
            return;

        if (lineRenderer == null)
        {
            Vector2[] totalPoints = _mainMap.getPoints();
            scale = (Camera.main.orthographicSize * 2) / (totalPoints[totalPoints.Length - 1] - totalPoints[0]).magnitude;
            scale *= _scale;
            startPos = totalPoints[0] * scale;
            setupRenderer();
        }

        Vector2 posOffset = new Vector2(_offset.x, _offset.y);
  
            Vector2[] points = _mainMap.getPoints(Camera.main.transform.position.x, mapRadius*0.33f, mapRadius*0.66f);

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = points[i] * scale;
            }

        startOffset = startPos- points[0];

        lineRenderer.positionCount = points.Length;
        int index = 0;
        foreach (Vector2 v in points)
        {
            lineRenderer.SetPosition(index, new Vector3(v.x + posOffset.x + startOffset.x, v.y + posOffset.y + startOffset.y, transform.position.z));
            index++;
        }
    }

    void drawPlayer(Vector3 p)
    {
        if (_player == null || _pMap == null || _mainMap == null)
            return;

        /*Vector3 curPoint = transform.position * scale + new Vector3(_offset.x, _offset.y) + new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y);

        if (prevPos == Vector3.zero)
        {
            prevPos = curPoint;
        }

        Vector3 pos = prevPos + (curPoint - prevPos) * Time.deltaTime*50;*/

        //float x = (Camera.main.transform.position.x - _points[0].x) / _points[_points.Length - 1].x *;

        //float x = (Camera.main.transform.position.x - _points[0].x) / (_points[_points.Length - 1].x - _points[0].x) * (_points[_points.Length - 1].x - _points[0].x);

        Vector3 playerPos = p * scale; //new Vector3(p.x, p.y, 0) * scale;//Camera.main.transform.position.x, Camera.main.transform.position.y, 0) * scale;

        Vector3 pos = playerPos + new Vector3(_offset.x+ startOffset.x, _offset.y+ startOffset.y, transform.position.z-0.1f);

        _pMap.drawPlayer(pos);
    }

}
