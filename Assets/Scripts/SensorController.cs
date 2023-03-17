using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensorController : Base
{

    bool[] detectActive = new bool[playerCount] { false, false }; // Check if mouse is clicked, in sensor, represented for the active button
    bool[] hasSetParticularPoints = new bool[playerCount];
    const int coordinatePointsCount = 2; // x and y
    const int xx = 0; // Position to store x
    const int yy = 1; // Position to store y
    const int particularPointsCount = 7;
    const int endParticularPointsStartIndex = 5; // Where the index begins to indicate the ending particular points
    int[,,] particularPoints = new int[playerCount, particularPointsCount, coordinatePointsCount]; // Store the x and y of particular points of each player
    bool[,] activedParticularPoints = new bool[playerCount, particularPointsCount]; // Check if moved to particular points
    int[,] beginPoint = new int[playerCount, coordinatePointsCount]; // Store the x and y of the begin point of each player
    int[,] endPoint = new int[playerCount, coordinatePointsCount]; // Store the x and y of the end point of each player

    // Distance between particular points
    const int interval = 45; // distance between particular points
    const int coInterval = 20; // length of one side of detecting area
    const int scale = 8; // resize x and y

    // Transformed data from serial port
    static int[] convertedPositionX = new int[playerCount];
    static int[] convertedPositionZ = new int[playerCount];
    static int[] convertedUIPositionX = new int[playerCount];
    static int[] convertedUIPositionY = new int[playerCount];

    // GameObjects
    GameObject pnPlayer1Points;
    GameObject pnPlayer2Points;
    List<GameObject> pnPlayerPoints = new List<GameObject>(playerCount);
    GameObject[] playerBeginParticularPoints = new GameObject[playerCount];
    GameObject[,] playerParticularPoints = new GameObject[playerCount, particularPointsCount];
    GameObject[] playerParticularPointers = new GameObject[playerCount];

    // Class
    public SerialController serialController;
    Players players;
    Shapes shapes;
    Cards cards;

    // Use this for initialization
    void Start()
    {
        // init
        serialController = GameObject.Find("SerialController").GetComponent<SerialController>();
        players = gameObject.GetComponent<Players>();
        shapes = gameObject.GetComponent<Shapes>();
        cards = gameObject.GetComponent<Cards>();
        // Set panels
        pnPlayer1Points = GameObject.Find("Canvas/PnPlayer1Points");
        pnPlayerPoints.Add(pnPlayer1Points);
        pnPlayer2Points = GameObject.Find("Canvas/PnPlayer2Points");
        pnPlayerPoints.Add(pnPlayer2Points);
        SetVisible(pnPlayerPoints[0], false);
        SetVisible(pnPlayerPoints[1], false);
        // Set particular points
        playerBeginParticularPoints[player1] = GameObject.Find("Canvas/PnPlayer1Points/Player1Point0");
        playerBeginParticularPoints[player2] = GameObject.Find("Canvas/PnPlayer2Points/Player2Point0");
        for (int i = 1; i <= particularPointsCount; i++)
        {
            playerParticularPoints[player1, i - 1] = GameObject.Find("Canvas/PnPlayer1Points/Player1Point" + i);
            playerParticularPoints[player2, i - 1] = GameObject.Find("Canvas/PnPlayer2Points/Player2Point" + i);
        }
        playerParticularPointers[player1] = GameObject.Find("Canvas/PnPlayer1Points/Player1Pointer");
        playerParticularPointers[player2] = GameObject.Find("Canvas/PnPlayer2Points/Player2Pointer");
    }


    public int[] GetConvertedDataFromSerial(string message)
    {
        string[] strs = message.Split(',');
        int x = -Convert.ToInt32(strs[1]) >> scale;
        int z = Convert.ToInt32(strs[2]) >> scale;
        return new int[] { x, z };
    }

    public int[] GetConvertedUIDataFromSerial(string message)
    {
        string[] strs = message.Split(',');
        int x = -Convert.ToInt32(strs[1]) >> scale;
        int y = Convert.ToInt32(strs[2]) >> scale;
        return new int[] { x, y };
    }

    public bool IsInRange(int num, int range)
    {
        return num > -range && num < range;
    }

    public bool Coincident(int x1, int y1, int x2, int y2)
    {
        return Mathf.Abs(x1 - x2) < coInterval && Mathf.Abs(y1 - y2) < coInterval;
    }

    public void InitialParticularPoints(int targetPlayer, int pointIndex, int x, int y)
    {
        particularPoints[targetPlayer, pointIndex, xx] = x;
        particularPoints[targetPlayer, pointIndex, yy] = y;
        playerParticularPoints[targetPlayer, pointIndex].GetComponent<Image>().color = pointIndex < endParticularPointsStartIndex ? new Color(70, 70, 70) : new Color(255, 195, 0); ;
        playerParticularPoints[targetPlayer, pointIndex].GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0);
    }

    private void SensorMoving(int targetPlayer)
    {
        if (detectActive[targetPlayer]) // detecting targetPlayer points
        {
            if (activedParticularPoints[targetPlayer, endParticularPointsStartIndex] &&
                activedParticularPoints[targetPlayer, endParticularPointsStartIndex + 1])
            {
                SetVisible(pnPlayerPoints[targetPlayer], detectActive[targetPlayer] = false);
                cards.RemoveShapes(targetPlayer);
                return;
            }
            for (int i = 0; i < particularPointsCount; i++)
            {
                if (Coincident(convertedUIPositionX[targetPlayer],
                    convertedUIPositionY[targetPlayer],
                    particularPoints[targetPlayer, i, xx],
                    particularPoints[targetPlayer, i, yy]))
                {
                    activedParticularPoints[targetPlayer, i] = true;
                    playerParticularPoints[targetPlayer, i].GetComponent<Image>().color = new Color(0, 0, 150f);
                }
            }
        }
        else
        {
            if (IsInRange(convertedPositionZ[targetPlayer], sensorSensitivity))
            {
                if (convertedPositionX[targetPlayer] < -sensorSensitivity)
                {
                    players.Move(targetPlayer, Players.MoveDirection.LEFT);

                }
                else if (convertedPositionX[targetPlayer] > sensorSensitivity)
                {
                    players.Move(targetPlayer, Players.MoveDirection.RIGHT);

                }
            }
            else if (IsInRange(convertedPositionX[targetPlayer], sensorSensitivity))
            {
                if (convertedPositionZ[targetPlayer] > sensorSensitivity)
                {
                    players.Move(targetPlayer, Players.MoveDirection.UP);
                }
                else if (convertedPositionZ[targetPlayer] < -sensorSensitivity)
                {
                    players.Move(targetPlayer, Players.MoveDirection.DOWN);
                }
            }
        }
    }

    // Checking each particular points, if no particular points actived, then intialize them
    private void CheckingPoints(int targetPlayer)
    {
        detectActive[targetPlayer] = !detectActive[targetPlayer];
        SetVisible(pnPlayerPoints[targetPlayer], detectActive[targetPlayer]);
        if (detectActive[targetPlayer])
        {
            // Reset particular points
            for (int i = 0; i < particularPointsCount; i++)
            {
                activedParticularPoints[targetPlayer, i] = false;
            }
            detectActive[targetPlayer] = true;
            hasSetParticularPoints[targetPlayer] = false;
            beginPoint[targetPlayer, xx] = convertedUIPositionX[targetPlayer];
            beginPoint[targetPlayer, yy] = convertedUIPositionY[targetPlayer];
            if (!hasSetParticularPoints[targetPlayer])
            {
                // Initial particular points
                InitialParticularPoints(targetPlayer, 0,
                    convertedUIPositionX[targetPlayer] + interval, convertedUIPositionY[targetPlayer]);
                InitialParticularPoints(targetPlayer, 1,
                    convertedUIPositionX[targetPlayer] + (interval << 1), convertedUIPositionY[targetPlayer]);
                InitialParticularPoints(targetPlayer, 2,
                    convertedUIPositionX[targetPlayer] + interval, convertedUIPositionY[targetPlayer] - interval);
                InitialParticularPoints(targetPlayer, 3,
                    convertedUIPositionX[targetPlayer], convertedUIPositionY[targetPlayer] - interval);
                InitialParticularPoints(targetPlayer, 4,
                    convertedUIPositionX[targetPlayer] - interval, convertedUIPositionY[targetPlayer] - interval);
                InitialParticularPoints(targetPlayer, 5,
                    convertedUIPositionX[targetPlayer] - interval, convertedUIPositionY[targetPlayer]);
                InitialParticularPoints(targetPlayer, 6,
                    convertedUIPositionX[targetPlayer], convertedUIPositionY[targetPlayer] + interval);
                playerBeginParticularPoints[targetPlayer].GetComponent<RectTransform>().localPosition = new Vector3(beginPoint[targetPlayer, xx], beginPoint[targetPlayer, yy]);
                hasSetParticularPoints[targetPlayer] = true;
            }
        }
        else
        {
            endPoint[targetPlayer, xx] = convertedUIPositionX[targetPlayer];
            endPoint[targetPlayer, yy] = convertedUIPositionY[targetPlayer];
            // Check shape when begin point meet end point
            if (Coincident(beginPoint[targetPlayer, xx],
                beginPoint[targetPlayer, yy],
                endPoint[targetPlayer, xx],
                endPoint[targetPlayer, yy]))
            {
                CheckShape(targetPlayer);
            }
        }
    }

    public void CheckShape(int targetPlayer)
    {
        if (activedParticularPoints[targetPlayer, 0] &&
            !activedParticularPoints[targetPlayer, 1] &&
            activedParticularPoints[targetPlayer, 2] &&
            activedParticularPoints[targetPlayer, 3] &&
            !activedParticularPoints[targetPlayer, 4])
        {
            shapes.AppendShape(targetPlayer, Shapes.TargetShape.SQUARE);
        }
        else if (activedParticularPoints[targetPlayer, 0] &&
            activedParticularPoints[targetPlayer, 1] &&
            activedParticularPoints[targetPlayer, 2] &&
            !activedParticularPoints[targetPlayer, 3] &&
            !activedParticularPoints[targetPlayer, 4])
        {
            shapes.AppendShape(targetPlayer, Shapes.TargetShape.LOWER_TRIANGLE);
        }
        else if (!activedParticularPoints[targetPlayer, 0] &&
            !activedParticularPoints[targetPlayer, 1] &&
            activedParticularPoints[targetPlayer, 2] &&
            activedParticularPoints[targetPlayer, 3] &&
            activedParticularPoints[targetPlayer, 4])
        {
            shapes.AppendShape(targetPlayer, Shapes.TargetShape.UPPER_TRIANGLE);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameControl == GetGameControl(GameControl.SENSORS))
        {
            if (Input.GetKeyUp(KeyCode.A))
            {
                CheckingPoints(player1);
            }
            if (Input.GetKeyUp(KeyCode.L))
            {
                CheckingPoints(player2);
            }
            SensorMoving(player1);
            SensorMoving(player2);
            // Get data from serial port
            string message = serialController.ReadSerialMessage();
            // debug
            if (message != null)
            {
                Debug.Log(message);
            }
            if (message != null &&
                !ReferenceEquals(message, SerialController.SERIAL_DEVICE_CONNECTED) &&
                !ReferenceEquals(message, SerialController.SERIAL_DEVICE_DISCONNECTED))
            {
                convertedPositionX[player1] = GetConvertedDataFromSerial(message.Split('|')[player1])[0];
                convertedPositionZ[player1] = GetConvertedDataFromSerial(message.Split('|')[player1])[1];
                convertedUIPositionX[player1] = GetConvertedUIDataFromSerial(message.Split('|')[player1])[0];
                convertedUIPositionY[player1] = GetConvertedUIDataFromSerial(message.Split('|')[player1])[1];
                convertedPositionX[player2] = GetConvertedDataFromSerial(message.Split('|')[player2])[0];
                convertedPositionZ[player2] = GetConvertedDataFromSerial(message.Split('|')[player2])[1];
                convertedUIPositionX[player2] = GetConvertedUIDataFromSerial(message.Split('|')[player2])[0];
                convertedUIPositionY[player2] = GetConvertedUIDataFromSerial(message.Split('|')[player2])[1];
                if (detectActive[player1])
                {
                    playerParticularPointers[player1].GetComponent<RectTransform>().localPosition = new Vector3(convertedUIPositionX[player1], convertedUIPositionY[player1], 0);
                }
                if (detectActive[player2])
                {
                    playerParticularPointers[player2].GetComponent<RectTransform>().localPosition = new Vector3(convertedUIPositionX[player2], convertedUIPositionY[player2], 0);
                }
            }
        }
    }
}
