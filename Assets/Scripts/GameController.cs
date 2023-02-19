using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : Base
{
    // GameObjects
    public GameObject[] Player = new GameObject[playerCount];
    public Text[] TxPlayerLog = new Text[playerCount];
    Button BtnMenu;
    Text TxTimer;
    Text TxController;
    GameObject PnMenu;
    GameObject PnGameOver;
    // Players
    const float dropY = -5f; // player lose the game if y under the value
    // Class
    Players players;
    Cards cards;
    Shapes shapes;
    CardItems cardItems;
    Status status;
    // Timer
    const float initActiveCardItemTimer = 5f;
    float activeCardItemTimer = 1f;
    // Log
    const int maxLogLength = 300;
    // Menu
    bool isShownMenu = false;
    // Data
    string gameDataPath; // game data path
    bool gameDataSaved = false;
    // Game
    public static bool end = false;

    // Use this for initialization
    void Start()
    {
        end = false;
        // Data
        gameDataPath = Application.dataPath + "/Data/data.txt";
        // Set time
        minutes = 0;
        seconds = 0f;
        // Set game speed
        Time.timeScale = gameSpeed;
        // Set GameObjects
        Player[player1] = GameObject.Find("Player1");
        Player[player2] = GameObject.Find("Player2");
        TxPlayerLog[player1] = GameObject.Find("TxPlayer1Log").GetComponent<Text>();
        TxPlayerLog[player2] = GameObject.Find("TxPlayer2Log").GetComponent<Text>();
        BtnMenu = GameObject.Find("BtnMenu").GetComponent<Button>();
        TxTimer = GameObject.Find("TxTimer").GetComponent<Text>();
        TxController = GameObject.Find("TxController").GetComponent<Text>();
        TxController.text = "Controller - " + (gameControl == GetGameControl(GameControl.KEYBOARD) ? "Keyboard" : "Sensor");
        // Set class
        players = gameObject.GetComponent<Players>();
        cards = gameObject.GetComponent<Cards>();
        shapes = gameObject.GetComponent<Shapes>();
        cardItems = gameObject.GetComponent<CardItems>();
        status = gameObject.GetComponent<Status>();
        // Reset status
        status.ResetStatus();
        // Set players initial HP and GP
        players.SetMaxHp(player1, 20);
        players.SetMaxHp(player2, IsBoss() ? 50 : 20);
        players.SetHP(player1, 20);
        players.SetHP(player2, IsBoss() ? 50 : 20);
        players.SetGP(player1, 0);
        players.SetGP(player2, 0);
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
        if (bossTitle == GetBossTitle(BossTitle.TUTORIAL))
        {
            SetVisible(GameObject.Find("PnTutorial"), false);
        }
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
        if (gamePlayer == GetGamePlayer(GamePlayer.DOUBLE))
        {
            SceneManager.LoadScene("Game");
        }
        else
        {
            if (bossTitle == GetBossTitle(BossTitle.TUTORIAL))
            {
                SceneManager.LoadScene("Tutorial");
            }
            else if (bossTitle == GetBossTitle(BossTitle.MEDUSA))
            {
                SceneManager.LoadScene("Medusa");
            }
        }
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
        sb.Append("Found Cards\t\t");
        sb.AppendLine(Cards.playerFoundCardsCount[player].ToString());
        sb.Append("Used Cards\t\t");
        sb.AppendLine(Cards.playerUsedCardsCount[player].ToString());
        sb.Append("Drew Shapes\t\t");
        sb.AppendLine(Shapes.playerDrewShapesCount[player].ToString());
        sb.Append("Moved Distance\t\t");
        sb.AppendLine(Players.playerMoveDistance[player].ToString(playerMoveDistanceReserveDigits));
        return sb.ToString();
    }

    private void SaveData(int winner)
    {
        try
        {
            StreamWriter streamWriter;
            FileInfo fileInfo = new FileInfo(gameDataPath);
            if (!File.Exists(gameDataPath))
            {
                streamWriter = fileInfo.CreateText();
            }
            else
            {
                streamWriter = fileInfo.AppendText();
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.ToLocalTime().ToShortDateString()); sb.Append(gameDataSpliter);
            sb.Append(minutes * 60 + (int)seconds); sb.Append(gameDataSpliter);
            sb.Append(gamePlayer); sb.Append(gameDataSpliter);
            sb.Append(gameControl); sb.Append(gameDataSpliter);
            sb.Append(IsBoss() ? bossTitle : -1); sb.Append(gameDataSpliter);
            sb.Append(Cards.playerFoundCardsCount[player1]); sb.Append(gameDataSpliter);
            sb.Append(gamePlayer == GetGamePlayer(GamePlayer.DOUBLE) ? Cards.playerFoundCardsCount[player2] : -1); sb.Append(gameDataSpliter);
            sb.Append(Cards.playerUsedCardsCount[player1]); sb.Append(gameDataSpliter);
            sb.Append(gamePlayer == GetGamePlayer(GamePlayer.DOUBLE) ? Cards.playerUsedCardsCount[player2] : -1); sb.Append(gameDataSpliter);
            sb.Append(Shapes.playerDrewShapesCount[player1]); sb.Append(gameDataSpliter);
            sb.Append(gamePlayer == GetGamePlayer(GamePlayer.DOUBLE) ? Shapes.playerDrewShapesCount[player2] : -1); sb.Append(gameDataSpliter);
            sb.Append(Players.playerMoveDistance[player1].ToString(playerMoveDistanceReserveDigits)); sb.Append(gameDataSpliter);
            sb.Append(gamePlayer == GetGamePlayer(GamePlayer.DOUBLE) ? Players.playerMoveDistance[player2] : -1); sb.Append(gameDataSpliter);
            sb.Append(winner);
            streamWriter.WriteLine(sb.ToString());
            streamWriter.Close();
            streamWriter.Dispose();
        }
        catch
        {

        }
        gameDataSaved = true;
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
        if (!gameDataSaved)
        {
            SaveData(winner);
        }
        end = true;
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
        if (players.GetHP(player2) == 0 || (!IsBoss() && players.GetGP(player1) == Players.maxGp) || Player[player2].transform.position.y < dropY)
        {
            GameOver(player1);
        }
        if (end)
        {
            return;
        }
        // Controller
        if (gameControl == GetGameControl(GameControl.KEYBOARD)) // Keyboard
        {
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
            if (!IsBoss())
            {
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
            if (!IsBoss())
            {
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
            }
        }
        // Generate CardItem
        if (activeCardItemTimer > 0)
        {
            activeCardItemTimer -= Time.deltaTime;
        }
        else
        {
            cardItems.ActiveCardItem(IsBoss() ? 3 : 5);
            activeCardItemTimer = initActiveCardItemTimer;
        }
    }
}
