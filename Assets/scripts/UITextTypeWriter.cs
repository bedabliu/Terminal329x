﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UITextTypeWriter : MonoBehaviour 
{
	
	Text txt;
	string story;
	
	void Awake () 
	{
		txt = GetComponent<Text> ();
		story = txt.text;
		txt.text = "";
		
		// TODO: add optional delay when to start
		StartCoroutine ("PlayText");
	}

	public void animate(){
		txt = GetComponent<Text> ();
		story = txt.text;
		txt.text = "";
		
		// TODO: add optional delay when to start
		StartCoroutine ("PlayText");
	}
	
	IEnumerator PlayText()
	{
		foreach (char c in story) 
		{
			txt.text += c;
			yield return new WaitForSeconds (0.1f);
		}
	}
	
}