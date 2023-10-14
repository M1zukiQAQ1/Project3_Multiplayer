using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Healing Item", menuName = "Backpack/Healing Item")]
public class HealingItem : Item
{
    [SerializeField] private float heallingAbility;

    // Consider adding cd to use item?
    public override void Use(PlayerController targetPlayer)
    {
        if(targetPlayer.TotalHealth >= targetPlayer.CurrentHealth.Value + heallingAbility)
        {
            targetPlayer.CurrentHealth.Value += heallingAbility;
        }
        else
        {
            targetPlayer.CurrentHealth.Value = targetPlayer.TotalHealth;
        }
        ClientUIController.instance.indicationTextManager.DisplayHintTextOnUI($"{heallingAbility} health restored");

        base.Use(targetPlayer);
    }
}
