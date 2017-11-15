using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.UI;

public class MouseClickEvent : MonoBehaviour {
    public SocketIOComponent socket;

    private void Start()
    {
        //Debug.Log("hi?");
        //socket.On("ClickEvent", onClick);
    }

    void onClick(SocketIOEvent socketIOEvent)
    {
        
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("0");
            //string data = JsonUtility.ToJson("{ 'a':'A'}");
           // socket.Emit("ClickEvent", new JSONObject(data));
        }
    }
}