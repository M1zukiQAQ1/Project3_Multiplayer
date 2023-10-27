using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.EventSystems;
using Mono.Cecil.Cil;
using UnityEngine.UIElements;

public class ItemOnHoverControllerForImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private GameObject dialogBoxPrefab;
    [SerializeField] private Vector3 dialogBoxOffsetValue = new();
    private GameObject currentDialogBox;
    private BackpackItem slotItem;

    public void Initialize(BackpackItem slotItem)
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(currentDialogBox != null) return;

        currentDialogBox = Instantiate(dialogBoxPrefab, ClientUIController.instance.transform);
        var displayText = $"{slotItem.numberOfItems} {slotItem.item.displayName}(s)\r\n{slotItem.item.itemDescription}";
        if (slotItem.item.isUsable)
        {
            displayText += "\r\nLeft Click to Use";
        }
        currentDialogBox.GetComponentInChildren<TMP_Text>().text = displayText;
        StartCoroutine(IETrackMousePositionToDescription());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentDialogBox != null) Destroy(currentDialogBox);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (slotItem.item.isUsable)
        {
            var targetPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>();
            slotItem.item.Use(targetPlayer);
            Destroy(currentDialogBox);
        }
    }
}
