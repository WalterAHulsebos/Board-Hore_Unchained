using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obscurable : MonoBehaviour
{
    private void Awake()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        int length = renderers.Length;
        for (int i = 0; i < length; i++)
            renderers[i].material.renderQueue = 3002;
    }
}
