using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInstantiator : MonoBehaviour
{
    [SerializeField]
    MapDrawer drawerPrefab;

    [SerializeField]
    MapController mainMapPrefab;


    [SerializeField]
    MapController[] objects;
	// Use this for initialization
	void Start () {
        
        CreateLevel();


        Destroy(transform.gameObject);
	}

    void CreateLevel() {
        MapController mainMap = GameObject.Instantiate<MapController>(mainMapPrefab,transform.parent) as MapController;

        MapStitcher mapStitcher = new MapStitcher(objects);

        Vector2[] points = mapStitcher.getPoints();
        mainMap.setMapLine(points);

        // TODO place objects

        Camera.main.GetComponent<followPlayer>().setMap(mainMap);

        MapDrawer drawer = Instantiate<MapDrawer>(drawerPrefab,mainMap.transform) as MapDrawer;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
