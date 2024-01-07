using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class LevelSelector : MonoBehaviour
{

    public GameObject rightBtn;
    public GameObject leftBtn;
    public GameObject playBtn;
    public GameObject changeDifficultyBtn;
    public Texture2D[] imageList;
    private int level = 0;
    public GameManager gameManager;
    public GameObject levelSelectorUI;
    public GameObject difficultyBtn;
    public Button difficultyBtnTMP;
    public Difficulty difficulty;
    public TextMeshProUGUI enumValueText;
    RawImage imageTexture;


    public GameObject image;


    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {

        difficulty = Difficulty.EASY;
        UpdateButtonVisuals();
        imageTexture = image.GetComponent<RawImage>();
        imageTexture.texture = imageList[0];
        level = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameManager.gameState.Equals(GameManager.GameState.Menu))
        {
            levelSelectorUI.SetActive(true);
        }

        if (gameManager.gameState.Equals(GameManager.GameState.Playing)
            || gameManager.gameState.Equals(GameManager.GameState.Shuffling))
        {
            levelSelectorUI.SetActive(false);
        }
    }

    public void NextLevel()
    {
        level++;
        if (level > imageList.Length - 1)
        {
            level = 0;
        }
        imageTexture.texture = imageList[level];
    }

    public void PreviousLevel()
    {
        level--;
        if (level < 0)
        {
            level = imageList.Length - 1;
        }
        imageTexture.texture = imageList[level];
    }

    public void StartLevel()
    {
        gameManager.size = (int)difficulty;
        gameManager.StartLevel(level);
    }

    public void SetDifficulty()
    {
        difficulty++;

        if (difficulty > Difficulty.EXTREME)
        {
            difficulty = Difficulty.EASY;
        }

        UpdateButtonVisuals();

    }



    private void UpdateButtonVisuals()
    {
        // Update the text
        enumValueText.text = difficulty.ToString();

        // Update the color of the button based on the currentDifficulty
        Color buttonColor = GetButtonColor(difficulty);
        UpdateButtonColor(buttonColor);
    }

    //Enum to decide what difficulty we want the puzzle to be.
    public enum Difficulty
    {
        EASY = 2,
        MEDIUM = 3,
        HARD = 4,
        EXTREME = 5
    }

    //Decide the color of the button based on the Difficulty
    private Color GetButtonColor(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.EASY:
                return Color.green;
            case Difficulty.MEDIUM:
                return Color.yellow;
            case Difficulty.HARD:
                return Color.red;
            case Difficulty.EXTREME:
                return Color.magenta;
            default:
                return Color.white;
        }
    }

    //Updates the value of Color in the button
    private void UpdateButtonColor(Color color)
    {
        // Access the Image component of the button and update its color
        Image buttonImage = difficultyBtnTMP.GetComponent<Image>();

        if (buttonImage != null)
        {
            buttonImage.color = color;
        }
    }
}
