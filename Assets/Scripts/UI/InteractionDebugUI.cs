using UnityEngine;

/// <summary>
/// Simple on-screen debug HUD that displays the current interaction state.
/// Attach to the Player alongside PlayerInteraction.
/// </summary>
[RequireComponent(typeof(PlayerInteraction))]
public class InteractionDebugUI : MonoBehaviour
{
    private PlayerInteraction playerInteraction;

    private void Awake()
    {
        playerInteraction = GetComponent<PlayerInteraction>();
    }

    private void OnGUI()
    {
        // --- Style ---
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.fontSize = 22;
        boxStyle.alignment = TextAnchor.MiddleLeft;
        boxStyle.normal.textColor = Color.white;
        boxStyle.padding = new RectOffset(16, 16, 12, 12);

        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = 22;
        labelStyle.normal.textColor = Color.white;
        labelStyle.richText = true;

        // --- Layout ---
        float boxWidth = 460f;
        float boxHeight = 130f;
        float margin = 20f;
        Rect boxRect = new Rect(margin, Screen.height - boxHeight - margin, boxWidth, boxHeight);

        GUI.Box(boxRect, "", boxStyle);

        // Held item
        string heldText = playerInteraction.HeldItem != null
            ? $"<color=yellow>{playerInteraction.HeldItem.name}</color>"
            : "<color=#888888>Empty</color>";

        // Nearest interactable
        string nearestText;
        GameObject nearest = playerInteraction.NearestInteractableObject;
        if (nearest != null)
        {
            nearestText = $"<color=#00FF88>{nearest.name}</color>";
        }
        else
        {
            nearestText = "<color=#888888>None</color>";
        }

        GUI.Label(new Rect(boxRect.x + 16, boxRect.y + 10, boxWidth - 32, 36),
            $"Holding: {heldText}", labelStyle);
        GUI.Label(new Rect(boxRect.x + 16, boxRect.y + 48, boxWidth - 32, 36),
            $"Nearest: {nearestText}", labelStyle);
        GUI.Label(new Rect(boxRect.x + 16, boxRect.y + 86, boxWidth - 32, 36),
            "<color=#AAAAAA>[E / Space] to interact</color>", labelStyle);
    }
}
