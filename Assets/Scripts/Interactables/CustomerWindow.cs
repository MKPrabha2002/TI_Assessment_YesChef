using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Collider))]
public class CustomerWindow : MonoBehaviour, IInteractable
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image[] ingredientIcons; // Array of 3 UI Images

    [Header("UI Colors")]
    [SerializeField] private Color vegColor = Color.green;
    [SerializeField] private Color cheeseColor = Color.yellow;
    [SerializeField] private Color meatColor = Color.red;

    [Header("Floating Score")]
    [SerializeField] private GameObject floatingScorePrefab;
    [SerializeField] private Transform scoreSpawnPoint;

    private OrderData currentOrder;
    private bool isWaitingForNewOrder = false;
    private const float newOrderDelay = 5.0f;

    private void Start()
    {
        RequestNewOrder();
    }

    private void Update()
    {
        if (currentOrder != null && timerText != null)
        {
            // Calculate and display elapsed time in whole seconds
            float elapsedTime = Time.time - currentOrder.TimeCreated;
            timerText.text = Mathf.FloorToInt(elapsedTime).ToString() + "s";
        }
    }

    private void RequestNewOrder()
    {
        if (OrderGenerator.Instance == null)
        {
            Debug.LogError("[CustomerWindow] OrderGenerator Singleton not found!");
            return;
        }

        currentOrder = OrderGenerator.Instance.GenerateNewOrder();
        isWaitingForNewOrder = false;

        UpdateUIIcons();
        Debug.Log($"[CustomerWindow] {gameObject.name} received a new order with {currentOrder.RequiredIngredients.Count} items.");
    }

    public void Interact(PlayerInteraction player)
    {
        // Reject if we are waiting for a new order
        if (currentOrder == null || isWaitingForNewOrder) return;

        if (player.HeldItem != null)
        {
            Ingredient heldIngredient = player.HeldItem.GetComponent<Ingredient>();

            if (heldIngredient != null)
            {
                // Validation 1: Check if the state is Prepared
                if (heldIngredient.CurrentState == PreparationState.Prepared)
                {
                    // Validation 2: Check if this specific ingredient type is required by the order
                    if (currentOrder.RequiredIngredients.Contains(heldIngredient.Type))
                    {
                        // --- FULFILLMENT ---
                        
                        // 1. Remove exactly one instance of this type from the required list
                        currentOrder.RequiredIngredients.Remove(heldIngredient.Type);

                        // 2. Safely clear from player hands and destroy
                        GameObject itemToDestroy = player.ClearHeldItem();
                        Destroy(itemToDestroy);

                        // 3. Update the UI to visually grey out the delivered icon
                        GreyOutDeliveredIcon(heldIngredient.Type);

                        // 4. Check for Completion
                        if (currentOrder.IsComplete())
                        {
                            int finalScore = ScoreCalculator.CalculateFinalScore(currentOrder);
                            Debug.Log($"[CustomerWindow] Order complete on {gameObject.name}! Base: {currentOrder.BaseScoreValue}. Final Score: {finalScore}");
                            
                            // Report to global GameManager
                            if (GameManager.Instance != null)
                            {
                                GameManager.Instance.AddScore(finalScore);
                            }

                            StartCoroutine(HandleOrderCompletion(finalScore));
                        }
                    }
                    else
                    {
                        Debug.Log("[CustomerWindow] Rejected: Order does not require this ingredient.");
                    }
                }
                else
                {
                    Debug.Log("[CustomerWindow] Rejected: Ingredient is RAW and must be prepared first.");
                }
            }
        }
    }

    private void UpdateUIIcons()
    {
        // Loop through all 3 slots
        for (int i = 0; i < ingredientIcons.Length; i++)
        {
            if (i < currentOrder.RequiredIngredients.Count)
            {
                // This slot is needed
                ingredientIcons[i].gameObject.SetActive(true);
                ingredientIcons[i].color = GetColorForIngredient(currentOrder.RequiredIngredients[i]);
            }
            else
            {
                // Hide unused slot (e.g., the 3rd icon if it's a 2-item order)
                ingredientIcons[i].gameObject.SetActive(false);
            }
        }
    }

    private void GreyOutDeliveredIcon(IngredientType typeDelivered)
    {
        // Find the first active, non-greyed out icon that matches the type we just delivered
        Color deliveredColor = GetColorForIngredient(typeDelivered);
        
        for (int i = 0; i < ingredientIcons.Length; i++)
        {
            if (ingredientIcons[i].gameObject.activeSelf && ingredientIcons[i].color == deliveredColor)
            {
                // Found it! Grey it out
                ingredientIcons[i].color = Color.gray;
                break; // Only grey out ONE icon
            }
        }
    }

    private IEnumerator HandleOrderCompletion(int finalScore)
    {
        isWaitingForNewOrder = true;
        currentOrder = null;

        // Clear UI text and icons
        if (timerText != null) timerText.text = "Done!";
        
        foreach (var icon in ingredientIcons)
        {
            icon.gameObject.SetActive(false);
        }

        // Spawn the floating score visual right as the UI clears
        if (floatingScorePrefab != null && scoreSpawnPoint != null)
        {
            GameObject floatObj = Instantiate(floatingScorePrefab, scoreSpawnPoint.position, scoreSpawnPoint.rotation);
            FloatingScore floatScript = floatObj.GetComponent<FloatingScore>();
            if (floatScript != null)
            {
                floatScript.SetScore(finalScore);
            }
        }

        // Wait strictly for 5 seconds as specified
        yield return new WaitForSeconds(newOrderDelay);

        RequestNewOrder();
    }

    private Color GetColorForIngredient(IngredientType type)
    {
        switch (type)
        {
            case IngredientType.Vegetables: return vegColor;
            case IngredientType.Cheese: return cheeseColor;
            case IngredientType.Meat: return meatColor;
            default: return Color.white;
        }
    }
}
