using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotController : MonoBehaviour
{
    public BackpackItem slotItem;
    public Image slotImage;
    public TMP_Text slotName;
    public TMP_Text slotItemNumber;
    public Button spriteBtn;

    public void Initialize(BackpackItem backpackItem)
    {
        slotItem = backpackItem;
        slotImage.sprite = backpackItem.item.itemSprite;
        slotName.text = backpackItem.item.name;
        slotItemNumber.text = backpackItem.numberOfItems.ToString();
    }

    public void SpriteOnClick()
    {
        ClientUIController.instance.DisplayItemDescriptionMenu(slotItem);
    }

    public void SelectOnClick()
    {

    }

}
