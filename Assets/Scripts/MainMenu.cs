using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;

public class MainMenu : Base
{

	// GameObjects
	GameObject PnStart;
	GameObject PnWelcome;
	GameObject PnHowToPlay;
	GameObject PnOptions;
	GameObject PnAbout;
	List<GameObject> mainPanels = new List<GameObject>();
	Toggle TgLog;

	// Enum panel target, used to indicate which panel to be actived
	enum PanelTarget
	{
		NONE = -1,
		WELCOME = 0,
		START = 1,
		HOWTOPLAY = 2,
		OPTIONS = 3,
		ABOUT = 4,
	}

    // Use this for initialization
    void Start()
	{
		// Set GameObjects
		PnWelcome = GameObject.Find("PnWelcome");
		mainPanels.Add(PnWelcome);
		PnStart = GameObject.Find("PnStart");
		mainPanels.Add(PnStart);
		PnHowToPlay = GameObject.Find("PnHowToPlay");
		mainPanels.Add(PnHowToPlay);
		PnOptions = GameObject.Find("PnOptions");
		mainPanels.Add(PnOptions);
		PnAbout = GameObject.Find("PnAbout");
		mainPanels.Add(PnAbout);
		// Load panel
		ActivePanel(PanelTarget.WELCOME);
		// Set onClick events
		GameObject.Find("BtnStart").GetComponent<Button>().onClick.AddListener(BtnStartOnClick);
		GameObject.Find("BtnHowToPlay").GetComponent<Button>().onClick.AddListener(BtnHowToPlayOnClick);
		GameObject.Find("BtnOptions").GetComponent<Button>().onClick.AddListener(BtnOptionsOnClick);
		GameObject.Find("BtnAbout").GetComponent<Button>().onClick.AddListener(BtnAboutOnClick);
		GameObject.Find("BtnExit").GetComponent<Button>().onClick.AddListener(BtnExitOnClick);
	}

	// Display the target panel and hide the others
	private void ActivePanel(PanelTarget panelTargetValue)
	{
		for (int i = 0; i < mainPanels.Count; i++)
		{
			mainPanels[i].SetActive(i == (int)panelTargetValue);
		}
	}

	private void UpdateSelection()
    {
		StringBuilder sb = new StringBuilder();
		sb.Append(gameControl == GetGameControl(GameControl.KEYBOARD) ? "Keyboard - " : "Sensors - ");
		if (gamePlayer == GetGamePlayer(GamePlayer.SINGLE))
        {
			sb.Append("Single - ");
			SetVisible(GameObject.Find("TxGameMode"), true);
			SetVisible(GameObject.Find("BtnTutorial"), true);
			SetVisible(GameObject.Find("BtnBoss"), true);
			sb.Append(gameMode == GetGameMode(GameMode.TUTORIAL) ? "Tutorial" : "Boss");
        } else
        {
			sb.Append("Double");
			SetVisible(GameObject.Find("TxGameMode"), false);
			SetVisible(GameObject.Find("BtnTutorial"), false);
			SetVisible(GameObject.Find("BtnBoss"), false);
		}
		GameObject.Find("TxReadyToPlay").GetComponent<Text>().text = sb.ToString();
    }

	private void BtnStartOnClick()
	{
		ActivePanel(PanelTarget.START);
		// Set onClick events
		GameObject.Find("BtnKeyboard").GetComponent<Button>().onClick.AddListener(KeyboardOnClick);
		GameObject.Find("BtnSensors").GetComponent<Button>().onClick.AddListener(SensorsOnClick);
		GameObject.Find("BtnDoublePlayers").GetComponent<Button>().onClick.AddListener(DoublePlayersOnClick);
		GameObject.Find("BtnSinglePlayer").GetComponent<Button>().onClick.AddListener(SinglePlayerOnClick);
		GameObject.Find("BtnTutorial").GetComponent<Button>().onClick.AddListener(TutorialOnClick);
		GameObject.Find("BtnBoss").GetComponent<Button>().onClick.AddListener(BossOnClick);
		GameObject.Find("BtnPlay").GetComponent<Button>().onClick.AddListener(PlayOnClick);
	}

	private void KeyboardOnClick()
	{
		gameControl = GetGameControl(GameControl.KEYBOARD);
		UpdateSelection();
	}

	private void SensorsOnClick()
	{
		gameControl = GetGameControl(GameControl.SENSORS);
		UpdateSelection();
	}

	private void DoublePlayersOnClick()
    {
		gamePlayer = GetGamePlayer(GamePlayer.DOUBLE);
		UpdateSelection();
	}

	private void SinglePlayerOnClick()
    {
		gamePlayer = GetGamePlayer(GamePlayer.SINGLE);
		UpdateSelection();
	}

	private void TutorialOnClick()
    {
		gameMode = GetGameMode(GameMode.TUTORIAL);
		UpdateSelection();
	}

	private void BossOnClick()
    {
		gameMode = GetGameMode(GameMode.BOSS);
		UpdateSelection();
	}

	private void PlayOnClick()
    {
		SceneManager.LoadScene(gamePlayer == GetGamePlayer(GamePlayer.DOUBLE) ? "Game" : "Boss");
	}

	private void BtnHowToPlayOnClick()
	{
		ActivePanel(PanelTarget.HOWTOPLAY);
		UpdateSelection();
	}

	private void BtnOptionsOnClick()
	{
		ActivePanel(PanelTarget.OPTIONS);
		TgLog = GameObject.Find("TgLog").GetComponent<Toggle>();
		TgLog.isOn = Base.showLog;
		TgLog.onValueChanged.AddListener(TgLogOnValueChanged);
	}

	private void TgLogOnValueChanged(bool value)
    {
		Base.showLog = TgLog.isOn;
    }

	private void BtnAboutOnClick()
	{
		ActivePanel(PanelTarget.ABOUT);
	}

	private void BtnExitOnClick()
	{
		Application.Quit();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
