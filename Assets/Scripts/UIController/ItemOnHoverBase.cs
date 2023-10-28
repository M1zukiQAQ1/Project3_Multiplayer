using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.EventSystems;

public abstract class ItemOnHoverBase: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private GameObject dialogBoxPrefab;
    [SerializeField] private Vector3 dialogBoxOffsetValue = new(100, -50, 0);
    protected GameObject currentDialogBox;
    protected BackpackItem slotItem;

    public virtual void Initialize(BackpackItem slotItem)
    {
        this.slotItem = slotItem;
    }

    private IEnumerator IETrackMousePositionToDescription()
    {
        yield return new WaitUntil(() => {
            if(currentDialogBox == null) return true;
            currentDialogBox.transform.position = Input.mousePosition + dialogBoxOffsetValue;
            return false;
        });
    }

    protected abstract string GetDialogBoxDisplayText();

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(currentDialogBox != null) return;

        currentDialogBox = Instantiate(dialogBoxPrefab, ClientUIController.instance.transform);

        currentDialogBox.GetComponentInChildren<TMP_Text>().text = GetDialogBoxDisplayText();
        StartCoroutine(IETrackMousePositionToDescription());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentDialogBox != null) Destroy(currentDialogBox);
    }

    public abstract void OnPointerDown(PointerEventData eventData);
}
