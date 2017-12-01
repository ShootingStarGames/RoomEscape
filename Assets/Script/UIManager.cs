using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager instance;
    public InputField inputField;
    public GameObject startPanel, roomPanel, playerPanel, stopPanel;
    public InputField quizField;
    public GameObject quizObjet;

    string playerName;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

    }

    #region OpenPanel
    public void OpenStartPanel()
    {
        startPanel.SetActive(true);
    }
    public void OpenRoomPanel()
    {
        //roomPanel.SetActive(true);
    }
    public void OpenStopPanel()
    {
        stopPanel.SetActive(true);
    }
    public void OpenPlayerPanel()
    {
        playerPanel.SetActive(true);
    }
    #endregion

    #region ClosePanel
    public void CloseStartPanel()
    {
        startPanel.SetActive(false);
    }
    public void CloseRoomPanel()
    {
        roomPanel.SetActive(false);
    }
    public void CloseStopPanel()
    {
        stopPanel.SetActive(false);
    }
    public void ClosePlayerPanel()
    {
        playerPanel.SetActive(false);
    }
    #endregion

    #region Command
    public void BackToPlay()
    {
        playerName = inputField.text;
        GameObject p = GameObject.Find(playerName) as GameObject;
        CloseStopPanel();
        OpenPlayerPanel();
        p.GetComponent<ControllManager>().isMine = true;
        Screen.lockCursor = true;
    }
    
    public void BackFromQuiz()
    {
        Screen.lockCursor = true;
        string s = "154";
        if (quizField.text == s)
        {
            quizObjet.GetComponent<ObjectScript>().locked = false;
            quizObjet.GetComponent<ObjectScript>().ActiveObject();
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
