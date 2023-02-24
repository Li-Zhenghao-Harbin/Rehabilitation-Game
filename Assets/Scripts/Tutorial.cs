using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : GameController {

	// GameObjects
	GameObject PnTutorial;
	Text TxTutorial;
	GameObject Target;
	GameObject CardItemTarget;
	// Tutorial Steps
	const int maxSteps = 6;
    readonly List<string> steps = new List<string>(maxSteps);
	int currentStep = 0;
	// Class
	Players players;
	GameController gameController;
	Cards cards;

	// Use this for initialization
	void Start () {
		// Set Steps
		steps.Add(gameControl == GetGameControl(GameControl.KEYBOARD) ? "[1] Press \'S\' to move toward yellow area" : "[1] Move sensors down to move toward yellow area");
		steps.Add(gameControl == GetGameControl(GameControl.KEYBOARD) ? "[2] Press \'D\' to move toward yellow area" : "[2] Move sensors right to move toward yellow area");
		steps.Add(gameControl == GetGameControl(GameControl.KEYBOARD) ? "[3] Press \'A\' to move toward yellow area" : "[3] Move sensors left to move toward yellow area");
		steps.Add(gameControl == GetGameControl(GameControl.KEYBOARD) ? "[4] Press \'W\' to move toward yellow area to pick up the card" : "[4] Move sensors up to move toward yellow area to pick up the card");
		steps.Add(gameControl == GetGameControl(GameControl.KEYBOARD) ?
			"[5] Press \'R\' to draw a upper triangle\n[6] Press \'E\' to draw a square\n[7] Press \'T\' to draw a lower triangle\n[8] Press \'Y\' to draw a remove the shapes\n[9] Press \'E\' to draw a square to active the card"
			:
			"[5] Press the button than draw a upper triangle, press again after finishing\n[6] Press the button to draw a square, press again after finishing\n[7] Press the button to draw a lower triangle, press again after finishing\n[8] Press the button to cover the two yellow points, press again after finishing to remove the shapes\n[9] Press the button to draw a square, press again after drawing to active the card"
			);
		// Set GameObjects
		PnTutorial = GameObject.Find("Canvas/PnTutorial");
		TxTutorial = GameObject.Find("Canvas/PnTutorial/TxTutorial").GetComponent<Text>();
		TxTutorial.text = steps[0];
		Target = GameObject.Find("Target");
		CardItemTarget = GameObject.Find("CardItems/CardItemTarget");
		// Set class
		players = gameObject.GetComponent<Players>();
		gameController = gameObject.GetComponent<GameController>();
		cards = gameObject.GetComponent<Cards>();
	}

    private void SetStepByMovement(int step)
    {
		if (step >= 4)
        {
			return;
        }
        TxTutorial.text = steps[step + 1];
		switch (currentStep)
		{
			case 0:
				Target.transform.position = new Vector3(2, 0.01f, -2);
				break;
			case 1:
				Target.transform.position = new Vector3(-2, 0.01f, -2);
				break;
			case 2:
				Target.transform.position = new Vector3(-2, 0.01f, 2);
				CardItemTarget.transform.position = new Vector3(-2, 0.1f, 2);
				break;
			case 3:
				CardItemTarget.SetActive(false);
				cards.FindCard(player1, 0);
				break;
			default:
				break;
		}
		currentStep = step + 1;
    }

    // Update is called once per frame
    void Update () {
		if (players.Meet(gameController.Player[player1], Target))
        {
			SetStepByMovement(currentStep);
		}
	}
}
