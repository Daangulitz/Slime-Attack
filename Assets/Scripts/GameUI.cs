using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Text roundText;
    public Text[] playerScoreTexts;

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        roundText.text = $"Round {GameManager.Instance.currentRound}/{GameManager.Instance.totalRounds}"; // gets the round amount and shows it at the top fo the screen

        for (int i = 0; i < GameManager.Instance.playerScores.Count; i++)
        {
            if (i < playerScoreTexts.Length)
            {
                playerScoreTexts[i].text = $"Player {i + 1}: {GameManager.Instance.playerScores[i]} Died"; // shows the amount of deaths that the player has
            }
        }
    }
}