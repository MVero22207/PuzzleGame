using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Transform gameTransform;
    [SerializeField]
    private Transform[] piecePrefab;

    // UI stuff
    public TextMeshProUGUI counterTxt;
    public GameObject levelCompleteUI;
    public GameObject mainScreenUI;
    public GameObject backToMainMenuBtn;

    private List<Transform> pieces;

    private int emptyLocation;
    public int size;
    public GameState gameState;
    private int level = 0;



    //Destroys the GameObjects of the puzzle if they exist.
    private void CleanUp()
    {
        if (gameTransform.childCount > 0)
        {
            for (int i = 0; i < gameTransform.childCount; i++)
            {
                Transform child = gameTransform.GetChild(i);
                Destroy(child.gameObject);
            }
        }
    }

    private void CreateGamePieces(float gapThickness)
    {
        CleanUp();

        //Code to generate the pieces in a square based on size.
        float width = 1 / (float)size;
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                Transform piece = Instantiate(piecePrefab[level], gameTransform);
                pieces.Add(piece);
                piece.localPosition = new Vector3(-1 + (2 * width * col) + width,
                                +1 - (2 * width * row) - width,
                                0);
                piece.localScale = ((2 * width) - gapThickness) * Vector3.one;
                piece.name = $"{(row * size) + col}";

                if ((row == size - 1) && (col == size - 1))
                {
                    emptyLocation = (size * size) - 1;
                    piece.gameObject.SetActive(false);
                }
                else
                {
                    float gap = gapThickness / 8;
                    Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                    Vector2[] uv = new Vector2[4];
                    uv[0] = new Vector2((width * col) + gap, 1 - ((width * (row + 1)) - gap));
                    uv[1] = new Vector2((width * (col + 1)) + gap, 1 - ((width * (row + 1)) - gap));
                    uv[2] = new Vector2((width * col) + gap, 1 - ((width * row) + gap));
                    uv[3] = new Vector2((width * (col + 1)) - gap, 1 - ((width * row) + gap));
                    mesh.uv = uv;
                }

            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.Menu;
        levelCompleteUI.SetActive(false);
        counterTxt.gameObject.SetActive(false);
        backToMainMenuBtn.SetActive(false);     
    }

    // starts the level.
    public void StartLevel(int level)
    {
        pieces = new List<Transform>();
        levelCompleteUI.SetActive(false);
        mainScreenUI.SetActive(false);
        this.level = level;  
        CreateGamePieces(0.01f);
        gameState = GameState.Start;

    }

    // Update is called once per frame
    void Update()
    {

        if (gameState.Equals(GameState.Menu))
        {
            mainScreenUI.SetActive(true);
            backToMainMenuBtn.SetActive(false);
            return;
        }
       

        if (gameState.Equals(GameState.Start))
        {
            StartCoroutine(WaitShuffle(4.0f));
        }

        if (gameState.Equals(GameState.FinishLevel))
        {
            levelCompleteUI.SetActive(true);
        }


        if (gameState.Equals(GameState.Playing))
        {
            backToMainMenuBtn.SetActive(true);
            counterTxt.gameObject.SetActive(false);
            levelCompleteUI.SetActive(false);

            // code below handles the clicks to move pieces and the check to see if the game is done.
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit)
                {
                    for (int i = 0; i < pieces.Count; i++)
                    {
                        if (pieces[i] == hit.transform)
                        {
                            if (SwapIfValid(i, -size, size)) { break; }
                            if (SwapIfValid(i, +size, size)) { break; }
                            if (SwapIfValid(i, -1, 0)) { break; }
                            if (SwapIfValid(i, +1, size - 1)) { break; }
                        }
                    }
                }
                if (CheckCompletion())
                {
                    // show UI to replay or exit.
                    gameState = GameState.FinishLevel;
                    levelCompleteUI.SetActive(true);


                }
            }
        }
    }


    //code that checks if the swap can actually happen.
    private bool SwapIfValid(int i, int offset, int colCheck)
    {
        if (((i % size) != colCheck) && ((i + offset) == emptyLocation))
        {
            (pieces[i], pieces[i + offset]) = (pieces[i + offset], pieces[i]);

            (pieces[i].localPosition, pieces[i + offset].localPosition) = ((pieces[i + offset].localPosition, pieces[i].localPosition));
            emptyLocation = i;
            return true;
        }
        return false;
    }
    
    private bool CheckCompletion()
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i].name != $"{i}")
            {
                return false;
            }
        }
        return true;
    }

    private IEnumerator WaitShuffle(float duration)
    {
        gameState = GameState.Shuffling;
        counterTxt.gameObject.SetActive(true);
        float timeLeft = duration;

        while (timeLeft > 0)
        {
            counterTxt.text = WaitForSecondsUnit(timeLeft);
            yield return new WaitForSeconds(1f); // Update every second
            timeLeft--;
        }

        counterTxt.text = "0"; // Ensure that the final text is displayed as 0
        counterTxt.gameObject.SetActive(false);
        Shuffle();
        gameState = GameState.Playing;
    }

    private string WaitForSecondsUnit(float seconds)
    {
        // Format the remaining time as needed (e.g., show seconds)
        return seconds.ToString("F0");
    }

    //shuffle the pieces of the puzzle.
    private void Shuffle()
    {
        int count = 0;
        int last = 0;
        while (count < (size * size * size))
        {
            int rnd = Random.Range(0, size * size);

            if (rnd == last) { continue; }

            last = emptyLocation;

            if (SwapIfValid(rnd, -size, size))
            {
                count++;
            } 
            else if (SwapIfValid(rnd, +size, size))
            {
                count++;
            }
            else if (SwapIfValid(rnd, -1, 0))
            {
                count++;
            }
            else if (SwapIfValid(rnd, +1, size - 1))
            {
                count++;
            }
        }
    }

    public void BackToMainMenu()
    {
        CleanUp();
        levelCompleteUI.SetActive(false);
        mainScreenUI.SetActive(true);
        gameState = GameState.Menu;
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    public enum GameState
    {
        Start,
        Shuffling,
        Playing,
        FinishLevel,
        Menu
    }
}
    