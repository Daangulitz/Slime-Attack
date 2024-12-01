using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq; // Include LINQ to help with sorting

public class Resultsscript : MonoBehaviour
{
    public GameObject resultsPanel; // Parent panel for the results (this is now a regular GameObject)
    public GameObject playerResultPrefab; // Prefab with Image and Text
    public List<Sprite> playerSprites; // List of player sprites assigned in the Inspector
    public float spacing = 125f; // Vertical spacing between each result

    private void Start()
    {
        DisplayResults();
    }

    private void DisplayResults()
    {
        Vector3 startPosition = resultsPanel.transform.position / 2.5f; // Starting position for results
        startPosition.y += 250f; // Adjust starting Y position to give a small margin from top (optional)

        // Sort player scores and their corresponding sprites in ascending order (least deaths first)
        var sortedScores = GameManager.Instance.playerScores
            .Select((score, index) => new { score, index }) // Pair each score with its player index
            .OrderBy(x => x.score) // Sort by score (deaths)
            .ToList();

        for (int i = 0; i < sortedScores.Count; i++)
        {
            GameObject resultItem = Instantiate(playerResultPrefab, resultsPanel.transform);

            // Set the position of the result item (move it 25 pixels below the previous one)
            resultItem.transform.position = startPosition + new Vector3(-50, -i * spacing, 0);

            // Set the player's sprite
            resultItem.transform.Find("PlayerImage").GetComponent<Image>().sprite = playerSprites[sortedScores[i].index];

            // Set the player's score (deaths)
            resultItem.transform.Find("PlayerScoreText").GetComponent<Text>().text = $"{sortedScores[i].score} Deaths";
        }
    }

    public void BackToSetup()
    {
        SceneManager.LoadScene("SetupScene"); // go's to the setup scene for new amount of players or rounds
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene"); // go's back to the main menu
    }
}
