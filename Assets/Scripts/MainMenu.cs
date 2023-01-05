using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
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
	// Enum game control type
	enum GameControl
    {
		KEYBOARD = 0,
		SENSORS = 1
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

	// Load scene and set game control
	private void LoadScene(GameControl gameControl)
	{
		SceneManager.LoadScene("Game");
		Base.gameControl = (int)gameControl;
	}

	private void BtnStartOnClick()
	{
		ActivePanel(PanelTarget.START);
		// Set onClick events
		GameObject.Find("BtnKeyboard").GetComponent<Button>().onClick.AddListener(KeyboardOnClick);
		GameObject.Find("BtnSensors").GetComponent<Button>().onClick.AddListener(SensorsOnClick);
	}

	private void KeyboardOnClick()
	{
		LoadScene(GameControl.KEYBOARD);
	}

	private void SensorsOnClick()
	{
		LoadScene(GameControl.SENSORS);
	}

	private void BtnHowToPlayOnClick()
	{
		ActivePanel(PanelTarget.HOWTOPLAY);
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
