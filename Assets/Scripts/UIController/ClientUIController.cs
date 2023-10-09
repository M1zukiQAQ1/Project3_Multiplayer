using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.UI;

public class ClientUIController : NetworkBehaviour
{
    // Start is called before the first frame update
    public static ClientUIController instance;

    [Header("UI: Damage Number")]
    [SerializeField] private GameObject damageNumberPrefab;

    [Header("UI: Interactable")]
    [SerializeField] private TMP_Text hintText;

    [Header("UI: Backpack System")]
    [SerializeField] private RectTransform backpackPanel;
    [SerializeField] private GameObject slotsContainer;
    [SerializeField] private SlotController slotObject;

    [Header("UI: Backpack System: Item Description Panel")]
    [SerializeField] private RectTransform itemDescriptionPanel;
    [SerializeField] private Image itemSpriteImage;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private Button selectBtn;

    [Header("UI: Health Bar")]
    [SerializeField] private Slider healthBar;

    // If UI elements increased, change this boolean expression
    public bool IsUsingUIElements() => backpackPanel.gameObject.activeSelf;

    private void Start()
    {
        if(instance == null)
        {
            Debug.Log("UI: Setting this as instance " + this);
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    [ClientRpc]
    public void DisplayDamageNumberClientRpc(float damage, NetworkObjectReference enemyRef)
    {
        var damageNumberObj = Instantiate(damageNumberPrefab).GetComponent<DamageNumberController>();
        damageNumberObj.transform.SetParent(transform);
        damageNumberObj.Initialize(damage);

        enemyRef.TryGet(out var enemyObject);

        StartCoroutine(IEUpdateHintTextPosition(damageNumberObj.transform, enemyObject.transform));
    }

    public void OpenBackpackPanel()
    {
        backpackPanel.gameObject.SetActive(true);
    }

    public void DisplayHintText(string hintStr, Transform objectToTrack) 
    {
        hintText.gameObject.SetActive(true);
        hintText.text = hintStr;
        StartCoroutine(IEUpdateHintTextPosition(hintText.transform, objectToTrack));
    }

    private IEnumerator IEUpdateHintTextPosition(Transform trackingObject, Transform objectToTrack)
    {
        while (trackingObject.gameObject.activeSelf)
        {
            if(objectToTrack == null)
            {
                yield break;
            }

            var screenPosition = Camera.main.WorldToScreenPoint(objectToTrack.position);
            // Debug.Log($"ClientUIController: Updating hint text's position to {screenPosition}");
            trackingObject.position = screenPosition;
            yield return new WaitForEndOfFrame();
        }
        // Debug.Log($"ClientUIController: Text's active state is {hintText.gameObject.activeSelf}, returning");
    }

    public void CloseItemDescriptionPanel()
    {
        itemDescriptionPanel.gameObject.SetActive(false);
        selectBtn.gameObject.SetActive(false);
        selectBtn.onClick.RemoveAllListeners();
    }

    public void DisplayItemDescriptionMenu(Item itemToDisplay)
    {
        itemDescriptionPanel.gameObject.SetActive(true);
        itemSpriteImage.sprite = itemToDisplay.itemSprite;
        itemName.text = itemToDisplay.displayName;
        itemDescriptionText.text = itemToDisplay.itemDescription;


        // Consider inheriting WeaponItem to UsableItem class, or make it implementing an interface to simplify this if
        if(itemToDisplay is WeaponItem)
        {
            selectBtn.gameObject.SetActive(true);
            Debug.Log("UIController: " + "IWeaponHolder " + NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<IWeaponHoldable>());
            selectBtn.onClick.AddListener(() => (itemToDisplay as WeaponItem).InstantiateWeaponWithOwnership(NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<IWeaponHoldable>()));
        }
    }

    public void UpdateHealthBarValue(float newHealth)
    {
        healthBar.value = newHealth;
    }

    public void RefreshBackpackPanel()
    {
        foreach (SlotController slot in slotsContainer.GetComponentsInChildren<SlotController>())
            Destroy(slot.gameObject);
        foreach (BackpackItem item in GameManager.instance.GetPlayerOwnedByClient().GetComponent<BackpackController>().GetAllBackpackItems())
            CreateSlot(item);
    }

    public void CreateSlot(BackpackItem backpackItem)
    {
        SlotController newSlot = Instantiate(slotObject, slotsContainer.transform);
        newSlot.slotItem = backpackItem.item;
        newSlot.slotImage.sprite = backpackItem.item.itemSprite;
        newSlot.slotName.text = backpackItem.item.name;
        newSlot.slotItemNumber.text = backpackItem.numberOfItems.ToString();
    }

    public void CloseHintText()
    {
        hintText.gameObject.SetActive(false);
        hintText.text = "";
    }
}
