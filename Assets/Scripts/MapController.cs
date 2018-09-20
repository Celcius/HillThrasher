using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comp : IComparer
{ 
    public int Compare(object x, object y)
    {
        Vector2 v1 = (Vector2)x;
        Vector2 v2 = (Vector2)y;

        return v1.x >= v2.x  ? 1 : -1;
    }
}

public class MapController : MonoBehaviour {

    ArrayList _points;
    [SerializeField]
    public EdgeCollider2D _collider;

    [SerializeField]
    public Transform _startPos;

    [SerializeField]
    public Transform _finishLine;

    [SerializeField]
    Transform additionalMapsParent;

    [SerializeField]
    Transform obstaclesParent;

    int indexToStartSearching = 0;


    // Use this for initialization
    void Start () {
        getCollider();
        updatePoints();
    }

    void getCollider() {
        if (_collider != null)
            return;
        if (_collider == null)
        {
            if (_collider != null)
                return;

            _collider = GetComponent<EdgeCollider2D>();
            if (_collider == null)
                _collider = GetComponentInChildren<EdgeCollider2D>();
            if (_collider == null)
                return;
            _points = new ArrayList();
        }
    }

    void updatePoints()
    {
        getCollider();
        if (_collider == null)
            return;
        
        _points = new ArrayList();

        int index = 0;
        foreach (Vector2 point in _collider.points)
        {
            Vector3 p = _collider.transform.TransformPoint(point);
            _points.Add(new Vector2(p.x, p.y));
            index++;


        }
        _points.Sort(new Comp());
    }

    public Vector3 mapPoint(Vector3 p)
    {
        return transform.InverseTransformPoint(p.x, p.y, 0);
    }
    public Vector2[] getPoints(float xpos, float leftRadius, float rightRadius)
    {
        if(xpos- leftRadius < ((Vector2)_points[0]).x)
        {
            xpos = ((Vector2)_points[0]).x + leftRadius;
        }
        else if (xpos + rightRadius > ((Vector2)_points[_points.Count-1]).x)
        {
            xpos = ((Vector2)_points[_points.Count - 1]).x - rightRadius;
        }

        int startIndex = 0;
        int endIndex = 0;

        for (; startIndex < _points.Count - 2 && ((Vector2)_points[startIndex]).x < xpos - leftRadius; startIndex++);
        for (endIndex = startIndex + 1; endIndex < _points.Count - 1 && ((Vector2)_points[endIndex]).x < xpos + rightRadius; endIndex++);



        Vector2[] points = new Vector2[endIndex - startIndex +3];
        for (int i = startIndex, j = 1; i <= endIndex; i++, j++)
        {
            points[j] = (Vector2)_points[i];
        }

        Vector2 start = points[1];
        float missingStartMagnitude =start.x - (xpos - leftRadius);
        points[0] = start + ((Vector2)_points[(int)Mathf.Max(startIndex - 1, 0)] - start).normalized * missingStartMagnitude;

        Vector2 end = points[points.Length-2];
        float missingEndtMagnitude = Mathf.Abs((xpos + rightRadius) - end.x);
        points[points.Length-1] = end + ((Vector2)_points[(int)Mathf.Min(endIndex + 1, _points.Count-1)] - end).normalized * missingEndtMagnitude;

        return getPoints(points);
    }

    public Vector2[] getPoints()
    {
        updatePoints();
        Vector2[] points = new Vector2[_points.Count];
        int i = 0;
        foreach (Vector2 p in _points)
        {
            points[i] = p;
            i++;
        }

        return getPoints(points);
    }

    public Vector2[] getPoints(Vector2[] pRange)
    {

        Vector2[] points = new Vector2[pRange.Length];
        int i = 0;
        foreach(Vector2 p in pRange)
        {
            Vector3 p3 = transform.InverseTransformPoint(p.x, p.y, 0);
            Vector2 p2 = new Vector2(p3.x, p3.y);
            points[i] = p2;
            i++;
        }

        return points;
    }

    public Vector2 getInterpolatedForward(float x)
    {
        int i2 = getRightIndex(x);

        if (indexToStartSearching == i2 || i2 == _points.Count)
            return Vector2.right; // Cant decide

        Vector2 v1 = (Vector2)_points[indexToStartSearching];
        Vector2 v2 = (Vector2)_points[i2];
        return v2 - v1;
    }

    public float getYForX(float x)
    {
        int i2 = getRightIndex(x);

        if (indexToStartSearching == i2 || i2 == _points.Count)
            return ((Vector2)_points[indexToStartSearching]).y;

        Vector2 v1 = (Vector2)_points[indexToStartSearching];
        Vector2 v2 = (Vector2)_points[i2];

        float y = v1.y + ((v2.y - v1.y) / (v2.x - v1.x)) * (x - v1.x);
        return y;

    }

    public bool leftMap(float x)
    {
        int i2 = getRightIndex(x);
        return i2 == _points.Count;
    }

    public float getRightY(float x)
    {
        int i2 = getRightIndex(x);

        if (indexToStartSearching == i2 || i2 == _points.Count)
            return ((Vector2)_points[indexToStartSearching]).y;

        return ((Vector2)_points[i2]).y;
    }

    public int getRightIndex(float x)
    { 
        if(_points == null) {
            updatePoints();
        }
        int i2 = Mathf.Min(indexToStartSearching + 1, _points.Count - 1);
        
        for (; i2 < _points.Count; i2++)
        {
            Vector2 v = (Vector2)_points[i2];
            if (v.x > x)
                break;
        }
       
        int i1 = Mathf.Max(i2 - 1, 0);
        indexToStartSearching = i1;
        return i2;
    }

    public bool badCollision(Vector2 right, float x, float comparisonAngle)
    {
        Vector2 comparison = getInterpolatedForward(x);

        return Mathf.Abs(Vector2.Angle(right, comparison)) > comparisonAngle;
 
    }
    // Update is called once per frame
    void Update () {
		
	}

    public void setMapLine(Vector2[] points) {
        if (_collider == null)
            return;
        
        _collider.points = points;
        updateLevelPositions();
    }

    void updateLevelPositions() {
        if (_collider == null || _collider.points.Length <= 0)
            return;
        
        _startPos.position = _collider.points[0];
        _finishLine.position = _collider.points[_collider.points.Length-1];
    }

    public Transform[] getAdditionalChildren() {
        
        Transform[] children1 = additionalMapsParent.GetComponentsInChildren<Transform>();
        Transform[] children2 = obstaclesParent.GetComponentsInChildren<Transform>();
        List<Transform> list = new List<Transform>();

        foreach(Transform t in children1) {
            if (t.parent == additionalMapsParent)
                list.Add(t);
        }
        foreach (Transform t in children2)
        {
            if (t.parent == obstaclesParent)
                list.Add(t);
        }

        return list.ToArray();
    }
}
