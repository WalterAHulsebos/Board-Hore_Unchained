using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    public float distance = 1.0f;
    
    void Update()
    {
        var screenPoint = Input.mousePosition;
        screenPoint.z = distance; //distance of the plane from the camera
        transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
    }
}