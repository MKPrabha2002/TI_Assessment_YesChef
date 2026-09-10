using UnityEngine;

public static class ScoreCalculator
{
    /// <summary>
    /// Calculates the final score of an order by deducting elapsed time from its base value.
    /// Uses Mathf.FloorToInt to strictly round down time. Result can be negative.
    /// </summary>
    public static int CalculateFinalScore(OrderData completedOrder)
    {
        float elapsedTime = Time.time - completedOrder.TimeCreated;
        
        // Strict round down penalty
        int timePenalty = Mathf.FloorToInt(elapsedTime);
        
        int finalScore = completedOrder.BaseScoreValue - timePenalty;
        return finalScore;
    }
}
