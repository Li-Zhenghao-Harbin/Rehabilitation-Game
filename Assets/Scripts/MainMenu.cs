using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;
using System.IO;
using System;

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
	// Config
	string configPath;

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
		// Config
		string tempPath = Application.dataPath + "/Data/config.txt";
		if (File.Exists(tempPath))
		{
			configPath = tempPath;
		}
		else
		{
			Application.Quit();
			return;
		}
		ReadConfig();
		// Reset parameters
		gameControl = (int)GameControl.KEYBOARD;
		gamePlayer = (int)GamePlayer.DOUBLE;
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
			SetVisible(GameObject.Find("BtnMedusa"), true);
			sb.Append(bossTitle == GetBossTitle(BossTitle.TUTORIAL) ? "Tutorial" : "Medusa");
		}
		else
		{
			sb.Append("Double");
			SetVisible(GameObject.Find("TxGameMode"), false);
			SetVisible(GameObject.Find("BtnTutorial"), false);
			SetVisible(GameObject.Find("BtnMedusa"), false);
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
		GameObject.Find("BtnMedusa").GetComponent<Button>().onClick.AddListener(BossMedusaOnClick);
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
		bossTitle = GetBossTitle(BossTitle.TUTORIAL);
		UpdateSelection();
	}

	private void BossMedusaOnClick()
	{
		bossTitle = GetBossTitle(BossTitle.MEDUSA);
		UpdateSelection();
	}

	private void PlayOnClick()
	{
		if (gamePlayer == GetGamePlayer(GamePlayer.SINGLE))
		{
			SceneManager.LoadScene(bossTitle == GetBossTitle(BossTitle.TUTORIAL) ? "Tutorial" : "Medusa");
		}
		else
		{
			SceneManager.LoadScene("Game");
		}
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

	private void ReadConfig()
	{
		if (!File.Exists(configPath))
		{
			SaveConfig();
		}
		else
		{
			try
			{
				string[] lines = File.ReadAllLines(configPath);
				int n = lines.Length;
				string[,] configs = new string[n, 2];
				for (int i = 0; i < n; i++)
				{
					string[] lineValues = lines[i].Split('=');
					if (lineValues[0] == "showLog")
					{
						showLog = Convert.ToBoolean(lineValues[1]);
					}
				}
			}
			catch
			{

			}
		}
		// C# 7.0
		//if (!File.Exists(configPath))
		//{
		//	SaveConfig();
		//}
		//else
		//{
		//	string[] lines = File.ReadAllLines(configPath);
		//	Dictionary<string, bool> configs = new Dictionary<string, bool>();
		//	foreach (string s in lines)
		//	{
		//		string[] currentLine = s.Split('=');
		//		configs.Add(currentLine[0], Convert.ToBoolean(currentLine[1]));
		//	}
		//	if (configs.TryGetValue("showLog", out bool configLog))
		//	{
		//		showLog = configLog;
		//	}
		//}
	}

	private void SaveConfig()
	{
		StreamWriter streamWriter;
		FileInfo fileInfo = new FileInfo(configPath);
		streamWriter = fileInfo.CreateText();
		streamWriter.WriteLine("showLog=" + showLog.ToString());
		streamWriter.Close();
		streamWriter.Dispose();
	}

	private void BtnExitOnClick()
	{
        SaveConfig();
        Application.Quit();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
