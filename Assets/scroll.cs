using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scroll : MonoBehaviour {

    [SerializeField]
    float scrollSpeed;
    float savedOffset;
    Renderer renderer;
	// Use this for initialization
	void Start () {
        renderer = GetComponent<Renderer>();
         savedOffset = renderer.material.GetTextureOffset("_MainTex").y;
     }

        void Update () {
            float x = Mathf.Repeat(Time.time * scrollSpeed, 1);
            Vector2 offset = new Vector2(x, savedOffset);
            renderer.material.SetTextureOffset("_MainTex", offset);
        }

    }
