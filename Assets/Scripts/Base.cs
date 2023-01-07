using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{

	// Game
	public const int playerCount = 2;
	public const int player1 = 0;
	public const int player2 = 1;
	// Settings
	public const float gameSpeed = 1f;
	public static int gameControl = 1; // 0-keyboard, 1-sensors
	public static int gamePlayer = 1; // 0-single, 1-double
	public static bool showLog = true;

    public enum GameControl
    {
        KEYBOARD = 0,
        SENSORS = 1
    }

	public enum GamePlayer
    {
		SINGLE = 0,
		DOUBLE = 1
    }

    public int GetGameControl(GameControl gameControl)
    {
        return (int)gameControl;
    }

	public int GetGamePlayer(GamePlayer gamePlayer)
    {
		return (int)gamePlayer;
    }

	public void SetVisible(GameObject gameObject, bool visible)
    {
		gameObject.GetComponent<CanvasGroup>().alpha = visible ? 1 : 0;
		gameObject.GetComponent<CanvasGroup>().interactable = gameObject.GetComponent<CanvasGroup>().blocksRaycasts = visible;
    }

    // Use this for initialization
    void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}
