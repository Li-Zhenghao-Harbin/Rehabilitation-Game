﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Status : Base
{

    // Player status
    static bool[] isDuplicate = new bool[2];
    static int[] level = new int[2];
    static bool[] isBlur = new bool[2];
    static float[] freeze = new float[2];
    static bool[] isInfest = new bool[2];
    float[] infestTimer = new float[2];
    const int infestStepInterval = 5;
    int[] infestStep = new int[2];
    static bool[] isCounterspell = new bool[2];
    static float[] invincible = new float[2];
    static bool[] isAvenge = new bool[2];
    static int[] curse = new int[2];
    static int[] pride = new int[2];
    // GameObject
    public static Text[] TxPlayerStatus = new Text[2];
    // Display status
    StringBuilder sb;
    // Class
    Players players;

    public bool GetDuplicate(int player)
    {
        return isDuplicate[player];
    }

    public void SetDuplicate(int player, bool value = true)
    {
        isDuplicate[player] = value;
    }

    public int GetLevel(int player)
    {
        return level[player];
    }

    public void AddLevel(int player)
    {
        level[player]++;
        players.ChangeMaxHp(player, 1);
    }

    public bool GetBlur(int player)
    {
        return isBlur[player];
    }

    public void SetBlur(int player, bool value = true)
    {
        isBlur[player] = value;
    }

    public bool IsFreeze(int player)
    {
        return freeze[player] > 0;
    }

    public float GetFreeze(int player)
    {
        return freeze[player];
    }

    public void SetFreeze(int player)
    {
        freeze[player] = 5f;
    }

    public bool GetInfest(int player)
    {
        return isInfest[player];
    }

    private float GetInfestTime(int player)
    {
        return infestTimer[player];
    }

    public void SetInfest(int player, bool value = true)
    {
        if (!isInfest[player])
        {
            infestTimer[player] = 0;
        }
        isInfest[player] = value;
    }

    public bool GetCounterspell(int player)
    {
        return isCounterspell[player];
    }

    public void SetCounterspell(int player, bool value = true)
    {
        isCounterspell[player] = value;
    }

    public bool IsInvincible(int player)
    {
        return invincible[player] > 0;
    }

    public float GetInvincible(int player)
    {
        return invincible[player];
    }

    public void SetInvicible(int player)
    {
        invincible[player] = 10f;
    }

    public bool GetAvenge(int player)
    {
        return isAvenge[player];
    }

    public void SetAvenge(int player, bool value = true)
    {
        isAvenge[player] = value;
    }

    public bool GetCurse(int player)
    {
        return curse[player] != 0;
    }

    public void SetCurse(int player)
    {
        curse[player] = 3;
        Cards.prepareToCurse[player] = true;
    }

    public void LessCurse(int player)
    {
        curse[player]--;
    }

    public bool GetPride(int player)
    {
        return pride[player] != 0;
    }

    public void SetPride(int player)
    {
        pride[player] = 3;
        Cards.prepareToPride[player] = true;
    }

    public void LessPride(int player)
    {
        pride[player]--;
    }

    // Use this for initialization
    void Start()
    {
        // Set GameObjects
        TxPlayerStatus[player1] = GameObject.Find("TxPlayer1Status").GetComponent<Text>();
        TxPlayerStatus[player2] = GameObject.Find("TxPlayer2Status").GetComponent<Text>();
        // Set class
        players = gameObject.GetComponent<Players>();
    }

    private void DisplayStatus(int player)
    {
        sb = new StringBuilder();
        if (GetDuplicate(player))
        {
            sb.AppendLine("Duplicate");
        }
        if (GetLevel(player) > 0)
        {
            sb.Append("Upgrade[");
            sb.Append(GetLevel(player).ToString());
            sb.AppendLine("]");
        }
        if (GetBlur(player))
        {
            sb.AppendLine("Blur");
        }
        if (IsFreeze(player))
        {
            sb.Append("Freeze*");
            sb.AppendLine(((int)GetFreeze(player)).ToString());
            freeze[player] -= Time.deltaTime;
        }
        if (GetInfest(player))
        {
            sb.Append("Infest*");
            sb.AppendLine(((int)GetInfestTime(player)).ToString());
        }
        if (GetCounterspell(player))
        {
            sb.AppendLine("Counterspell");
        }
        if (IsInvincible(player))
        {
            sb.Append("Invincible*");
            sb.AppendLine(((int)GetInvincible(player)).ToString());
            invincible[player] -= Time.deltaTime;
        }
        if (GetAvenge(player))
        {
            sb.AppendLine("Avenge");
        }
        if (GetCurse(player))
        {
            sb.Append("Curse(");
            sb.Append(curse[player].ToString());
            sb.AppendLine(")");
        }
        if (GetPride(player))
        {
            sb.Append("Pride(");
            sb.Append(pride[player].ToString());
            sb.AppendLine(")");
        }
        TxPlayerStatus[player].text = sb.ToString();
    }

    // Status-Infest
    private void UpdateInfest(int player)
    {
        if (GetInfest(player))
        {
            infestTimer[player] += Time.deltaTime;
            if ((int)GetInfestTime(player) == infestStep[player])
            {
                players.ChangeHP(player, -1);
                infestStep[player] += infestStepInterval;
            }
        }
    }

    private int GetOpponent(int player)
    {
        return player == player1 ? player2 : player1;
    }

    // Update is called once per frame
    void Update()
    {
        // Display player's status
        DisplayStatus(player1);
        DisplayStatus(player2);
        UpdateInfest(player1);
        UpdateInfest(player2);
    }
}
