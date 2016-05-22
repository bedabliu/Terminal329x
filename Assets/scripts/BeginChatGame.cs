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
		desactivateButtons ();
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
			yield return new WaitForSeconds(20);
			statusConnection.GetComponent<Text>().text = "Connected";
		}
		if (story.GetField ("story").list [line].GetField ("problem").i > 0) {
			buttonOne.SetActive(false);
			buttonTwo.SetActive(false);
			yield return new WaitForSeconds(10);
			savePoint = (int) story.GetField ("story").list [line].GetField ("problem").i;
			callNextLines((int) story.GetField ("story").list [line].GetField ("problem").i, true);
		} else {
			buttonOne.SetActive (true);
			buttonTwo.SetActive (true);
			buttonContinue.SetActive (false);
			buttonOne.GetComponentInChildren<Text> ().text = story.GetField ("story").list [line].GetField ("optionOneTextline1").str + "\n" + story.GetField ("story").list [line].GetField ("optionOneTextline2").str;
			buttonTwo.GetComponentInChildren<Text> ().text = story.GetField ("story").list [line].GetField ("optionTwoTextline1").str + "\n" + story.GetField ("story").list [line].GetField ("optionTwoTextline2").str;
		}
		yield return new WaitForSeconds (8);
		activateButtons ();
	}

	void instantiateAnswer(bool answer){
		string response;
		hasProblemInTheWay = false;
		if (answer) {
			if((int) story.GetField ("story").list[savePoint].GetField ("optionOnewayProblem").i > 0){
				hasProblemInTheWay = true;
			}
			response = story.GetField ("story").list [savePoint].GetField ("optionOneTextline1").str + "\n" + story.GetField ("story").list [savePoint].GetField ("optionOneTextline2").str;
			savePoint = (int) story.GetField ("story").list [savePoint].GetField ("optionOneway").i;
		} else {
			if((int) story.GetField ("story").list [savePoint].GetField ("optiontwowayProblem").i > 0){
				hasProblemInTheWay = true;
			}
			response = story.GetField ("story").list [savePoint].GetField ("optionTwoTextline1").str + "\n" + story.GetField ("story").list [savePoint].GetField ("optionTwoTextline2").str;
			savePoint = (int) story.GetField ("story").list [savePoint].GetField ("optiontwoway").i;
		}
		GameObject textPcl1 = Instantiate (prefabTextPc) as GameObject;
		textPcl1.transform.SetParent (panelChats.transform, false);
		Transform npcText = panelChats.transform.GetChild(0);
		textPcl1.transform.localPosition = new Vector3(390f, (-1*(npcText.GetComponent<RectTransform>().rect.height+80)), 0f);
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

	public void activateButtons(){
		buttonOne.GetComponent<Button>().interactable = true;
		buttonTwo.GetComponent<Button>().interactable = true;
	}

	public void desactivateButtons(){
		buttonOne.GetComponent<Button>().interactable = false;
		buttonTwo.GetComponent<Button>().interactable = false;
	}

	public void continueTheAdventure(){
		if (!story.GetField ("story").list [savePoint].GetField ("line1").str.Equals ("")) {
			buttonContinue.SetActive(false);
			Debug.Log(hasProblemInTheWay);
			callNextLines (savePoint, hasProblemInTheWay);
		}
	}
}
