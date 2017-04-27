using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class textScript : MonoBehaviour {

    Text myText;

	// Use this for initialization
	void Start () {
        myText = GetComponent<Text>();
	}
	
    //replace text on the screen
    public void updateText(string s)
    {
        myText.text = s;
    }

    //replace text on the screen
    public string getText()
    {
        return myText.text;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
