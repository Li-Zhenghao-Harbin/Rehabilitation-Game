using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cards : Base
{

    // Cards
    const int maxCardsCount = 5;
    static int[,] playerCards = new int[playerCount, maxCardsCount];
    int[] playerCardsCount = new int[2];
    public const int maxCardIndex = 30;
    string[] cardsName = new string[]
    {
        "Strike",
        "Strike2",
        "Strike3",
        "Heal",
        "Refresh",
        "Duplicate",
        "LifeSteal",
        "Upgrade",
        "Sacrifice",
        "Blur",
        "Rob",
        "Freeze",
        "Push",
        "Infest",
        "Discover",
        "Mirror",
        "Ignite",
        "Counterspell",
        "Invincible",
        "Blast",
        "Vaporize",
        "Balance",
        "Avenge",
        "Curse",
        "Pride",
        "Shock",
        "Explode",
        "Treasure",
        "Rage",
        "Charge"
    };
    int[] cardsPoint = new int[]
    {
        3,
        4,
        5,
        3,
        3,
        1,
        3,
        1,
        6,
        1,
        1,
        1,
        2,
        1,
        2,
        1,
        3,
        2,
        3,
        3,
        2,
        0,
        2,
        3,
        3,
        3,
        3,
        7,
        2,
        2
    };
    string[] cardsShape = new string[]
    {
        "■",
        "■",
        "■",
        "▲",
        "▼▼▼",
        "▼▲",
        "■▲",
        "▲▲▲",
        "▼■■",
        "▲",
        "■■",
        "▲",
        "▲",
        "■",
        "▼▲",
        "▼▼▲",
        "■▲",
        "▼",
        "▲",
        "■▼",
        "▼▲▼",
        "▲▼▼",
        "▼",
        "▼■",
        "▼▲",
        "■",
        "■",
        "▲▲▼",
        "■■",
        "■▼"
    };
    int[] cardsColor = new int[]
    {
        0,
        1,
        2,
        1,
        2,
        2,
        2,
        3,
        1,
        0,
        3,
        0,
        0,
        1,
        1,
        2,
        1,
        1,
        1,
        2,
        3,
        1,
        0,
        3,
        3,
        1,
        1,
        2,
        2,
        1
    };
    const int arrangeCardY = 125; // 125
    const int arrangeCardCriticalValueY = 50;
    // GameObjects
    Image[,] ImgPlayerCards = new Image[playerCount, maxCardsCount];
    // Class
    Players players;
    Shapes shapes;
    Status status;
    GameController gameController;
    // Status
    public static bool[] prepareToCurse = new bool[playerCount];
    public static bool[] prepareToPride = new bool[playerCount];
    // Statics
    public static int[] playerUsedCardsCount = new int[playerCount];
    public static int[] playerFoundCardsCount = new int[playerCount];

    // Use this for initialization
    void Start()
    {
        // Set GameObjects
        for (int i = 0; i < maxCardsCount; i++)
        {
            ImgPlayerCards[player1, i] = GameObject.Find("PnPlayer1Cards/Card" + (i + 1)).GetComponent<Image>();
            ImgPlayerCards[player2, i] = GameObject.Find("PnPlayer2Cards/Card" + (i + 1)).GetComponent<Image>();
        }
        // Set cards
        for (int i = 0; i < maxCardsCount; i++)
        {
            playerCards[player1, i] = playerCards[player2, i] = -1;
        }
        // Set class
        players = gameObject.GetComponent<Players>();
        shapes = gameObject.GetComponent<Shapes>();
        status = gameObject.GetComponent<Status>();
        gameController = gameObject.GetComponent<GameController>();
    }

    public string GetCardName(int cardIndex)
    {
        return cardsName[cardIndex];
    }

    public int GetCardPoint(int cardIndex)
    {
        return cardsPoint[cardIndex];
    }

    public string GetCardShape(int cardIndex)
    {
        return cardsShape[cardIndex];
    }

    public int GetCardColor(int cardIndex)
    {
        return cardsColor[cardIndex];
    }

    public List<int> GetCardIndexByShapes(string shape)
    {
        List<int> list = new List<int>();
        for (int i = 0; i < maxCardIndex; i++)
        {
            if (cardsShape[i] == shape)
            {
                list.Add(i);
            }
        }
        return list;
    }

    // Player get a card
    public void FindCard(int player, int index = -1)
    {
        playerFoundCardsCount[player]++;
        if (playerCardsCount[player] >= maxCardsCount)
        {
            return;
        }
        int cardIndex = index == -1 ? Random.Range(0, maxCardIndex) : index;
        playerCardsCount[player]++;
        int newCardLocation = -1;
        for (int i = 0; i < maxCardsCount; i++)
        {
            if (playerCards[player, i] == -1)
            {
                newCardLocation = i;
                break;
            }
        }
        playerCards[player, newCardLocation] = cardIndex;
        ImgPlayerCards[player, newCardLocation].sprite = Resources.Load((cardIndex + 1).ToString(), typeof(Sprite)) as Sprite;
        ArrangeCards(player);
    }

    public void ArrangeCards(int player)
    {
        for (int i = 0; i < maxCardsCount; i++)
        {
            var v = ImgPlayerCards[player, i].transform.position;
            if (playerCards[player, i] == -1 && v.y > arrangeCardCriticalValueY)
            {
                v.y -= arrangeCardY;
            }
            if (playerCards[player, i] != -1 && v.y < arrangeCardCriticalValueY)
            {
                v.y += arrangeCardY;
            }
            ImgPlayerCards[player, i].transform.position = v;
        }
    }

    // Player try to use cards after drawing a shape
    public void TryToUseCard(int player, string shape)
    {
        List<int> cardsSuitShapes = GetCardIndexByShapes(shape);
        for (int i = 0; i < maxCardsCount; i++)
        {
            for (int j = 0; j < cardsSuitShapes.Count; j++)
            {
                if (playerCards[player, i] == cardsSuitShapes[j])
                {
                    UseCard(player, cardsSuitShapes[j]);
                    RemoveCard(player, i);
                    RemoveShapes(player);
                }
            }
        }
    }

    // Remove a used card
    private void RemoveCard(int player, int index)
    {
        if (playerCards[player, index] != -1)
        {
            playerCardsCount[player]--;
            playerCards[player, index] = -1;
            ImgPlayerCards[player, index].sprite = null;
        }
        ArrangeCards(player);
    }

    // Remove drawn shapes
    public void RemoveShapes(int player)
    {
        shapes.playerShapes[player] = new List<Shapes.TargetShape>();
        shapes.TxPlayerShapes[player].text = "";
    }

    // Player use a card
    private void UseCard(int player, int cardIndex)
    {
        playerUsedCardsCount[player]++;
        gameController.UpdateLog(player, "Use a card - " + GetCardName(cardIndex));
        if (Medusa.buffBless)
        {
            players.ChangeHP(player1, 1);
        }
        int opponentPlayer = GetOpponent(player);
        // Status-Counterspell
        if (status.GetCounterspell(opponentPlayer))
        {
            status.SetCounterspell(opponentPlayer, false);
            return;
        }
        // Status-Duplicate
        if (status.GetDuplicate(player))
        {
            status.SetDuplicate(player, false);
            UseCard(player, cardIndex);
        }
        // Status-Infest
        if (status.GetInfest(player))
        {
            status.SetInfest(player, false);
        }
        switch (cardIndex + 1)
        {
            case 1: // Strike
                players.Attack(player, 2);
                break;
            case 2: // Strike2 
                players.Attack(player, 3);
                break;
            case 3: // Strike3
                players.Attack(player, 4);
                break;
            case 4: // Heal
                players.ChangeHP(player, 3);
                break;
            case 5: // Rrefresh
                int refreshCardsCount = playerCardsCount[player];
                for (int i = 0; i < maxCardsCount; i++)
                {
                    RemoveCard(player, i);
                }
                for (int i = 0; i < refreshCardsCount; i++)
                {
                    FindCard(player);
                }
                break;
            case 6: // Duplicate *
                status.SetDuplicate(player);
                break;
            case 7: // LifeSteal
                players.Attack(player, 2);
                players.ChangeHP(player, 2);
                break;
            case 8: // Upgrade *
                status.AddLevel(player);
                break;
            case 9: // Sacrifice
                players.Attack(player, 4);
                players.Attack(opponentPlayer, 4);
                break;
            case 10: // Blur *
                status.SetBlur(player);
                break;
            case 11: // Rob
                players.ChangeGP(opponentPlayer, -5);
                players.ChangeGP(player, 5);
                if (playerCardsCount[opponentPlayer] == 0)
                {
                    break;
                }
                for (int i = 0; i < maxCardsCount; i++)
                {
                    if (playerCards[opponentPlayer, i] != -1)
                    {
                        FindCard(player, i + 1);
                        RemoveCard(opponentPlayer, i);
                        break;
                    }
                }
                break;
            case 12: // Freeze *
                status.SetFreeze(opponentPlayer);
                break;
            case 13: // Push
                players.Attack(player, 1);
                players.Push(player);
                break;
            case 14: // Infest *
                status.SetInfest(opponentPlayer);
                break;
            case 15: // Discover
                FindCard(player);
                FindCard(player);
                break;
            case 16: // Mirror
                for (int i = 0; i < maxCardsCount; i++)
                {
                    RemoveCard(player, i);
                }
                for (int i = 0; i < maxCardsCount; i++)
                {
                    if (playerCards[opponentPlayer, i] != -1)
                    {
                        FindCard(player, playerCards[opponentPlayer, i]);
                    }
                }
                break;
            case 17: // Ignite
                players.Attack(player, playerCardsCount[opponentPlayer] >= 4 ? 5 : 1);
                break;
            case 18: // Counterspell *
                status.SetCounterspell(player);
                break;
            case 19: // Invincible *
                status.SetInvicible(player);
                break;
            case 20: // Blast
                HashSet<int> set = new HashSet<int>();
                for (int i = 0; i < maxCardsCount; i++)
                {
                    if (playerCards[player, i] != -1)
                    {
                        set.Add(GetCardColor(i));
                    }
                }
                players.Attack(player, set.Count >= 4 ? 5 : 1);
                break;
            case 21: // Vaporize
                if (playerCardsCount[opponentPlayer] == 0)
                {
                    break;
                }
                for (int i = 0; i < maxCardsCount; i++)
                {
                    RemoveCard(opponentPlayer, i);
                }
                players.ChangeGP(opponentPlayer, -10);
                break;
            case 22: // Balance
                players.ChangeGP(player, -5);
                players.ChangeHP(player, 5);
                break;
            case 23: // Avenge *
                status.SetAvenge(player);
                break;
            case 24: // Curse *
                status.SetCurse(player);
                break;
            case 25: // Pride *
                status.SetPride(player);
                break;
            case 26: // Shock
                if (Random.Range(0, 2) == 0)
                {
                    players.Attack(player, 5);
                }
                break;
            case 27: // Explode
                players.Attack(player, Random.Range(1, 6));
                break;
            case 28: break;// Treasure
            case 29: // Rage
                players.Attack(player, players.GetHP(player) < 10 ? 5 : 3);
                break;
            case 30: // Charge
                players.Attack(player, playerCardsCount[player]);
                break;
        }
        players.ChangeGP(player, GetCardPoint(cardIndex));
        // Use Treasure automatically
        for (int i = 0; i < maxCardsCount; i++)
        {
            if (playerCards[player, i] == 27)
            {
                players.ChangeGP(player, 7);
                RemoveCard(player, i);
            }
        }
        // Status-Avenge
        if (status.GetAvenge(opponentPlayer))
        {
            players.ChangeHP(player, -2);
            status.SetAvenge(opponentPlayer, false);
        }
        // Status-Curse
        if (status.GetCurse(player))
        {
            if (prepareToCurse[player])
            {
                prepareToCurse[player] = false;
            }
            else
            {
                players.Attack(player, 1);
                status.LessCurse(player);
            }
        }
        // Status-Pride
        if (status.GetPride(player))
        {
            if (prepareToPride[player])
            {
                prepareToPride[player] = false;
            }
            else
            {
                players.ChangeGP(player, 2);
                status.LessPride(player);
            }
        }
        // Tutorial check
        if (bossTitle == GetBossTitle(BossTitle.TUTORIAL))
        {
            gameController.GameOver(player1);
        }
    }

    // Get opponent player
    private int GetOpponent(int player)
    {
        return player == player1 ? player2 : player1;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
