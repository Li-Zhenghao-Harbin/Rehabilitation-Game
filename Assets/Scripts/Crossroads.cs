using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crossroads : Base
{

	// Game object
	GameObject Car;
	const float initCarPassTimer = 5f;
	float carPassTimer = 0f;
	const float carSpeed = 0.3f;
	// Car
	const float carX = 0.5f;
	const float carY = 2f;
	const float carMoveBeginZ = 10f;
	const float carMoveEndZ = -15f;
	const float carCrashDetectDistance = 1.5f;
	// Class
	Players players;
	GameController gameController;

	// Use this for initialization
	void Start()
	{
		// Set game object
		Car = GameObject.Find("Map/Crossroads/car");
		// Set class
		players = gameObject.GetComponent<Players>();
		gameController = gameObject.GetComponent<GameController>();
	}

	private void CarPass()
	{
		Car.transform.localPosition = Vector3.MoveTowards(Car.transform.localPosition, new Vector3(carX, carY, carMoveEndZ), carSpeed);
	}

	private void CarAccident(int victim)
	{
		var v = gameController.Player[victim].transform.position;
		v.x = gameController.Player[victim].transform.position.x < carX ? v.x - 2f : v.x + 2f;
		gameController.Player[victim].transform.position = v;
		players.ChangeHP(victim, -5);
	}

	// Update is called once per frame
	void Update()
	{
		if (carPassTimer > 0f)
		{
			carPassTimer -= Time.deltaTime;
			CarPass();
		}
		else
		{
			Car.transform.localPosition = new Vector3(carX, carY, carMoveBeginZ);
			carPassTimer = initCarPassTimer;
		}
		if (players.Meet(gameController.Player[player1], Car, carCrashDetectDistance))
		{
			CarAccident(player1);
		}
		if (players.Meet(gameController.Player[player2], Car, carCrashDetectDistance))
		{
			CarAccident(player2);
		}
	}
}
