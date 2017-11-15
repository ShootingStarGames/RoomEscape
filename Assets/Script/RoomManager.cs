using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {

    private static RoomManager _instance;
    private GameObject _roomObj;
    private Transform _contentFrame;
    public static RoomManager getInstance()
    {
        if (_instance == null)
            _instance = GameObject.FindObjectOfType(typeof(RoomManager)) as RoomManager;
 
        return _instance;
    }

    public void ShowList(JSONObject listObj)
    {
        if(_contentFrame==null)
            _contentFrame = GameObject.FindGameObjectWithTag("Content").transform;

        List<JSONObject> list = listObj.list;

        foreach (Transform child in _contentFrame)
        {
            Destroy(child.gameObject);
        }

        if (list[0].list.Count == 0)
            return;

        for(int i = 0; i < list[0].Count; i++)
        {
            GameObject roomObj = Instantiate(_roomObj);
            roomObj.transform.SetParent(_contentFrame);
            roomObj.transform.GetChild(1).name = list[0].list[i].list[0].str;
            roomObj.transform.GetChild(2).GetComponent<UnityEngine.UI.Text>().text = list[0].list[i].list[1].str;
            roomObj.transform.GetChild(1).gameObject.GetComponent<JoinRoomScript>().InitButton();
            roomObj.transform.localScale = Vector3.one;
        }

    }

	// Use this for initialization
	void Start () {
        _roomObj = Resources.Load("Prefabs/Room_") as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
