using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shapes : Base
{

    // GameObjects
    public Text[] TxPlayerShapes = new Text[playerCount];
    // Shapes
    public enum TargetShape
    {
        SQUARE = 1,
        UPPER_TRIANGLE = 2,
        LOWER_TRIANGLE = 3
    }
    const int maxShapesCount = 3;
    public List<List<TargetShape>> playerShapes = new List<List<TargetShape>>();
    // Class
    Cards cards;
    // Statics
    public static int[] playerDrewShapesCount = new int[2];

    // Use this for initialization
    void Start()
    {
        // Set GameObjects
        TxPlayerShapes[player1] = GameObject.Find("TxPlayer1Shapes").GetComponent<Text>();
        TxPlayerShapes[player2] = GameObject.Find("TxPlayer2Shapes").GetComponent<Text>();
        // Set Shapes
        playerShapes.Add(new List<TargetShape>(maxShapesCount));
        playerShapes.Add(new List<TargetShape>(maxShapesCount));
        // Set class
        cards = gameObject.GetComponent<Cards>();
    }

    // Update shapes drawn by player
    public void AppendShape(int player, TargetShape shape)
    {
        playerDrewShapesCount[player]++;
        playerShapes[player].Add(shape);
        cards.TryToUseCard(player, TxPlayerShapes[player].text = ConvertShapesToString(player));
    }

    // Convert the drawn shapes to a string
    private string ConvertShapesToString(int player)
    {
        string s = "";
        for (int i = 0; i < playerShapes[player].Count; i++)
        {
            s += (int)playerShapes[player][i];
        }
        return s.Replace('1', '■').Replace('2', '▲').Replace('3', '▼');
    }

    // Update is called once per frame
    void Update()
    {

    }
}
