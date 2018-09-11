using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapStitcher {

    GameObject[] _miscellaneousObjects;
    Vector2[] _points;

    public MapStitcher(MapController[] maps) {
        stitchMaps(maps);
    }

    public void stitchMaps(MapController[] m) {
        stitchMapPoints(m);
    }

    void stitchMapPoints(MapController[] m) {
        if (m.Length <= 0)
            return;

        Vector2[] pointsPivot = m[0]._collider.points;

        for (int i = 1; i < m.Length; i++)
        {
            pointsPivot = stitchColliders(pointsPivot, m[i]._collider.points);
        }

        _points = pointsPivot;
    }

    Vector2[] stitchColliders(Vector2[] p1, Vector2[] p2) {
        Vector2[] stitchedVec = stitchPosVectors(p1, p2);
        return stitchedVec;
    }

    Vector2[] stitchPosVectors(Vector2[] vec1, Vector2[] vec2) {
        if(vec1.Length <= 0) {
            return vec2;
        }

        if (vec2.Length <= 0)
        {
            return vec1;
        }

        Vector2 startPos = vec1[0];
        Vector2[] points = new Vector2[vec1.Length + vec2.Length - 1];

        int index = 0;
        foreach (Vector2 point in vec1)
        {
            points[index] = point-startPos;
            index++;
        }

        Vector2 pivot = vec1[index - 1];
        Vector2 pivot2 = vec2[0];
        for (int i = 1; i < vec2.Length; i++) {
            points[index] = vec2[i] + pivot - pivot2 - startPos;
            index++;
        }

        return points;
    }

    public GameObject[] getMiscellaneousObjects() {
        return _miscellaneousObjects;
    }
    public Vector2[] getPoints() {
        return _points;
    }
}
