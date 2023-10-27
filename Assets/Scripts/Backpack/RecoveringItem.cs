using UnityEngine;

[CreateAssetMenu(fileName = "New Recovering Item", menuName = "Backpack/Recovering Item")]
public class RecoveringItem : Item {
    public PlayerController.AttributesOfPlayer.AttributeType attributeType;
    public float recoverAmount;
    public override void Use(PlayerController targetPlayer)
    {
        targetPlayer.attributes.ChangeValueOfAttribute(attributeType, recoverAmount);
        base.Use(targetPlayer);
    }
}