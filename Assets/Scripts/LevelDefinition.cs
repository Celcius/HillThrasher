using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LevelDefinitionSection {
    public MapController[] maps;
    public float[] weights;
    public int minLength;
    public int maxLength;
}

[CreateAssetMenu(fileName = "level_", menuName = "Levels/LevelDefinition", order = 1)]
public class LevelDefinition : ScriptableObject {

    public LevelDefinitionSection[] sections;
    public MapDrawer drawerPrefab;
}
