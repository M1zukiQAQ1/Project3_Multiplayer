using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MonitorController : MonoBehaviour
{
    [SerializeField] private Dictionary<Camera, DoorController> monitor;
    [SerializeField] private List<Camera> targetCameras;
    [SerializeField] private RenderTexture camRenderTexture;
    private int currentCam = 0;

    public void DisplayNextCamera()
    {
        Debug.Log($"MonitorController: currentCam = {currentCam}");
        targetCameras[currentCam % targetCameras.Count].targetTexture = null;
        currentCam = currentCam + 1 == targetCameras.Count ? 0 : currentCam + 1;
        Debug.Log($"MonitorController: currentCam = {currentCam}");
        targetCameras[currentCam % targetCameras.Count].targetTexture = camRenderTexture;
    }

    public void DisplayPriorCamera()
    {
        Debug.Log($"MonitorController: currentCam = {currentCam}");
        targetCameras[currentCam % targetCameras.Count].targetTexture = null;
        currentCam = currentCam - 1 < 0 ? targetCameras.Count - 1 : currentCam - 1;
        Debug.Log($"MonitorController: currentCam = {currentCam}");
        targetCameras[currentCam % targetCameras.Count].targetTexture = camRenderTexture;
    }
}


