using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TrashCan : MonoBehaviour, IInteractable
{
    public void Interact(PlayerInteraction player)
    {
        if (player.HeldItem != null)
        {
            // Clear the item from the player's hands without dropping it
            GameObject itemToTrash = player.ClearHeldItem();
            
            if (itemToTrash != null)
            {
                Debug.Log($"[TrashCan] Destroyed {itemToTrash.name}");
                Destroy(itemToTrash);
            }
        }
        else
        {
            Debug.Log("[TrashCan] Player is empty-handed. Nothing to trash.");
        }
    }
}
