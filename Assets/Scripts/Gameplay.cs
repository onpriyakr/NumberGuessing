using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Gameplay : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI attempsLeft;
    public TextMeshProUGUI currentPlayer;
    public TextMeshProUGUI gameState;
    public TextMeshProUGUI gameLog;

    [Header("input")]
    public TMP_InputField guessInputField;
    public Button submitButton;
    public Button newgameButton;

    [Header("Game Settings")]
    public int minNumber = 1;
    public int maxNumber = 100;
    public int maxAttempts = 12;

    private int targetNumber;
    private int currentAttemps;
    private bool isPlayerTurn;
    private bool gameActive;

    void InitializedUI()
    {
        submitButton.onClick.AddListener(SubmitGuess);
        newgameButton.onClick.AddListener(StartNewGame);
        guessInputField.onSubmit.AddListener(delegate { SubmitGuess(); });
    }

    void SubmitGuess()
    {
        if (!gameActive || !isPlayerTurn) return;

        string input = guessInputField.text.Trim();
        if (string.IsNullOrEmpty(input)) return;

        int guess;
        if (!int.TryParse(input, out guess))
        {
            gameState.text = "Please enter a valid number.";
            return;
        }
        if (guess < minNumber || guess > maxNumber)
        {
            gameState.text = $"Please enter a number between {minNumber} - {maxNumber}.";
            return;
        }

        ProgressGuess(guess, true);
        guessInputField.text = "";
    }

    void ProgressGuess(int guess,bool isPlayerTurn)
    {
        currentAttemps++;
        string playerName = isPlayerTurn ? "Player" : "Computer";

        gameLog.text += $"{playerName} guessed: {guess}\n";

        if (guess == targetNumber)
        {
            //Win
            gameLog.text += $"{playerName} wins! The number was.\n";
            EndGame();
        }
        else if (currentAttemps >= maxAttempts)
        {
            //Lose
            gameLog.text += $"Gameover! The correct number was {targetNumber}.\n";
            EndGame();
        }
        else
        {
            //Wrong guess - give hint
            string hint = guess < targetNumber ? "Too Low" : "Too High";
            gameLog.text += $"{hint}!\n";

            //Switch players
            isPlayerTurn = !isPlayerTurn;
            currentPlayer.text = isPlayerTurn ? "Player" : "Computer";
            attempsLeft.text = $"Attempts Left: {maxAttempts - currentAttemps}";

            if (!isPlayerTurn)
            {
                guessInputField.interactable = false;
                submitButton.interactable = false;
                StartCoroutine(ComputerTurn(guess < targetNumber));
            }
            else
            {
                guessInputField.interactable = true;
                submitButton.interactable = true;
                guessInputField.Select();
                guessInputField.ActivateInputField();
            }
        }
    }

    IEnumerator ComputerTurn(bool targetIsHigher)
    {
        yield return new WaitForSeconds(2f); // Wait to simulate thinking
        if (!gameActive) yield break;
        int computerGuess = Random.Range(minNumber, maxNumber + 1);
        ProgressGuess(computerGuess, false);
    } 

    void EndGame()
    {
        gameActive = false;
        guessInputField.interactable = false;
        submitButton.interactable = false;
        currentPlayer.text = "";
        gameState.text = "Game Over! Press 'New Game' to play again.";
        Canvas.ForceUpdateCanvases();
    }

    void StartNewGame()
    {
        targetNumber = Random.Range(minNumber, maxNumber + 1);
        currentAttemps = 0;
        isPlayerTurn = true;
        gameActive = true;

        currentPlayer.text = "Player's Turn";
        attempsLeft.text = $"Attempts Left: {maxAttempts}";
        gameLog.text = "=== Game Log ===\n";
        gameState.text = "New game started! Player goes first";

        guessInputField.interactable = true;
        submitButton.interactable = true;
        guessInputField.text = "";
        guessInputField.Select();
        guessInputField.ActivateInputField();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializedUI();
        StartNewGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
