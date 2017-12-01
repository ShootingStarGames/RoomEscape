using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallQuiz : MonoBehaviour {

    public GameObject panel;

    void OnMouseDown()
    {
        OpenPanel();
    }
    public void OpenPanel()
    {
        panel.SetActive(true);
        Screen.lockCursor = false;
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
