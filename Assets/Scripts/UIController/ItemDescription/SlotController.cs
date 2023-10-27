using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotController : MonoBehaviour
{
    [SerializeField] private BackpackItem slotItem;
    [SerializeField] private Image slotImage;
    [SerializeField] private TMP_Text slotDisplayName;
    
    
    public void Initialize(BackpackItem backpackItem)
    {
        slotItem = backpackItem;

        slotDisplayName.text = slotItem.item.displayName;
        
        slotImage.sprite = backpackItem.item.itemSprite;
        slotImage.SetNativeSize();
        slotImage.rectTransform.localScale = new(0.08f, 0.08f, 0.08f);
        slotImage.GetComponent<ItemOnHoverControllerForImage>().Initialize(slotItem);
    }
}
