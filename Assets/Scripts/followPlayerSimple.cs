using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class followPlayerSimple : MonoBehaviour {

    [SerializeField]
    Text[] _texts;
    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void setText(string text, Color color)
    {
        foreach ( Text t in _texts)
        {
            if (t.color != Color.black)
                t.color = color;
            t.text = text;
        }
    }
}
