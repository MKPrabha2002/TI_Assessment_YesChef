using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float detectionRadius = 2f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("Highlight Settings")]
    [SerializeField] private Color highlightColor = new Color(1f, 1f, 1f, 127f / 255f);

    [Header("Holding Settings")]
    [SerializeField] private Transform holdPoint;
    
    public GameObject HeldItem { get; private set; }

    /// <summary>
    /// The GameObject of the nearest IInteractable in range. Used by InteractionDebugUI.
    /// </summary>
    public GameObject NearestInteractableObject { get; private set; }

    // Track highlight state so we can restore original color
    private Renderer currentHighlightedRenderer;
    private Color originalHighlightColor;

    private void Update()
    {
        UpdateNearestInteractable();
    }

    /// <summary>
    /// Every frame, detect the closest interactable and apply/remove highlight.
    /// </summary>
    private void UpdateNearestInteractable()
    {
        GameObject previousNearest = NearestInteractableObject;

        // Find what's currently closest
        IInteractable closestInteractable = FindClosestInteractable(out GameObject closestObject);

        NearestInteractableObject = closestObject;

        // --- Remove highlight from old target ---
        if (previousNearest != null && previousNearest != NearestInteractableObject)
        {
            RemoveHighlight();
        }

        // --- Apply highlight to new target ---
        if (NearestInteractableObject != null && NearestInteractableObject != previousNearest)
        {
            ApplyHighlight(NearestInteractableObject);
        }
    }

    // Called by PlayerMovement's Input hook
    public void OnInteractPressed()
    {
        if (NearestInteractableObject != null)
        {
            IInteractable interactable = NearestInteractableObject.GetComponent<IInteractable>();
            if (interactable != null)
            {
                string objectName = NearestInteractableObject.name;
                interactable.Interact(this);
                Debug.Log($"[Interact] Triggered on: {objectName}");
            }
        }
        else if (HeldItem != null)
        {
            DropItem();
        }
    }

    private IInteractable FindClosestInteractable(out GameObject closestObject)
    {
        closestObject = null;

        // Use OverlapSphere centered slightly in front of the player at ground level
        Vector3 center = transform.position + transform.forward * 0.5f;
        Collider[] hits = Physics.OverlapSphere(center, detectionRadius, interactableLayer);

        IInteractable closest = null;
        float minDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = interactable;
                    closestObject = hit.gameObject;
                }
            }
        }

        return closest;
    }

    private void ApplyHighlight(GameObject target)
    {
        Renderer rend = target.GetComponent<Renderer>();
        if (rend != null)
        {
            currentHighlightedRenderer = rend;
            originalHighlightColor = rend.material.color;
            rend.material.color = highlightColor;
        }
    }

    private void RemoveHighlight()
    {
        if (currentHighlightedRenderer != null)
        {
            currentHighlightedRenderer.material.color = originalHighlightColor;
            currentHighlightedRenderer = null;
        }
    }

    public void GrabItem(GameObject item)
    {
        if (HeldItem != null)
        {
            Debug.LogWarning("Player is already holding an item!");
            return;
        }

        // Remove highlight before grabbing (it's about to disappear from the floor)
        RemoveHighlight();
        NearestInteractableObject = null;

        HeldItem = item;
        
        // Disable physics interaction while holding
        Collider itemCollider = item.GetComponent<Collider>();
        if (itemCollider != null) itemCollider.enabled = false;
        
        // Remove Rigidbody dynamics if it exists
        Rigidbody itemRb = item.GetComponent<Rigidbody>();
        if (itemRb != null) itemRb.isKinematic = true;

        // Parent and snap to hold point
        item.transform.SetParent(holdPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        Debug.Log($"[GrabItem] Picked up: {item.name}");
    }

    /// <summary>
    /// Removes the held item from the player without dropping it on the floor.
    /// Useful for transferring items to stations or destroying them.
    /// </summary>
    /// <returns>The GameObject that was held.</returns>
    public GameObject ClearHeldItem()
    {
        if (HeldItem == null) return null;

        GameObject clearedItem = HeldItem;
        HeldItem = null;

        clearedItem.transform.SetParent(null);
        return clearedItem;
    }

    public void DropItem()
    {
        if (HeldItem == null) return;

        GameObject itemToDrop = HeldItem;
        HeldItem = null;

        itemToDrop.transform.SetParent(null);
        
        // Place item slightly in front of player on the floor
        itemToDrop.transform.position = transform.position + transform.forward * 1.0f + Vector3.up * 0.5f;

        // Re-enable physics
        Collider itemCollider = itemToDrop.GetComponent<Collider>();
        if (itemCollider != null) itemCollider.enabled = true;

        Rigidbody itemRb = itemToDrop.GetComponent<Rigidbody>();
        if (itemRb != null) itemRb.isKinematic = false;

        Debug.Log($"[DropItem] Dropped: {itemToDrop.name}");
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the interaction sphere
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position + transform.forward * 0.5f;
        Gizmos.DrawWireSphere(center, detectionRadius);
    }
}
