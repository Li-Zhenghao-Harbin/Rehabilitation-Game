using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Medusa : BossController
{
	// Timer
	const float initBossActionTimer = 10f;
	float bossActionTimer = initBossActionTimer;
	const float initPreparingTimer = 2f;
	float preparingTimer = initPreparingTimer;
	const float initGazetTimer = 2f;
	float gazeTimer = 0f;
	const float initShotTimer = 4f;
	float shotTimer = 0f;
	// Class
	GameController gameController;
	Players players;
	Status status;
	// Game object
	Text TxBossPosition;
	// Boss
	int medusaAttackDamge = 5;
	public static bool buffBless = false;
	// Position
	int medusaPosition = 1; // 0-left, 1-top, 2-right
	bool activedMove = false;
	// Gaze
	bool activedGaze = false;
	// Shot
	GameObject Arrow;
	float arrowSpeed = 0.12f;
	Vector3 playerVector3;
	bool activedShot = false;

	private enum MedusaPosition
	{
		LEFT = 0,
		TOP = 1,
		RIGHT = 2
	}

	private void SetMedusaPosition(MedusaPosition medusaPosition)
	{
		this.medusaPosition = (int)medusaPosition;
		SetArrowRotation(medusaPosition);
	}

	private int GetMedusaPosition(MedusaPosition medusaPosition)
	{
		return (int)medusaPosition;
	}

	// Use this for initialization
	void Start()
	{
		// Set game object
		TxBossPosition = GameObject.Find("TxBossPosition").GetComponent<Text>();
		Arrow = GameObject.Find("Arrow");
		// Set class
		gameController = gameObject.GetComponent<GameController>();
		players = gameObject.GetComponent<Players>();
		status = gameObject.GetComponent<Status>();
	}

	private void Gaze()
	{
		preparingTimer = initPreparingTimer;
		activedGaze = false;
		gazeTimer = initGazetTimer;
	}

	private void Shot()
	{
		shotTimer = initShotTimer;
		activedShot = false;
		Arrow.SetActive(true);
		if (medusaPosition == GetMedusaPosition(MedusaPosition.LEFT))
		{
			Arrow.transform.position = new Vector3(-10, 0, 0);
		}
		else if (medusaPosition == GetMedusaPosition(MedusaPosition.TOP))
		{
			Arrow.transform.position = new Vector3(0, 0, 5);
		}
		else if (medusaPosition == GetMedusaPosition(MedusaPosition.RIGHT))
		{
			Arrow.transform.position = new Vector3(10, 0, 0);
		}
		playerVector3 = gameController.Player[player1].transform.localPosition;
	}

	private void ArrowMoving()
	{
		Arrow.transform.localPosition = Vector3.MoveTowards(Arrow.transform.localPosition, playerVector3, arrowSpeed);
	}

	private void RemoveArrow()
	{
		Arrow.SetActive(false);
	}

	private void SetMedusaPositionWarning(string message, Vector2 vector2)
	{
		TxBossPosition.text = message;
		TxBossPosition.transform.localPosition = vector2;
	}

	private void SetArrowRotation(MedusaPosition medusaPosition)
    {
		switch (medusaPosition)
        {
			case MedusaPosition.LEFT:
				Arrow.transform.eulerAngles = new Vector3(0, -90, 0.3f);
				break;
			case MedusaPosition.TOP:
				Arrow.transform.eulerAngles = new Vector3(0, 0, 0);
				break;
			case MedusaPosition.RIGHT:
				Arrow.transform.eulerAngles = new Vector3(0, 90, 0.3f);
				break;
		}
	}

	// Medusa moves
	private void Move()
	{
		int direction = Random.Range(0, 3);
		switch (direction)
		{
			case 0:
				SetMedusaPosition(MedusaPosition.LEFT);
				SetMedusaPositionWarning("Medusa on LEFT", new Vector2(-300, 0));
				break;
			case 1:
				SetMedusaPosition(MedusaPosition.TOP);
				SetMedusaPositionWarning("Medusa on TOP", new Vector2(0, 100));
				break;
			case 2:
				SetMedusaPosition(MedusaPosition.RIGHT);
				SetMedusaPositionWarning("Medusa on RIGHT", new Vector2(300, 0));
				break;
		}
	}


	// Update is called once per frame
	void Update()
	{
		// Boss action
		if (bossActionTimer > 0f)
		{
			bossActionTimer -= Time.deltaTime;
			if (!activedMove && bossActionTimer < 1f)
			{
				Move();
				activedMove = true;
			}
		}
		else
		{
			if (Random.Range(0, 2) == 0)
			{
				Gaze();
			}
			else
			{
				Shot();
			}
			bossActionTimer = initBossActionTimer;
			activedMove = false;
		}
		// Gaze
		if (gazeTimer > 0f)
		{
			SetWarning("Gaze", "Turn BACK to Medusa!");
			if (preparingTimer > 0f)
			{
				preparingTimer -= Time.deltaTime;
			}
			else
			{
				gazeTimer -= Time.deltaTime;
				if (!activedGaze)
				{
					if ((medusaPosition == GetMedusaPosition(MedusaPosition.LEFT) &&
						gameController.Player[player1].transform.localEulerAngles.y != 180) ||
						(medusaPosition == GetMedusaPosition(MedusaPosition.TOP) &&
						gameController.Player[player1].transform.localEulerAngles.y != 270) ||
						(medusaPosition == GetMedusaPosition(MedusaPosition.RIGHT) &&
						gameController.Player[player1].transform.localEulerAngles.y != 0))
					{
						status.SetFreeze(player1, 3f);
						players.Attack(player2, medusaAttackDamge);
						players.ChangeGP(player1, -10);
						players.ChangeGP(player2, 5);
						activedGaze = true;
					}
				}
			}
		}
		else
		{
			SetWarning();
		}
		// Shot
		if (shotTimer > 0f)
		{
			shotTimer -= Time.deltaTime;
			SetWarning("Shot", "Avoid the arrow!");
			ArrowMoving();
			if (!activedShot && players.Meet(gameController.Player[player1], Arrow))
			{
				RemoveArrow();
				players.Attack(player2, medusaAttackDamge);
				players.ChangeGP(player1, -10);
				players.ChangeGP(player2, 5);
				activedShot = true;
			}
		}
		else
		{
			RemoveArrow();
			if (gazeTimer == 0f) // Avoid cancel warning while activing gaze
			{
				SetWarning();

			}
		}
		// Buff - Accelerate
		if (players.GetGP(player1) >= 15)
		{
			DisplayBuff(PnPlayerBuffs, 0);
			playerMovingSpeed[player1] = 2.5f;
		}
		else
		{
			DisplayBuff(PnPlayerBuffs, 0, false);
			playerMovingSpeed[player1] = 2f;
		}
		// Buff - Bless
		DisplayBuff(PnPlayerBuffs, 1, buffBless = players.GetGP(player1) >= 30);
		// Buff - Rage
		if (players.GetHP(player2) < 20)
		{
			DisplayBuff(PnBossBuffs, 0);
			medusaAttackDamge = 10;
		}
		else
		{
			DisplayBuff(PnBossBuffs, 0, false);
			medusaAttackDamge = 5;
		}
		// Buff - Hunter
		if (players.GetGP(player2) >= 25)
		{
			DisplayBuff(PnBossBuffs, 1);
			arrowSpeed = 0.3f;
		}
		else
		{
			DisplayBuff(PnBossBuffs, 1, false);
			arrowSpeed = 0.15f;
		}
	}
}
