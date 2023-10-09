using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class CameraMover : MonoBehaviour
{
    // Start is called before the first frame updatel
    [SerializeField] private Transform camPosition;

    void Start()
    {
        if(GameManager.instance.GetPlayerOwnedByClient() != null)
            camPosition = GameManager.instance.GetPlayerOwnedByClient().cameraPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance == null) return;
        if(camPosition == null || NetworkManager.Singleton.LocalClient.PlayerObject == null)
        {
            try
            {
                camPosition = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().cameraPos;
            }
            catch(Exception e)
            {
                Debug.LogWarning("Camera Mover: Cannot find the player, is the server or client started?");
                Debug.LogWarning(e);
                return;
            }
        }

        transform.position = camPosition.position;
    }
}
