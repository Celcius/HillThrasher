using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapStitcher {
    
    Transform _miscellaneousObjects;

    Vector2[] _points;

    Transform _parent;

    public MapStitcher(LevelDefinition level, Transform parent) {

        MapController[] maps = generateMap(level);
        _miscellaneousObjects = new GameObject().GetComponent<Transform>();
        _miscellaneousObjects.parent = parent;
        _miscellaneousObjects.position = Vector3.zero;
        stitchMaps(maps);
    }

    public MapController[] generateMap(LevelDefinition level) {

        Debug.Log("Generating Map for definition " + level);
        List<MapController> maps = new List<MapController>();
        int i = 0;
        foreach(LevelDefinitionSection section in level.sections) {
            Debug.Log("=====================");
            Debug.Log("Generating section " + i);
            MapController[] mapSections = generateMapsFromSection(section);
            foreach(MapController map in mapSections) {
                maps.Add(map);
            }
            Debug.Log("=====================");
            i++;
        }

        return maps.ToArray();
    }

    public MapController[] generateMapsFromSection(LevelDefinitionSection section)
    {
        if (section.maps.Length == 0 ||
           section.weights.Length == 0 ||
            section.maps.Length != section.weights.Length ||
           section.maxLength < section.minLength ||
           section.maxLength == 0)
        {
            return new MapController[0];
        }
        int sectionLength = Random.Range(section.minLength, section.maxLength);
        Debug.Log("Length: " + sectionLength);

        MapController[] maps = new MapController[sectionLength];

        int[] computedWeights = new int[section.weights.Length];
        int totalWeight = 0;
        foreach (int weight in section.weights) {
            totalWeight += weight;
        }
        for (int i = 0; i < computedWeights.Length; i++) {
            computedWeights[i] = (int)(((float)section.weights[i] / (float)totalWeight) * 100.0f);
            if (i > 0)
                computedWeights[i] = computedWeights[i] + computedWeights[i - 1];
        }

        for (int i = 0; i < sectionLength; i++) {
            int random = Random.Range(0, 100);
            int index = indexForValue(random, computedWeights);
            maps[i] = section.maps[index];

            Debug.Log("[" + i + "] " + maps[i]);
        }

        return maps;
    }

    private int indexForValue(int value, int[] weights) {
        value = Mathf.Clamp(value, weights[0], weights[weights.Length - 1]);
        for (int i = 0; i < weights.Length; i++) {
            if(weights[i] >= value) {
                return i;
            }
        }
        return weights.Length - 1; // Defaults to lsat element
    }

    public void stitchMaps(MapController[] m) {
        stitchMapPoints(m);
    }

    void stitchMapPoints(MapController[] m) {
        if (m.Length <= 0)
            return;

        //_miscellaneousObjects.position = m[0]._collider.points[0];
        Vector2[] pointsPivot = m[0]._collider.points;
        Vector2 startPivot = - m[0]._collider.points[0];

        for (int i = 0; i < m.Length; i++)
        {
            addObjectCopiesWithRelativePosition(startPivot, m[i]);
            if (i < m.Length - 1)
            {
                startPivot += pointsPivot[pointsPivot.Length - 1] - m[i + 1]._collider.points[0];
            }

            if (i == 0) {
                continue;
            }
            
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

    void addObjectCopiesWithRelativePosition(Vector2 relativeStart, MapController m) {

        Transform[] children = m.getAdditionalChildren();

        foreach (Transform t in children)
        {
            Transform copyT = GameObject.Instantiate<Transform>(t, _miscellaneousObjects) as Transform;
         //   copyT.localPosition = t.localPosition;
            copyT.position += new Vector3(relativeStart.x, relativeStart.y, 0);
        }

    }

    public Vector2[] getPoints() {
        return _points;
    }
}
