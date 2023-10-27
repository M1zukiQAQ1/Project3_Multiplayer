using System.Collections;
using UnityEngine;

public class DoorController_ControllerByScripts : MonoBehaviour {
    [SerializeField] private Vector3 positionToAdd; 
    [SerializeField] private float doorMoveSpeed;
    private Vector3 originalPosition;

    private void Start() {
        originalPosition = transform.position;  
    }

    private Coroutine currentDoorOperation;

    public void OpenDoor(){
        if(currentDoorOperation != null) StopCoroutine(currentDoorOperation);        
        currentDoorOperation = StartCoroutine(IEOpenDoor());
    }

    private IEnumerator IEOpenDoor(){
        yield return new WaitUntil(() => {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + positionToAdd, Time.deltaTime * doorMoveSpeed);
            return Vector3.Distance(transform.position, originalPosition + positionToAdd) < 0.1f;
        });
    }

    public void CloseDoor(){
        if(currentDoorOperation != null) StopCoroutine(currentDoorOperation);
        currentDoorOperation = StartCoroutine(IECloseDoor());
    }

    private IEnumerator IECloseDoor(){
        yield return new WaitUntil(() => {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, Time.deltaTime * doorMoveSpeed);
            return Vector3.Distance(transform.position, originalPosition) < 0.1f;
        });
    }
}