using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinRoomScript : MonoBehaviour {

    private string key;

    public void InitButton()
    {
        key = this.gameObject.name;
    }

    public void JoinRoom()
    {
        NetworkManager.getInstance().JoinRoom(key);
    }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
