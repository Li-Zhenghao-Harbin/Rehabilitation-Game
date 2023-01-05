using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : Base
{
    // GameObjects
    public GameObject[] Player = new GameObject[2];
    public Text[] TxPlayerLog = new Text[2];
    Button BtnMenu;
    Text TxTimer;
    GameObject PnMenu;
    GameObject PnGameOver;
    // Players
    const float dropY = -5f;
    // Class
    Players players;
    Cards cards;
    Shapes shapes;
    CardItems cardItems;
    // Timer
    const float activeCardItemTimerInitialValue = 5f;
    float activeCardItemTimer = 1f;
    // Log
    const int maxLogLength = 300;
    // Timer
    int minutes = 0;
    float seconds = 0f;
    // Menu
    bool isShownMenu = false;

    // Use this for initialization
    void Start()
    {
        // Set game speed
        Time.timeScale = gameSpeed;
        // Set GameObjects
        Player[player1] = GameObject.Find("Player1");
        Player[player2] = GameObject.Find("Player2");
        TxPlayerLog[player1] = GameObject.Find("TxPlayer1Log").GetComponent<Text>();
        TxPlayerLog[player2] = GameObject.Find("TxPlayer2Log").GetComponent<Text>();
        BtnMenu = GameObject.Find("BtnMenu").GetComponent<Button>();
        TxTimer = GameObject.Find("TxTimer").GetComponent<Text>();
        // Set players initial HP and GP
        players = gameObject.GetComponent<Players>();
        players.SetMaxHp(player1, 20);
        players.SetMaxHp(player2, 20);
        players.SetHP(player1, 20);
        players.SetHP(player2, 20);
        players.SetGP(player1, 0);
        players.SetGP(player2, 0);
        // Set class
        cards = gameObject.GetComponent<Cards>();
        shapes = gameObject.GetComponent<Shapes>();
        cardItems = gameObject.GetComponent<CardItems>();
        // Set onclick event
        BtnMenu.onClick.AddListener(BtnMenuOnClick);
        // Menu
        PnMenu = GameObject.Find("PnMenu");
        PnMenu.SetActive(false);
        PnGameOver = GameObject.Find("PnGameOver");
        PnGameOver.SetActive(false);
    }

    private void BtnMenuOnClick()
    {
        Time.timeScale = (isShownMenu = !isShownMenu) ? 0f : gameSpeed;
        PnMenu.SetActive(isShownMenu);
        if (isShownMenu)
        {
            GameObject.Find("PnMenu/BtnResume").GetComponent<Button>().onClick.AddListener(BtnResumeOnClick);
            GameObject.Find("PnMenu/BtnReStart").GetComponent<Button>().onClick.AddListener(BtnReStartOnClick);
            GameObject.Find("PnMenu/BtnExit").GetComponent<Button>().onClick.AddListener(BtnExitOnClick);
        }
    }

    private void BtnResumeOnClick()
    {
        isShownMenu = true;
        BtnMenuOnClick();
    }

    private void BtnReStartOnClick()
    {
        SceneManager.LoadScene("Game");
    }

    private void BtnExitOnClick()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Update log
    public void UpdateLog(int player, string str)
    {
        if (!showLog)
        {
            return;
        }
        if (TxPlayerLog[player].text.Length > maxLogLength)
        {
            TxPlayerLog[player].text = "...\n";
        }
        TxPlayerLog[player].text += str + "\n";
    }

    // Update Timer
    private void UpdateTimer()
    {
        seconds += Time.deltaTime;
        if (seconds >= 60f)
        {
            minutes++;
            seconds = 0;
        }
        TxTimer.text = minutes.ToString() + " : " + ((int)seconds).ToString();
    }

    private string GetPlayerStatics(int player)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Player");
        sb.AppendLine((player + 1).ToString());
        sb.AppendLine();
        sb.Append("Found Cards\t");
        sb.AppendLine(Cards.playerFoundCardsCount[player].ToString());
        sb.Append("Used Cards\t\t");
        sb.AppendLine(Cards.playerUsedCardsCount[player].ToString());
        sb.Append("Drew Shapes\t");
        sb.AppendLine(Shapes.playerDrewShapesCount[player].ToString());
        return sb.ToString();
    }

    private void GameOver(int winner)
    {
        PnGameOver.SetActive(true);
        GameObject.Find("PnGameOver/TxTitle").GetComponent<Text>().text = "Winner - " + (winner == player1 ? "Player1" : "Player2");
        GameObject.Find("PnGameOver/TxPlayer1Statics").GetComponent<Text>().text = GetPlayerStatics(player1);
        GameObject.Find("PnGameOver/TxPlayer2Statics").GetComponent<Text>().text = GetPlayerStatics(player2);
        GameObject.Find("PnGameOver/BtnReStart").GetComponent<Button>().onClick.AddListener(BtnReStartOnClick);
        GameObject.Find("PnGameOver/BtnExit").GetComponent<Button>().onClick.AddListener(BtnExitOnClick);
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Timer
        UpdateTimer();
        // Game over
        if (players.GetHP(player1) == 0 || players.GetGP(player2) == Players.maxGp || Player[player1].transform.position.y < dropY)
        {
            GameOver(player2);
        }
        if (players.GetHP(player2) == 0 || players.GetGP(player1) == Players.maxGp || Player[player2].transform.position.y < dropY)
        {
            GameOver(player1);
        }
        // Player1 moves
        if (Input.GetKey(KeyCode.A))
        {
            players.Move(player1, Players.MoveDirection.LEFT);
        }
        if (Input.GetKey(KeyCode.D))
        {
            players.Move(player1, Players.MoveDirection.RIGHT);
        }
        if (Input.GetKey(KeyCode.W))
        {
            players.Move(player1, Players.MoveDirection.UP);
        }
        if (Input.GetKey(KeyCode.S))
        {
            players.Move(player1, Players.MoveDirection.DOWN);
        }
        // Player2 moves
        if (Input.GetKey(KeyCode.J))
        {
            players.Move(player2, Players.MoveDirection.LEFT);
        }
        if (Input.GetKey(KeyCode.L))
        {
            players.Move(player2, Players.MoveDirection.RIGHT);
        }
        if (Input.GetKey(KeyCode.I))
        {
            players.Move(player2, Players.MoveDirection.UP);
        }
        if (Input.GetKey(KeyCode.K))
        {
            players.Move(player2, Players.MoveDirection.DOWN);
        }
        // Player1 draws
        if (Input.GetKeyDown(KeyCode.E))
        {
            shapes.AppendShape(player1, Shapes.TargetShape.SQUARE);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            shapes.AppendShape(player1, Shapes.TargetShape.UPPER_TRIANGLE);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            shapes.AppendShape(player1, Shapes.TargetShape.LOWER_TRIANGLE);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            cards.RemoveShapes(player1);
        }
        // Player2 draws
        if (Input.GetKeyDown(KeyCode.O))
        {
            shapes.AppendShape(player2, Shapes.TargetShape.SQUARE);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            shapes.AppendShape(player2, Shapes.TargetShape.UPPER_TRIANGLE);
        }
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            shapes.AppendShape(player2, Shapes.TargetShape.LOWER_TRIANGLE);
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            cards.RemoveShapes(player2);
        }
        // Generate CardItem
        if (activeCardItemTimer > 0)
        {
            activeCardItemTimer -= Time.deltaTime;
        }
        else
        {
            cardItems.ActiveCardItem();
            activeCardItemTimer = activeCardItemTimerInitialValue;
        }
    }
}
