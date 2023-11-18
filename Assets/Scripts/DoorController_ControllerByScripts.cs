using System.Collections;
using UnityEngine;

public class DoorController_ControllerByScripts : MonoBehaviour
{
    [Header("Open by Movement")]
    [SerializeField] private Vector3 positionToAdd;
    [SerializeField] private float doorMoveSpeed;

    // True -> if the door is opened by rotation
    // False -> if the door is opened by moving
    [Header("Open by Rotation")]
    [SerializeField] private bool isRotation;
    
    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.position;
    }

    private Coroutine currentDoorOperation;

    public void OpenDoor()
    {
        if (currentDoorOperation != null) StopCoroutine(currentDoorOperation);
        if (!isRotation) currentDoorOperation = StartCoroutine(IEMoveOpenDoor());
    }

    private IEnumerator IEMoveOpenDoor()
    {
        yield return new WaitUntil(() =>
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + positionToAdd, Time.deltaTime * doorMoveSpeed);
            return Vector3.Distance(transform.position, originalPosition + positionToAdd) < 0.1f;
        });
    }

    public void CloseDoor()
    {
        if (currentDoorOperation != null) StopCoroutine(currentDoorOperation);
        currentDoorOperation = StartCoroutine(IEMoveCloseDoor());
    }

    private IEnumerator IEMoveCloseDoor()
    {
        yield return new WaitUntil(() =>
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, Time.deltaTime * doorMoveSpeed);
            return Vector3.Distance(transform.position, originalPosition) < 0.1f;
        });
    }
}