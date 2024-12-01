using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int totalRounds;  // Total number of rounds
    public int currentRound; // Tracks the current round
    public int playerCount;  // Number of players
    public List<int> playerScores; // List of player scores

   // makes sure that there's always a GameManager in every scene
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }
    

    // sets the counts for the game
    public void InitializeGame(int numPlayers, int numRounds)
    {
        playerCount = numPlayers; // sets amount of players
        totalRounds = numRounds; // sets amount of rounds
        currentRound = 1; // starts at round 1

        // Initialize player scores
        playerScores = new List<int>(new int[playerCount]);
        
    }
    
    // Add's the deaths to the right player
    public void AddScore(int winnerIndex, int score)
    {
        if (winnerIndex < 0 || winnerIndex >= playerScores.Count) // finds if there is a player to give the point to
        {
            Debug.LogError($"Invalid winner index: {winnerIndex}");
            return;
        }

        playerScores[winnerIndex] += score; // adds the point to the player score
    }

    public bool IsGameOver()
    {
        return currentRound >= totalRounds;  // The game is over if we have reached the total rounds
    }

    public void NextRound()
    {
        currentRound++; // do the current round by 1 up if the total rounds is not reached
    }
}