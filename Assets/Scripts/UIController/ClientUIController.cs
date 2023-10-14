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

    [Header("UI: Backpack System -> Item Description Panel")]
    [SerializeField] private RectTransform itemDescriptionPanel;
    [SerializeField] private TMP_Text itemUseDescriptionText;
    [SerializeField] private Image itemSpriteImage;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private Button selectBtn;

    [Header("UI: Backpack System -> Recipe Panel")]
    [SerializeField] private RectTransform recipePanel;
    [SerializeField] private RectTransform recipeScrollViewContentGrid;
    [SerializeField] private Button recipeButtonPrefab;

    [Header("UI: Health Bar")]
    [SerializeField] private Slider healthBar;

    [Header("UI: Weapon")]
    [SerializeField] private TMP_Text bulletsIndicationText;

    [Header("UI: Indication Text")]
    public IndicationText indicationTextManager;

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

    public void UpdateBulletNumberText(int currentBullets, int totalBullets)
    {
        Debug.Log($"UI: UpdateBulletNumberText function called");
        bulletsIndicationText.text = $"{currentBullets} / {totalBullets}";
    }

    public void UpdateBulletNumberText(string text)
    {
        bulletsIndicationText.text = text;
    }

    public void OpenBackpackPanel()
    {
        backpackPanel.gameObject.SetActive(true);
        RefreshBackpackPanel();
    }

    public void OpenRecipePanel()
    {
        recipePanel.gameObject.SetActive(true);
        RefreshRecipePanel();
    }
    
    private void RefreshRecipePanel()
    {
        Debug.Log($"UI: Refreshing recipe panel");
        foreach (Button btn in recipeScrollViewContentGrid.GetComponentsInChildren<Button>())
            Destroy(btn.gameObject);
        foreach (Item recipe in GameManager.instance.GetPlayerOwnedByClient().backpack.GetAllItems ())
        {
            if(recipe is Recipe)
            {
                CreateRecipeButton(recipe as Recipe);
            }
        }
    }

    private void CreateRecipeButton(Recipe recipe)
    {
        var newButton = Instantiate(recipeButtonPrefab, recipeScrollViewContentGrid);
        newButton.GetComponentInChildren<TMP_Text>().text = recipe.displayName;
        newButton.onClick.AddListener(() => RecipeDescriptionController.instance.RefreshRecipeDescriptionPanel(recipe));
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

    public void DisplayItemDescriptionMenu(BackpackItem itemToDisplay)
    {
        if (itemToDisplay.item.isUsable)
        {
            var targetPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>();
            selectBtn.gameObject.SetActive(true);
            selectBtn.onClick.AddListener(() =>
            {
                itemToDisplay.item.Use(targetPlayer);
                CloseItemDescriptionPanel();
                RefreshBackpackPanel();
            });
        }

        itemDescriptionPanel.gameObject.SetActive(true);
        itemSpriteImage.sprite = itemToDisplay.item.itemSprite;
        itemName.text = itemToDisplay.item.displayName;
        itemDescriptionText.text = itemToDisplay.item.itemDescription;

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
        if (backpackItem.item is Recipe)
            return;
        Instantiate(slotObject, slotsContainer.transform).Initialize(backpackItem);
    }

    public void CloseHintText()
    {
        hintText.gameObject.SetActive(false);
        hintText.text = "";
    }
}
