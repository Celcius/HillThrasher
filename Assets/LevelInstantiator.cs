using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInstantiator : MonoBehaviour
{
    [SerializeField]
    MapController mainMapPrefab;


    [SerializeField]
    LevelDefinition _level;
	// Use this for initialization
	void Start () {
        
        CreateLevel();


        Destroy(transform.gameObject);
	}

    void CreateLevel() {
        MapController mainMap = GameObject.Instantiate<MapController>(mainMapPrefab,transform.parent) as MapController;
        mainMap.tag = "Terrain";
        mainMap.gameObject.layer = LayerMask.NameToLayer("Terrain");

        MapStitcher mapStitcher = new MapStitcher(_level, mainMap.transform);

        Vector2[] points = mapStitcher.getPoints();
        mainMap.setMapLine(points);

        // TODO place objects

        Camera.main.GetComponent<followPlayer>().setMap(mainMap);

        MapDrawer drawer = Instantiate<MapDrawer>(_level.drawerPrefab,mainMap.transform) as MapDrawer;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
