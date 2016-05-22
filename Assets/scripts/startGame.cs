using UnityEngine;
using System.Collections;

public class startGame : MonoBehaviour {


	float timeLeft = 3.0f;
	
	void Update()
	{
		timeLeft -= Time.deltaTime;
		if(timeLeft < 0)
		{
			Application.LoadLevel("mainScene");
		}
	}

	// Use this for initialization
	void Start () {
	
	}

}
