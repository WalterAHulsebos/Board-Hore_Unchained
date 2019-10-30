using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmearEffect : MonoBehaviour
{
    Queue<Vector3> _recentPositions = new Queue<Vector3>();

    [SerializeField] private new Renderer renderer;
    
    [SerializeField] private int frameLag = 0;
    
    private static readonly int PrevPosition = Shader.PropertyToID("_PrevPosition");
    private static readonly int Position = Shader.PropertyToID("_Position");

    private Material _smearMat = null;
    public Material SmearMat
    {
        get
        {
            if(!_smearMat)
            {
                _smearMat = renderer.material;
            }

            if(!_smearMat.HasProperty("_PrevPosition"))
            {
                _smearMat.shader = Shader.Find("Custom/Smear");
            }

            return _smearMat;
        }
    }

    private void LateUpdate()
    {
        if(_recentPositions.Count > frameLag)
            SmearMat.SetVector(PrevPosition, _recentPositions.Dequeue());

        Vector3 __position = transform.position;
        
        SmearMat.SetVector(Position, __position);
        _recentPositions.Enqueue(__position);
    }
}