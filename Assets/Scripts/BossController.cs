using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : Base {

	// Boss warning
	public float bossWarningTimer = 2f;
	public static Text TxBossWarning;
	public static Text TxBossWarningSuggestion;
	// Buff
	public static GameObject[] PnPlayerBuffs = new GameObject[playerCount];
	public static GameObject[] PnBossBuffs = new GameObject[playerCount];
	// Class
	Medusa medusa;

	// Use this for initialization
	void Start () {
		if (bossTitle == GetBossTitle(BossTitle.TUTORIAL))
        {
			return;
        }
		// Set boss warning
		if (bossTitle != GetBossTitle(BossTitle.TUTORIAL))
        {
			TxBossWarning = GameObject.Find("TxBossWarning").GetComponent<Text>();
			TxBossWarningSuggestion = GameObject.Find("TxBossWarningSuggestion").GetComponent<Text>();
        }
		// Set buff
		PnPlayerBuffs[player1] = GameObject.Find("PnPlayerBuff1");
		PnPlayerBuffs[player2] = GameObject.Find("PnPlayerBuff2");
		PnBossBuffs[player1] = GameObject.Find("PnBossBuff1");
		PnBossBuffs[player2] = GameObject.Find("PnBossBuff2");
		PnPlayerBuffs[player1].SetActive(false);
		PnPlayerBuffs[player2].SetActive(false);
		PnBossBuffs[player1].SetActive(false);
		PnBossBuffs[player2].SetActive(false);
		// Set class
		medusa = gameObject.GetComponent<Medusa>();
	}

	public void SetWarning(string title, string suggestion)
    {
        TxBossWarning.text = title;
        TxBossWarningSuggestion.text = suggestion;
    }

	public void SetWarning(bool val = false)
    {
		TxBossWarning.text = TxBossWarningSuggestion.text = "";
	}

	public void DisplayBuff(GameObject[] gameObject, int index, bool val = true)
    {
		gameObject[index].SetActive(val);
    }

	// Update is called once per frame
	void Update () {

	}
}
