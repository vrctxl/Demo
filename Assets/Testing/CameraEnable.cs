
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CameraEnable : UdonSharpBehaviour
{
    void Start()
    {
        Camera cam = GetComponent<Camera>();
        if (cam)
            cam.enabled = true;
    }
}
