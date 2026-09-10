using System.Collections;
using UnityEngine;
using TMPro;

public class FloatingScore : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float floatSpeed = 0.5f;
    [SerializeField] private float fadeDuration = 2.0f;

    private TextMeshProUGUI scoreText;

    private void Awake()
    {
        // Require a TextMeshProUGUI component attached somewhere on this prefab or its children
        scoreText = GetComponentInChildren<TextMeshProUGUI>();
        if (scoreText == null)
        {
            Debug.LogError("[FloatingScore] Missing TextMeshProUGUI component!");
        }
    }

    /// <summary>
    /// Injects the final score value, formats it, and triggers the animation.
    /// </summary>
    public void SetScore(int score)
    {
        if (scoreText != null)
        {
            // Explicitly format positive numbers with a plus sign
            if (score > 0)
            {
                scoreText.text = "+" + score.ToString();
            }
            else
            {
                // Negative numbers naturally include a minus sign
                scoreText.text = score.ToString();
            }
        }

        StartCoroutine(AnimateAndDestroy());
    }

    private IEnumerator AnimateAndDestroy()
    {
        float timer = 0f;
        Color startColor = scoreText != null ? scoreText.color : Color.white;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            
            // Float upward continuously
            transform.position += Vector3.up * floatSpeed * Time.deltaTime;

            // Fade alpha
            if (scoreText != null)
            {
                float normalizedTime = timer / fadeDuration;
                scoreText.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(1f, 0f, normalizedTime));
            }

            yield return null;
        }

        // Cleanup self to prevent memory leaks
        Destroy(gameObject);
    }
}
