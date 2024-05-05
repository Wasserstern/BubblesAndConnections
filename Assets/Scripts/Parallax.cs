using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Camera mainCamera;
    public float[] parallax;
    Vector3[] originalLayerPosition;
    void Start()
    {
        originalLayerPosition = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++){
            originalLayerPosition[i] = transform.GetChild(i).position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < transform.childCount; i++){
            Transform backgroundLayer = transform.GetChild(i);
            backgroundLayer.transform.position = originalLayerPosition[i] + mainCamera.transform.position * parallax[i];
        }
    }
}
