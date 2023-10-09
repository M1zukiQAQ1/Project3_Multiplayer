using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotController : MonoBehaviour
{
    public Item slotItem;
    public Image slotImage;
    public TMP_Text slotName;
    public TMP_Text slotItemNumber;
    public Button spriteBtn;

    public void SpriteOnClick()
    {
        ClientUIController.instance.DisplayItemDescriptionMenu(slotItem);
    }

    public void SelectOnClick()
    {

    }

}
