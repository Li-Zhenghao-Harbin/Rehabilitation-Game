using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Players : Base
{

    // GameObject
    Text[] TxPlayerHP = new Text[playerCount];
    Text[] TxPlayerGP = new Text[playerCount];
    Image[] ImgPlayerHP = new Image[playerCount];
    Image[] ImgPlayerGP = new Image[playerCount];
    // Player's HP and GP
    public static int[] hp = new int[playerCount];
    public static int[] gp = new int[playerCount];
    public static int[] maxHp = new int[playerCount];
    public const int maxGp = 50;
    // HP and GP bar layout
    const int maxBarWidth = 130;
    const int maxBossBarWidth = 875;
    const int barHeight = 10;
    const int barPadding = 10;
    const int hpBarY = 0;
    const int gpBarY = -35;
    // Push
    const float pushDistance = 2f;
    // Class
    GameController gameController;
    Cards cards;
    CardItems cardItems;
    Status status;
    // Statics
    public static float[] playerMoveDistance = new float[playerCount];

    public enum MoveDirection
    {
        UP = 0,
        DOWN = 1,
        LEFT = 2,
        RIGHT = 3
    };

    // Use this for initialization
    void Start()
    {
        // Set GameObjects
        TxPlayerHP[player1] = GameObject.Find("PnPlayer1/TxPlayerHP").GetComponent<Text>();
        TxPlayerGP[player1] = GameObject.Find("PnPlayer1/TxPlayerGP").GetComponent<Text>();
        ImgPlayerHP[player1] = GameObject.Find("PnPlayer1/ImgPlayerHP").GetComponent<Image>();
        ImgPlayerGP[player1] = GameObject.Find("PnPlayer1/ImgPlayerGP").GetComponent<Image>();
        TxPlayerHP[player2] = GameObject.Find("PnPlayer2/TxPlayerHP").GetComponent<Text>();
        TxPlayerGP[player2] = GameObject.Find("PnPlayer2/TxPlayerGP").GetComponent<Text>();
        ImgPlayerHP[player2] = GameObject.Find("PnPlayer2/ImgPlayerHP").GetComponent<Image>();
        ImgPlayerGP[player2] = GameObject.Find("PnPlayer2/ImgPlayerGP").GetComponent<Image>();
        // Set class
        gameController = gameObject.GetComponent<GameController>();
        cards = gameObject.GetComponent<Cards>();
        cardItems = gameObject.GetComponent<CardItems>();
        status = gameObject.GetComponent<Status>();
    }

    public void SetHP(int player, int value)
    {
        hp[player] = value;
    }

    public void SetGP(int player, int value)
    {
        gp[player] = value;
    }

    public void SetMaxHp(int player, int value)
    {
        maxHp[player] = value;
    }

    public int GetHP(int player)
    {
        return hp[player];
    }

    public int GetGP(int player)
    {
        return gp[player];
    }

    public void ChangeMaxHp(int player, int value)
    {
        maxHp[player] += value;
        TxPlayerHP[player].text = "HP " + hp[player] + " / " + maxHp[player];
    }

    // Update HP
    public void ChangeHP(int player, int value)
    {
        hp[player] += value;
        // Fix value
        if (hp[player] > maxHp[player])
        {
            hp[player] = maxHp[player];
        }
        else if (hp[player] <= 0)
        {
            hp[player] = 0;
        }
        TxPlayerHP[player].text = "HP " + hp[player] + " / " + maxHp[player];
        UpdateHPBar(player);
        gameController.UpdateLog(player, "HP" + (value >= 0 ? "+" : "") + value);
    }

    private void UpdateHPBar(int player)
    {
        // Set size of HP bar
        int barWidth = ((IsBoss() && bossTitle != GetBossTitle(BossTitle.TUTORIAL) && player == player2) ? maxBossBarWidth : maxBarWidth) * hp[player] / maxHp[player];
        RectTransform rectTransformImgPlayerHP = ImgPlayerHP[player].GetComponent<RectTransform>();
        rectTransformImgPlayerHP.sizeDelta = new Vector2(barWidth, barHeight);
        // Set position of HP bar
        rectTransformImgPlayerHP.SetParent(rectTransformImgPlayerHP);
        rectTransformImgPlayerHP.anchoredPosition = new Vector3((player == player1 ? 1 : -1) * (barPadding + (barWidth >> 1)), hpBarY, 0);
    }

    // Update GP
    public void ChangeGP(int player, int value)
    {
        gp[player] += value;
        // Fix value
        if (gp[player] < 0)
        {
            gp[player] = 0;
        }
        else if (gp[player] >= maxGp)
        {
            gp[player] = maxGp;
        }
        TxPlayerGP[player].text = "GP " + gp[player] + " / " + maxGp;
        UpdateGPBar(player);
        gameController.UpdateLog(player, "GP" + (value >= 0 ? "+" : "") + value);
    }

    // Update GP bar
    private void UpdateGPBar(int player)
    {
        // Set size of GP bar
        int barWidth = maxBarWidth * gp[player] / maxGp;
        RectTransform rectTransformImgPlayerGP = ImgPlayerGP[player].GetComponent<RectTransform>();
        rectTransformImgPlayerGP.sizeDelta = new Vector2(barWidth, barHeight);
        // Set position of GP bar
        rectTransformImgPlayerGP.SetParent(rectTransformImgPlayerGP);
        rectTransformImgPlayerGP.anchoredPosition = new Vector3((player == player1 ? 1 : -1) * (barPadding + (barWidth >> 1)), gpBarY, 0);
    }

    // Common attack
    public void Attack(int attacker, int damage)
    {
        int opponentAttacker = GetOpponent(attacker);
        // Status-Invincible to avoid attack
        if (status.IsInvincible(opponentAttacker))
        {
            return;
        }
        // Status-Blur to avoid attack
        if (status.GetBlur(opponentAttacker))
        {
            status.SetBlur(opponentAttacker, false);
            if (Random.Range(1, 5) < 2)
            {
                return;
            }
        }
        ChangeHP(opponentAttacker, -(damage + status.GetLevel(attacker)));
    }

    // Get opponent player
    private int GetOpponent(int player)
    {
        return player == player1 ? player2 : player1;
    }

    // Player moves
    public void Move(int player, MoveDirection moveDirection)
    {
        if (status.IsFreeze(player))
        {
            return;
        }
        switch (moveDirection)
        {
            //case MoveDirection.LEFT:
            //    //game.Player[player].transform.Translate(-speed[player] * Time.deltaTime, 0, 0, Space.Self);
            //    gameController.Player[player].transform.Translate(0, 0, playerMovingSpeed[player] * Time.deltaTime, Space.Self);
            //    break;
            //case MoveDirection.RIGHT:
            //    //game.Player[player].transform.Translate(speed[player] * Time.deltaTime, 0, 0, Space.Self);
            //    gameController.Player[player].transform.Translate(0, 0, -playerMovingSpeed[player] * Time.deltaTime, Space.Self);
            //    break;
            //case MoveDirection.UP:
            //    //game.Player[player].transform.Translate(0, 0, speed[player] * Time.deltaTime, Space.Self);
            //    gameController.Player[player].transform.Translate(playerMovingSpeed[player] * Time.deltaTime, 0, 0, Space.Self);
            //    break;
            //case MoveDirection.DOWN:
            //    //game.Player[player].transform.Translate(0, 0, -speed[player] * Time.deltaTime, Space.Self);
            //    gameController.Player[player].transform.Translate(-playerMovingSpeed[player] * Time.deltaTime, 0, 0, Space.Self);
            //    break;
            case MoveDirection.LEFT:
                gameController.Player[player].transform.eulerAngles = new Vector3(0, 0, 0);
                gameController.Player[player].transform.Translate(-playerMovingSpeed[player] * Time.deltaTime, 0, 0, Space.Self);
                break;
            case MoveDirection.RIGHT:
                gameController.Player[player].transform.eulerAngles = new Vector3(0, -180, 0);
                gameController.Player[player].transform.Translate(-playerMovingSpeed[player] * Time.deltaTime, 0, 0, Space.Self);
                break;
            case MoveDirection.UP:
                gameController.Player[player].transform.eulerAngles = new Vector3(0, -270, 0);
                gameController.Player[player].transform.Translate(-playerMovingSpeed[player] * Time.deltaTime, 0, 0, Space.Self);
                break;
            case MoveDirection.DOWN:
                gameController.Player[player].transform.eulerAngles = new Vector3(0, -90, 0);
                gameController.Player[player].transform.Translate(-playerMovingSpeed[player] * Time.deltaTime, 0, 0, Space.Self);
                break;
        }
        playerMoveDistance[player] += 0.01f;
    }

    // Push the opponent
    public void Push(int player)
    {
        int opponentPlayer = GetOpponent(player);
        var p = gameController.Player[player].transform.position;
        var op = gameController.Player[opponentPlayer].transform.position;
        if (p.x > op.x)
        {
            op.x -= pushDistance;
            op.z = p.z < op.z ? op.z + pushDistance : op.z - pushDistance;
        }
        else
        {
            op.x += pushDistance;
            op.z = p.z < op.z ? op.z + pushDistance : op.z - pushDistance;
        }
        gameController.Player[opponentPlayer].transform.position = op;
    }

    // Check whether two GameObjects meets or not
    public bool Meet(GameObject gameObject1, GameObject gameObject2)
    {
        float minDistance = 1f;
        return Mathf.Abs(gameObject1.transform.position.x - gameObject2.transform.position.x) <= minDistance
            && Mathf.Abs(gameObject1.transform.position.z - gameObject2.transform.position.z) <= minDistance
            && Mathf.Abs(gameObject1.transform.position.y - gameObject2.transform.position.y) <= minDistance;
    }

    // Search cards when moving
    private void SearchCards(int player)
    {
        for (int i = 0; i < CardItems.maxCardItemCount; i++)
        {
            if (Meet(gameController.Player[player], cardItems.cardItem[i]))
            {
                cardItems.InactiveCardItem(i);
                cards.FindCard(player);
                gameController.UpdateLog(player, "Find a card");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        SearchCards(player1);
        SearchCards(player2);
    }
}
