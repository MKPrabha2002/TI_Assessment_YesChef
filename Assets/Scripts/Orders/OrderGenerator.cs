using System.Collections.Generic;
using UnityEngine;

public class OrderGenerator : MonoBehaviour
{
    public static OrderGenerator Instance { get; private set; }

    [Header("Generation Settings")]
    [Tooltip("Probability (0.0 to 1.0) of generating an order with 3 ingredients instead of 2.")]
    [Range(0f, 1f)]
    [SerializeField] private float chanceForThreeIngredients = 0.5f;

    [Header("Scoring Weights")]
    [SerializeField] private int vegBaseScore = 20;
    [SerializeField] private int cheeseBaseScore = 10;
    [SerializeField] private int meatBaseScore = 30;

    private void Awake()
    {
        // Standard Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Temporary testing loop to verify generation logic works independently
        Debug.Log("--- Order Generator Test ---");
        for (int i = 1; i <= 5; i++)
        {
            OrderData testOrder = GenerateNewOrder();
            string items = string.Join(", ", testOrder.RequiredIngredients);
            Debug.Log($"[OrderGenerator] Order {i} ({testOrder.RequiredIngredients.Count} items): {items}");
        }
    }

    /// <summary>
    /// Generates a completely new, randomized order.
    /// </summary>
    public OrderData GenerateNewOrder()
    {
        OrderData newOrder = new OrderData();
        newOrder.TimeCreated = Time.time; // Set at runtime to avoid serialization errors

        // Determine size based on the exposed probability weight
        int size = Random.value <= chanceForThreeIngredients ? 3 : 2;

        // Populate the order with purely random ingredients (duplicates naturally allowed)
        for (int i = 0; i < size; i++)
        {
            // Range(0, 3) returns 0, 1, or 2 (Vegetables, Cheese, Meat)
            IngredientType randomIngredient = (IngredientType)Random.Range(0, 3);
            newOrder.RequiredIngredients.Add(randomIngredient);

            // Tally the base score
            switch (randomIngredient)
            {
                case IngredientType.Vegetables: newOrder.BaseScoreValue += vegBaseScore; break;
                case IngredientType.Cheese: newOrder.BaseScoreValue += cheeseBaseScore; break;
                case IngredientType.Meat: newOrder.BaseScoreValue += meatBaseScore; break;
            }
        }

        return newOrder;
    }
}
