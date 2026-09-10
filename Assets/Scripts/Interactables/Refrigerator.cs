using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Refrigerator : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject ingredientPrefab;

    public void Interact(PlayerInteraction player)
    {
        if (player.HeldItem == null && ingredientPrefab != null)
        {
            // Instantiate the ingredient
            GameObject newIngredient = Instantiate(ingredientPrefab);
            
            // Name it cleanly
            newIngredient.name = ingredientPrefab.name;

            // Pass it directly to the player
            player.GrabItem(newIngredient);
            
            Debug.Log($"[Refrigerator] Dispensed {newIngredient.name}");
        }
        else if (player.HeldItem != null)
        {
            Debug.LogWarning("[Refrigerator] Cannot dispense: Player is already holding an item.");
        }
    }
}
