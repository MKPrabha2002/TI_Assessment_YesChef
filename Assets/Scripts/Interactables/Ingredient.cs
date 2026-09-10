using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Ingredient : MonoBehaviour, IInteractable
{
    public IngredientType Type;
    public PreparationState CurrentState;

    public int ScoreValue { get; private set; }

    [Header("Configuration")]
    [SerializeField] private int vegetableBaseScore = 20;
    [SerializeField] private int cheeseBaseScore = 10;
    [SerializeField] private int meatBaseScore = 30;

    [Header("Visuals")]
    [SerializeField] private Color preparedColor = Color.black; // Tint color when prepared

    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();

        // Initialize state and score based on the ingredient type
        switch (Type)
        {
            case IngredientType.Vegetables:
                ScoreValue = vegetableBaseScore;
                CurrentState = PreparationState.Raw;
                break;
            case IngredientType.Cheese:
                ScoreValue = cheeseBaseScore;
                CurrentState = PreparationState.Prepared; // Cheese does not need to be prepared
                break;
            case IngredientType.Meat:
                ScoreValue = meatBaseScore;
                CurrentState = PreparationState.Raw;
                break;
        }
    }

    public void PrepareIngredient()
    {
        if (CurrentState == PreparationState.Raw)
        {
            CurrentState = PreparationState.Prepared;
            
            // Apply visual distinction
            if (_meshRenderer != null)
            {
                _meshRenderer.material.color = preparedColor;
            }

            Debug.Log($"[{gameObject.name}] was prepared! Score value is now {ScoreValue}.");
        }
    }

    public void Interact(PlayerInteraction player)
    {
        // Only allow picking up if the player is empty-handed
        if (player.HeldItem == null)
        {
            player.GrabItem(this.gameObject);
        }
    }
}
