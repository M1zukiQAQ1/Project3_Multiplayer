using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class CameraController : MonoBehaviour
{
    public float xSens, ySens;

    [SerializeField] private Transform orientation;
    private float xRotation = 0;
    private float yRotation = 0;
    
    void Update()
    {
        if (ClientUIController.instance != null && ClientUIController.instance.IsUsingUIElements()) return;

        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSens;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySens;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80, 80);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);

        if(orientation == null)
        {
            orientation = GameManager.instance.GetPlayerOwnedByClient().orientation;
        }
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);

    }
}
