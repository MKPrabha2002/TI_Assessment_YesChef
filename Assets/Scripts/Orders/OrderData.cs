using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrderData
{
    public List<IngredientType> RequiredIngredients;
    public float TimeCreated;
    public int BaseScoreValue;

    public OrderData()
    {
        RequiredIngredients = new List<IngredientType>();
        TimeCreated = 0f; // Set externally by OrderGenerator at runtime
    }

    /// <summary>
    /// Checks if the order has been completely fulfilled.
    /// Ingredients are destructively removed from the list as they are delivered.
    /// </summary>
    public bool IsComplete()
    {
        return RequiredIngredients.Count == 0;
    }
}
