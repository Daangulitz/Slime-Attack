using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameRoundManager : MonoBehaviour
{
    public List<PlayerHealth> players; // List of PlayerHealth components
    public static GameRoundManager Instance;

    private void Start()
    {
        // Find all PlayerHealth components in the scene
        players = new List<PlayerHealth>(FindObjectsOfType<PlayerHealth>());

        // Ensure that each player has a unique PlayerID
        for (int i = 0; i < players.Count; i++)
        {
            players[i].PlayerID = i;  // Assign a unique ID to each player
        }

        Debug.Log($"Round {GameManager.Instance.currentRound} of {GameManager.Instance.totalRounds}");
    }

    public void CheckForRoundWinner()
{
    PlayerHealth survivingPlayer = null;

    // finds the player who is still alive to give the point to
    foreach (var player in players)
    {
        if (player.IsAlive)
        {
            survivingPlayer = player;
            break; // Found the surviving player
        }
    }

    
    if (survivingPlayer != null)
    {
        // Award points to the player(s) who died
        GameManager.Instance.AddScore(survivingPlayer.PlayerID, 1);
        Debug.Log($"Surviving Player: {survivingPlayer.gameObject.name} awarded 1 point.");
    }
    else
    {
        Debug.LogWarning("No surviving player found! Something went wrong.");
    }

    // After awarding points, check if the game is over
    if (GameManager.Instance.IsGameOver())
    {
        // Proceed to results or end the game
        SceneManager.LoadScene("ResultsScene");
    }
    else
    {
        // Proceed to the next round
        GameManager.Instance.NextRound();
        SceneManager.LoadScene("GameScene");
    }
}


    private void PlayerWins(PlayerHealth winner)
    {
        // Use a unique ID to find the winner's index in GameManager
        int winnerIndex = winner.PlayerID; // PlayerID is a new property in PlayerHealth

        if (winnerIndex < 0 || winnerIndex >= GameManager.Instance.playerScores.Count)
        {
            Debug.LogError("Winner's index is invalid!");
            return;
        }

        Debug.Log($"{winner.gameObject.name} wins the round!");

        // Assign the score immediately
        GameManager.Instance.AddScore(winnerIndex, 1);

        Debug.Log($"Player {winnerIndex} score updated: {GameManager.Instance.playerScores[winnerIndex]}");

        // Proceed to the next round after a delay so the animations can play
        StartCoroutine(GoToNextRoundAfterDelay(3f));
    }

    private IEnumerator GoToNextRoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (GameManager.Instance.IsGameOver())
        {
            // loads to results scene if there is no rounds any more
            SceneManager.LoadScene("ResultsScene");
        }
        else
        {
            // sets the new round up
            GameManager.Instance.NextRound();
            SceneManager.LoadScene("GameScene");
        }
    }
}
