using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class ChoppingTable : MonoBehaviour, IInteractable
{
    [Header("Table Settings")]
    [SerializeField] private Transform itemSlot;
    
    [Header("UI Integration")]
    [SerializeField] private Slider progressBar;
    
    private Ingredient currentIngredient;
    private bool isChopping = false;
    private const float choppingDuration = 2.0f;

    private void Start()
    {
        // Ensure UI is hidden initially
        if (progressBar != null)
        {
            Canvas parentCanvas = progressBar.GetComponentInParent<Canvas>(true);
            if (parentCanvas != null)
            {
                parentCanvas.gameObject.SetActive(false);
            }
            else
            {
                progressBar.gameObject.SetActive(false);
            }
            progressBar.value = 0f;
        }
    }

    public void Interact(PlayerInteraction player)
    {
        // CASE 1: Placement Logic
        if (currentIngredient == null && player.HeldItem != null)
        {
            Ingredient playerIngredient = player.HeldItem.GetComponent<Ingredient>();
            
            // Only accept Raw Vegetables
            if (playerIngredient != null && 
                playerIngredient.Type == IngredientType.Vegetables && 
                playerIngredient.CurrentState == PreparationState.Raw)
            {
                // Remove from player
                GameObject itemToChop = player.ClearHeldItem();
                
                // Place on table
                itemToChop.transform.SetParent(itemSlot);
                itemToChop.transform.localPosition = Vector3.zero;
                itemToChop.transform.localRotation = Quaternion.identity;
                
                currentIngredient = playerIngredient;
                
                // Start chopping process
                StartCoroutine(ChopVegetable());
            }
            else
            {
                Debug.Log("[ChoppingTable] Invalid item. Only Raw Vegetables can be placed here.");
            }
        }
        // CASE 2: Retrieval Logic
        else if (currentIngredient != null && player.HeldItem == null)
        {
            if (!isChopping)
            {
                // Unparent from table and give to player
                currentIngredient.transform.SetParent(null);
                player.GrabItem(currentIngredient.gameObject);
                
                currentIngredient = null;
                Debug.Log("[ChoppingTable] Player retrieved the prepared vegetable.");
            }
            else
            {
                Debug.Log("[ChoppingTable] Cannot retrieve vegetable. Chopping in progress...");
            }
        }
    }

    private IEnumerator ChopVegetable()
    {
        isChopping = true;
        float timer = 0f;
        
        // Enable and reset UI
        if (progressBar != null)
        {
            Canvas parentCanvas = progressBar.GetComponentInParent<Canvas>(true);
            if (parentCanvas != null)
            {
                parentCanvas.gameObject.SetActive(true);
            }
            progressBar.gameObject.SetActive(true);
            progressBar.value = 0f;
        }
        
        Debug.Log("[ChoppingTable] Started chopping...");

        // Process over 2 seconds
        while (timer < choppingDuration)
        {
            timer += Time.deltaTime;
            
            // Smoothly update UI slider (0 to 1)
            if (progressBar != null)
            {
                progressBar.value = timer / choppingDuration;
            }
            
            yield return null;
        }

        // Completion
        currentIngredient.PrepareIngredient();
        isChopping = false;
        
        // Hide UI
        if (progressBar != null)
        {
            Canvas parentCanvas = progressBar.GetComponentInParent<Canvas>(true);
            if (parentCanvas != null)
            {
                parentCanvas.gameObject.SetActive(false);
            }
            else
            {
                progressBar.gameObject.SetActive(false);
            }
        }
        
        Debug.Log("[ChoppingTable] Chopping complete!");
    }
}
