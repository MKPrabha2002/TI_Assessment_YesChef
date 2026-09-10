using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class Stove : MonoBehaviour, IInteractable
{
    [Serializable]
    public class StoveSlot
    {
        public Transform itemSlot;
        public Slider progressBar;
        
        [HideInInspector] public Ingredient currentMeat;
        [HideInInspector] public bool isCooking = false;
    }

    [Header("Stove Configuration")]
    [SerializeField] private StoveSlot[] cookingSlots;
    private const float cookingDuration = 6.0f;

    private void Start()
    {
        // Ensure UI is hidden initially for all slots
        foreach (var slot in cookingSlots)
        {
            if (slot.progressBar != null)
            {
                Canvas parentCanvas = slot.progressBar.GetComponentInParent<Canvas>(true);
                if (parentCanvas != null)
                {
                    parentCanvas.gameObject.SetActive(false);
                }
                else
                {
                    slot.progressBar.gameObject.SetActive(false);
                }
                slot.progressBar.value = 0f;
            }
        }
    }

    public void Interact(PlayerInteraction player)
    {
        // CASE 1: Placement Logic (Player is holding an item)
        if (player.HeldItem != null)
        {
            Ingredient playerIngredient = player.HeldItem.GetComponent<Ingredient>();
            
            // Only accept Raw Meat
            if (playerIngredient != null && 
                playerIngredient.Type == IngredientType.Meat && 
                playerIngredient.CurrentState == PreparationState.Raw)
            {
                // Find an empty slot
                foreach (var slot in cookingSlots)
                {
                    if (slot.currentMeat == null)
                    {
                        // Remove from player
                        GameObject itemToCook = player.ClearHeldItem();
                        
                        // Place on this specific slot
                        itemToCook.transform.SetParent(slot.itemSlot);
                        itemToCook.transform.localPosition = Vector3.zero;
                        itemToCook.transform.localRotation = Quaternion.identity;
                        
                        slot.currentMeat = playerIngredient;
                        
                        // Start independent cooking process
                        StartCoroutine(CookMeat(slot));
                        return; // Successfully placed, exit method
                    }
                }
                
                Debug.Log("[Stove] All slots are currently full.");
            }
            else
            {
                Debug.Log("[Stove] Invalid item. Only Raw Meat can be cooked here.");
            }
        }
        // CASE 2: Retrieval Logic (Player's hands are empty)
        else if (player.HeldItem == null)
        {
            // Find a slot with prepared meat that is NOT currently cooking
            foreach (var slot in cookingSlots)
            {
                if (slot.currentMeat != null && !slot.isCooking)
                {
                    // Unparent from stove and give to player
                    slot.currentMeat.transform.SetParent(null);
                    player.GrabItem(slot.currentMeat.gameObject);
                    
                    slot.currentMeat = null;
                    Debug.Log("[Stove] Player retrieved the cooked meat.");
                    return; // Successfully retrieved, exit method
                }
            }
            
            Debug.Log("[Stove] No cooked meat available to retrieve.");
        }
    }

    private IEnumerator CookMeat(StoveSlot slot)
    {
        slot.isCooking = true;
        float timer = 0f;
        
        // Enable and reset UI for this specific slot
        if (slot.progressBar != null)
        {
            Canvas parentCanvas = slot.progressBar.GetComponentInParent<Canvas>(true);
            if (parentCanvas != null)
            {
                parentCanvas.gameObject.SetActive(true);
            }
            slot.progressBar.gameObject.SetActive(true);
            slot.progressBar.value = 0f;
        }
        
        Debug.Log($"[Stove] Started cooking meat on {(slot.itemSlot != null ? slot.itemSlot.name : "slot")}...");

        // Process over 6 seconds
        while (timer < cookingDuration)
        {
            timer += Time.deltaTime;
            
            // Smoothly update UI slider (0 to 1)
            if (slot.progressBar != null)
            {
                slot.progressBar.value = timer / cookingDuration;
            }
            
            yield return null;
        }

        // Completion
        slot.currentMeat.PrepareIngredient();
        slot.isCooking = false;
        
        // Hide UI for this specific slot
        if (slot.progressBar != null)
        {
            slot.progressBar.gameObject.SetActive(false);
            
            // Check if ALL slots are inactive before hiding the shared parent canvas
            bool anyActive = false;
            foreach (var s in cookingSlots)
            {
                if (s.progressBar != null && s.progressBar.gameObject.activeSelf)
                {
                    anyActive = true;
                    break;
                }
            }
            
            if (!anyActive)
            {
                Canvas parentCanvas = slot.progressBar.GetComponentInParent<Canvas>(true);
                if (parentCanvas != null)
                {
                    parentCanvas.gameObject.SetActive(false);
                }
            }
        }
        
        Debug.Log($"[Stove] Cooking complete on {(slot.itemSlot != null ? slot.itemSlot.name : "slot")}!");
    }
}
