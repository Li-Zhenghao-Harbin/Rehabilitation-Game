using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

	public GameObject Bgm;
	GameObject BGM;

	// Use this for initialization
	void Start () {
		BGM = GameObject.FindGameObjectWithTag("sound");
		if (BGM == null)
        {
			BGM = (GameObject)Instantiate(Bgm);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
