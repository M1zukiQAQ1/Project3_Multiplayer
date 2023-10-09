using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode.Components;

public class DoorController : MonoBehaviour
{
    public Transform doorObj, doorOpenedPos;
    public float doorMoveSpeed;

    [Header("Attributes")]
    public bool isControlledByInteract = false;
    public bool isOpened = false;

    public event Action OnDoorOpened;

    [SerializeField] private NetworkAnimator doorAnimator;

    private void Start()
    {
        doorAnimator = GetComponentInChildren<NetworkAnimator>();  
    }

    public void CloseTheDoor()
    {
        doorAnimator.SetTrigger("Close");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided!");
        if (!isControlledByInteract && other.CompareTag("Player"))
        {
            Debug.Log("Opening!");
            doorAnimator.SetTrigger("Open");
            isOpened = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isOpened && other.CompareTag("Player"))
        {
            doorAnimator.SetTrigger("Close");
            isOpened = false;
        }
    }
}
