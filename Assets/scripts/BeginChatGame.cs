using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BeginChatGame : MonoBehaviour {

	public TextAsset StoryJson;
	public GameObject panelChats;
	public GameObject prefabTextNpc;
	public GameObject prefabTextPc;
	public GameObject buttonOne;
	public GameObject buttonTwo;
	public GameObject buttonContinue;
	public GameObject statusConnection;
	JSONObject story;
	int savePoint;
	bool hasProblemInTheWay;

	// Use this for initialization
	void Start () {
		story = new JSONObject(StoryJson.text);
		if (PlayerPrefs.HasKey ("saveState")) {
			Debug.Log("has save!");
			savePoint = PlayerPrefs.GetInt("saveState");
		}{
			Debug.Log("StartNewGame");
			savePoint = 0;
			startNewGame();
		};
	}
	
	// Update is called once per frame
	void Update () {
	}

	void startNewGame(){
		callNextLines (savePoint, false);
	}

	void callNextLines(int line, bool problem){
		StartCoroutine (instantiateNewLines(line, problem));
	}

	IEnumerator instantiateNewLines(int line, bool problem){
		buttonOne.SetActive (true);
		buttonTwo.SetActive (true);
		buttonContinue.SetActive (false);
		int childs = panelChats.transform.childCount;
		for (int i = childs - 1; i >= 0; i--)
		{
			GameObject.Destroy(panelChats.transform.GetChild(i).gameObject);
		}
		GameObject textNPCl1 = Instantiate(prefabTextNpc) as GameObject;
		textNPCl1.GetComponent<Text>().text = story.GetField("story").list[line].GetField("line1").str + "\n" + story.GetField("story").list[line].GetField("line2").str;
		textNPCl1.transform.SetParent (panelChats.transform, false);
		textNPCl1.GetComponent<UITextTypeWriter> ().animate ();
		if (problem) {
			statusConnection.GetComponent<Text>().text = "Lost...";
			yield return new WaitForSeconds(10);
			statusConnection.GetComponent<Text>().text = "Connected";
		}
		if (story.GetField ("story").list [line].GetField ("problem").i > 0) {
			buttonOne.GetComponentInChildren<Text> ().text = "";
			buttonTwo.GetComponentInChildren<Text> ().text = "";
			buttonOne.SetActive(false);
			buttonTwo.SetActive(false);
			yield return new WaitForSeconds(10);
			callNextLines((int) story.GetField ("story").list [line].GetField ("problem").i, true);
		} else {
			buttonOne.GetComponentInChildren<Text> ().text = story.GetField ("story").list [line].GetField ("optionOneTextline1").str + "\n" + story.GetField ("story").list [line].GetField ("optionOneTextline2").str;
			buttonTwo.GetComponentInChildren<Text> ().text = story.GetField ("story").list [line].GetField ("optionTwoTextline1").str + "\n" + story.GetField ("story").list [line].GetField ("optionTwoTextline2").str;
		}
	}

	void instantiateAnswer(bool answer){
		string response;
		hasProblemInTheWay = false;
		if (answer) {
			if(story.GetField ("story").list [savePoint].GetField ("optionOnewayProblem").i == -1){
				hasProblemInTheWay = true;
			}
			response = story.GetField ("story").list [savePoint].GetField ("optionOneTextline1").str + "\n" + story.GetField ("story").list [savePoint].GetField ("optionOneTextline2").str;
			savePoint = (int) story.GetField ("story").list [savePoint].GetField ("optionOneway").i;
		} else {
			if(story.GetField ("story").list [savePoint].GetField ("optiontwowayProblem").i == -1){
				hasProblemInTheWay = true;
			}
			response = story.GetField ("story").list [savePoint].GetField ("optionTwoTextline1").str + "\n" + story.GetField ("story").list [savePoint].GetField ("optionTwoTextline2").str;
			savePoint = (int) story.GetField ("story").list [savePoint].GetField ("optiontwoway").i;
		}
		GameObject textPcl1 = Instantiate (prefabTextPc) as GameObject;
		textPcl1.transform.SetParent (panelChats.transform, false);
		Transform npcText = panelChats.transform.GetChild(0);
		textPcl1.transform.localPosition = new Vector3(390f, (-1*(npcText.GetComponent<RectTransform>().rect.height+30)), 0f);
		textPcl1.GetComponent<Text> ().text = response;
		textPcl1.GetComponent<UITextTypeWriter> ().animate ();
		buttonOne.SetActive (false);
		buttonTwo.SetActive (false);
		buttonContinue.SetActive (true);
	}

	public void answerTheNpcButtonOne() {
		instantiateAnswer (true);
	} 

	public void answerTheNpcButtonTwo() {
		instantiateAnswer (false);
	} 

	public void continueTheAdventure(){
		if (!story.GetField ("story").list [savePoint].GetField ("line1").str.Equals ("")) {
			callNextLines (savePoint, hasProblemInTheWay);
		}
	}
}
