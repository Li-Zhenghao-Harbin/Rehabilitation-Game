using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calibration : Base
{
	const int directionCount = 4;
	// Game object
	Text TxAttention;
	// Direction
	int activeDirection = -1; // 0-player1left, 1-player1right, 2-player1up, 3-player1down, 4-player2left, 5-player2right, 6-player2up, 7-player2down
    private enum Direction
    {
		LEFT = 0,
		RIGHT = 1,
		UP = 2,
		DOWN = 3
    }
	// Calibration
	string[,] calibrationValue = new string[playerCount, directionCount];
	// Class
	public SerialController serialController;

	private int GetDirectionIndex(Direction direction)
    {
		return (int)direction;
    }

    
	// Use this for initialization
    void Start () {
		TxAttention = GameObject.Find("TxAttention").GetComponent<Text>();
		GameObject.Find("BtnPlayer1Left").GetComponent<Button>().onClick.AddListener(BtnPlayer1LeftOnClick);
		GameObject.Find("BtnPlayer1Right").GetComponent<Button>().onClick.AddListener(BtnPlayer1RightOnClick);
		GameObject.Find("BtnPlayer1Up").GetComponent<Button>().onClick.AddListener(BtnPlayer1UpOnClick);
		GameObject.Find("BtnPlayer1Down").GetComponent<Button>().onClick.AddListener(BtnPlayer1DownOnClick);
		GameObject.Find("BtnPlayer2Left").GetComponent<Button>().onClick.AddListener(BtnPlayer2LeftOnClick);
		GameObject.Find("BtnPlayer2Right").GetComponent<Button>().onClick.AddListener(BtnPlayer2RightOnClick);
		GameObject.Find("BtnPlayer2Up").GetComponent<Button>().onClick.AddListener(BtnPlayer2UpOnClick);
		GameObject.Find("BtnPlayer2Down").GetComponent<Button>().onClick.AddListener(BtnPlayer2DownOnClick);
	}

	private void BtnPlayer1LeftOnClick()
    {
		SetActiveDirection(player1, Direction.LEFT);
    }

	private void BtnPlayer1RightOnClick()
    {
		SetActiveDirection(player1, Direction.RIGHT);
    }

	private void BtnPlayer1UpOnClick()
    {
		SetActiveDirection(player1, Direction.UP);
    }

	private void BtnPlayer1DownOnClick()
    {
		SetActiveDirection(player1, Direction.DOWN);
    }

	private void BtnPlayer2LeftOnClick()
    {
		SetActiveDirection(player2, Direction.LEFT);
    }

	private void BtnPlayer2RightOnClick()
    {
		SetActiveDirection(player2, Direction.RIGHT);
    }

	private void BtnPlayer2UpOnClick()
    {
		SetActiveDirection(player2, Direction.UP);
    }

	private void BtnPlayer2DownOnClick()
    {
		SetActiveDirection(player2, Direction.DOWN);
    }


	private void SetActiveDirection(int player, Direction direction)
	{
		int currentDirectionIndex = (player << 2) + GetDirectionIndex(direction);
		if (activeDirection == -1)
		{
			activeDirection = currentDirectionIndex;
			TxAttention.text = "Please select a direction";
		}
		else
		{
			activeDirection = activeDirection == currentDirectionIndex ? -1 : currentDirectionIndex;
			TxAttention.text = "Move player" + (player + 1).ToString() + "\'s sensor " + direction.ToString();
		}
	}

	private int GetPlayerFromDirectionIndex(int direction)
    {
		return direction < direction - 1 ? player1 : player2; 
    }

	private int GetDirectionFronDirectionIndex(int direction)
    {
		return direction < direction - 1 ? direction : direction - directionCount;
    }

	// Update is called once per frame
	void Update () {
		if (activeDirection != -1)
        {
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
				calibrationValue[GetPlayerFromDirectionIndex(activeDirection), GetDirectionFronDirectionIndex(activeDirection)] = message;
			}
        }
	}
}
