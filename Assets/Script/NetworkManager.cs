using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.UI;
using System.Text.RegularExpressions;


public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public InputField playerNameInput,roomNameInput;
    public GameObject player;
    public SocketIOComponent socket;

    private bool[] ObjectBoolList;
    private static string roomKey;
    //private AddressJson addressJson;
    //private P2PNetworkManager p2pManager;
    //private string talkAddress = "";
    //private SpeechQueue speechQueue;
    // Use this for initialization
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject); 
    }

    private void Start()
    {
        ObjectBoolList = new bool[6];
        //p2pManager = new P2PNetworkManager();
    }

    public static NetworkManager getInstance()
    {
        return instance;
    }

    private void InitServer()
    {
        //socket.On("player address", OnPlayerAddress);
        socket.On("create room", OnCreateRoom);
        socket.On("object list", OnObjectSet);

        socket.On("room list", OnRoomShowList);
        socket.On("rejected room", OnRejectedRoom);
        socket.On("other player connected", OnOhterPlayerConnected);
        socket.On("player connected", OnPlayerConneted);
        socket.On("play", OnPlay);
        socket.On("player move", OnPlayerMove);
        socket.On("player turn", OnPlayerTurn);
        socket.On("other player disconnect", OtherPlayerDisconnected);

        socket.On("ClickEvent", MClickEvent);
    }

    public bool[] getObjectBoolList()
    {
        return ObjectBoolList;
    }
    #region etc
    //void OnApplicationQuit()
    //{
    //    p2pManager.Dispose();
    //}

    private bool IsCorrectCompanyName(string s)
    {
        if (!Regex.IsMatch(s, @"[^0-9가-힣a-zA-Z]"))
        {
            if (s.Length != 0)
                return true;
        }
        return false;
    }

    public void JoinServer()
    {
        InitServer();
        ShowRoomList();
    }

    public void CreateRoom()
    {
        StartCoroutine(CreateToRoom());
    }

    public void JoinRoom(string _roomKey)
    {
        StartCoroutine(CheckRoom(_roomKey));
    }

    //IEnumerator SpeechOther()
    //{
    //    while (true)
    //    {
    //        if (speechQueue.Check())
    //        {
    //            float[] _buffer = speechQueue.Pop();
    //            GameObject o = GameObject.Find(addressJson.name) as GameObject;
    //            if (o != null)
    //            {
    //                AudioSource audioSource = o.GetComponent<AudioSource>();
    //                audioSource.clip.SetData(_buffer, 0);
    //            }
    //            else
    //            {
    //                StopCoroutine(SpeechOther());
    //            }
    //        }
    //        yield return new WaitForSeconds(1);
    //    }
    //}
    #endregion

    #region Commands
    IEnumerator CreateToRoom()
    {
        if (IsCorrectCompanyName(roomNameInput.text) && IsCorrectCompanyName(playerNameInput.text))
        {
            string data = JsonUtility.ToJson(new RoomJSON(roomNameInput.text, "",""));
            socket.Emit("create room", new JSONObject(data));
            yield return new WaitForSeconds(0.1f);
            JoinRoom(roomKey);
            ShowRoomList();
        }
    }

    IEnumerator CheckRoom(string _roomKey)
    {
        string playerName = playerNameInput.text;

        if (IsCorrectCompanyName(playerName))
        {
            string data = JsonUtility.ToJson(new RoomJSON(roomNameInput.text, _roomKey, playerName));
            socket.Emit("player connect room", new JSONObject(data));
        }
        yield return null;
    }

    private void ConnectToRoom(string _roomKey)
    {
        string playerName = playerNameInput.text;

        if (IsCorrectCompanyName(playerName))
        {
            List<SpawnPoint> playerSpawnPoints = GetComponent<SpawnManager>().playerSpawnPoints;
            PlayerJSON playerJSON = new PlayerJSON(playerName, playerSpawnPoints, _roomKey);
            string data = JsonUtility.ToJson(playerJSON);
            socket.Emit("play", new JSONObject(data));
            UIManager.instance.CloseRoomPanel();
            UIManager.instance.OpenPlayerPanel();
        }
    }

    public void ShowRoomList()
    {
        socket.Emit("room list");
    }

    public void CommandMove(Vector3 vec3)
    {
        string data = JsonUtility.ToJson(new PositionJSON(vec3));
        socket.Emit("player move", new JSONObject(data));
    }

    public void CommandTurn(Quaternion quat)
    {
        string data = JsonUtility.ToJson(new RotationJSON(quat));
        socket.Emit("player turn", new JSONObject(data));
    }

    public void SendObject(int i)
    {
        ObjectBoolList[i] = !ObjectBoolList[i];
        string data = JsonUtility.ToJson(new ObjectJSON(ObjectBoolList));
        socket.Emit("object list", new JSONObject(data));
    }
     
    //public void StartChat()
    //{
    //    socket.Emit("player talk");
    //}

    public void Disconect()
    {
        socket.Emit("disconnect");
        UIManager.instance.OpenRoomPanel();
        UIManager.instance.ClosePlayerPanel();
        UIManager.instance.CloseStopPanel();
        Destroy(GameObject.Find(playerNameInput.text));
    }

    //public void SendVoice(float[] _audio)
    //{
    //    if (addressJson != null && p2pManager.GetConnect())
    //        p2pManager.Send(_audio);
    //}
    #endregion

    #region Listening
    //public void PushOtherSpeech(float[] _buffer)
    //{
    //    if (addressJson != null && p2pManager.GetConnect())
    //    {
    //        speechQueue.Push(_buffer);
    //    }
    //}

    //void OnPlayerAddress(SocketIOEvent socketIOEvent)
    //{
    //    string data = socketIOEvent.data.ToString();
    //    addressJson = JsonUtility.FromJson<AddressJson>(data);
    //    if (talkAddress != addressJson.address)
    //    {
    //        if (p2pManager.GetConnect())
    //        {
    //            p2pManager.Dispose();
    //            speechQueue = null;
    //        }
    //        talkAddress = addressJson.address;
    //        p2pManager.CreateP2PNetworkManager(addressJson.address, addressJson.port);
    //        //speechQueue = new SpeechQueue();
    //        //StartCoroutine(SpeechOther());
    //    }
    //}

    void MClickEvent(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        Debug.Log(data);
    }

    void OnRejectedRoom(SocketIOEvent socketIOEvent)
    {
        print("OnRejectedRoom");
        UIManager.instance.OpenRoomPanel();
        UIManager.instance.ClosePlayerPanel();
        UIManager.instance.CloseStopPanel();
    }

    void OnObjectSet(SocketIOEvent socketIOEvent)
    {
        print("OnObjectSet");
        string data = socketIOEvent.data.ToString();
        ObjectJSON objectJSON = ObjectJSON.CreateFromJson(data);
        ObjectBoolList = (bool[])objectJSON.obj.Clone();
        ObjectManager.instance.setObjectList(objectJSON.obj);
    }

    void OnPlayerConneted(SocketIOEvent socketIOEvent)
    {
        ConnectToRoom(socketIOEvent.data.list[1].str);
    }

    void OnCreateRoom(SocketIOEvent socketIOEvent)
    {
        roomKey = socketIOEvent.data.list[0].str;
    }

    void OnRoomShowList(SocketIOEvent socketIOEvent)
    {
        RoomManager.getInstance().ShowList(socketIOEvent.data);
    }

    void OnOhterPlayerConnected(SocketIOEvent socketIOEvent)
    {
        print("Someone else joined");
        string data = socketIOEvent.data.ToString();
        UserJSON userJSON = UserJSON.CreateFromJson(data);
        Vector3 position = new Vector3(userJSON.position[0], userJSON.position[1], userJSON.position[2]);
        Quaternion rotation = Quaternion.Euler(userJSON.rotation[0], userJSON.rotation[1], userJSON.rotation[2]);
        GameObject o = GameObject.Find(userJSON.name) as GameObject;

        if (o != null)
        {
            return;
        }
        GameObject p = Instantiate(player, position, rotation) as GameObject;
        ControllManager pm = p.GetComponent<ControllManager>();
        Transform t = p.transform.Find("Player Name_");
        TextMesh playerName = t.GetComponent<TextMesh>();
        playerName.text = userJSON.name;
        pm.isMine = false;
        p.name = userJSON.name;
    }

    void OnPlay(SocketIOEvent socketIOEvent)
    {
        print("you joind");
        string data = socketIOEvent.data.ToString();
        UserJSON currentUserJSON = UserJSON.CreateFromJson(data);
        Vector3 position = new Vector3(currentUserJSON.position[0], currentUserJSON.position[1], currentUserJSON.position[2]);
        Quaternion rotation = Quaternion.Euler(currentUserJSON.rotation[0], currentUserJSON.rotation[1], currentUserJSON.rotation[2]);
        GameObject p = Instantiate(player, position, rotation) as GameObject;
        p.GetComponent<AudioSource>().mute = true;
        GameObject c = new GameObject();
        c.name = "Camera";
        c.AddComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));
        c.transform.position = p.transform.position;
        c.transform.rotation = p.transform.rotation;
        c.transform.SetParent(p.transform);
        ControllManager pm = p.GetComponent<ControllManager>();
        pm.myCamera = c.GetComponent<Camera>();
        p.layer = 8;
        foreach (Transform child in p.GetComponentInChildren<Transform>())
        {
            child.gameObject.layer = 8;
        }
        Transform t = p.transform.Find("Player Name_");
        TextMesh playerName = t.GetComponent<TextMesh>();
        playerName.text = currentUserJSON.name;
        pm.isMine = true;
        p.name = currentUserJSON.name;
    }

    void OnPlayerMove(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        UserJSON userJSON = UserJSON.CreateFromJson(data);
        Vector3 postion = new Vector3(userJSON.position[0], userJSON.position[1], userJSON.position[2]);
        if(userJSON.name == playerNameInput.text)
        {
            return;
        }
        GameObject p = GameObject.Find(userJSON.name) as GameObject;
        if (p != null)
        {
            p.transform.position = postion;
        }
    }

    void OnPlayerTurn(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        UserJSON userJSON = UserJSON.CreateFromJson(data);
        Quaternion rotation = Quaternion.Euler(userJSON.rotation[0], userJSON.rotation[1], userJSON.rotation[2]);
        if (userJSON.name == playerNameInput.text)
        {
            return;
        }
        GameObject p = GameObject.Find(userJSON.name) as GameObject;
        if (p != null)
        {
            p.transform.rotation = rotation;
        }
    }

    void OtherPlayerDisconnected(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        UserJSON userJSON = UserJSON.CreateFromJson(data);
        //addressJson = null;
        Destroy(GameObject.Find(userJSON.name));
        //StopCoroutine(SpeechOther());
    }
    #endregion

    #region MessageClass
    //[SerializeField]
    //public class AddressJson
    //{
    //    public string address;
    //    public int port;
    //    public string name;

    //    public AddressJson(string _address, int _port,string _name)
    //    {
    //        address = _address;
    //        port = _port;
    //        name = _name;
    //    }
    //}

    [SerializeField]
    public class ObjectJSON
    {
        public bool[] obj;

        public ObjectJSON(bool[] _obj)
        {
            obj = _obj;
        }
        public static ObjectJSON CreateFromJson(string data)
        {
            return JsonUtility.FromJson<ObjectJSON>(data);
        }
    }

    [SerializeField]
    public class RoomJSON
    {
        public string roomName;
        public string key;
        public string name;

        public RoomJSON(string _roomName, string _key,string _name)
        {
            roomName = _roomName;
            key = _key;
            name = _name;
        }
    }

    [SerializeField]
    public class PlayerJSON
    {
        public string name;
        public string room;
        public List<PointJSON> playerSpawnPoints;
        public string socketId;

        public PlayerJSON(string _name,List<SpawnPoint> _playerSpawnPoints,string _room)
        {
            playerSpawnPoints = new List<PointJSON>();
            name = _name;
            room = _room;

            foreach (SpawnPoint playerSpawnPoint in _playerSpawnPoints)
            {
                PointJSON pointJSON = new PointJSON(playerSpawnPoint);
                playerSpawnPoints.Add(pointJSON);
            }
        }
    }

    [System.Serializable]
    public class PointJSON
    {
        public float[] position;
        public float[] rotation;
        public PointJSON(SpawnPoint spawnPoint)
        {
            position = new float[] {
                spawnPoint.transform.position.x,
                spawnPoint.transform.position.y,
                spawnPoint.transform.position.z
            };
            rotation = new float[] {
                spawnPoint.transform.eulerAngles.x,
                spawnPoint.transform.eulerAngles.y,
                spawnPoint.transform.eulerAngles.z
            };
        }

    }

    [System.Serializable]
    public class PositionJSON
    {
        public float[] position;

        public PositionJSON(Vector3 _position)
        {
            position = new float[] { _position.x, _position.y, _position.z };
        }
    }

    [System.Serializable]
    public class RotationJSON
    {
        public float[] rotation;

        public RotationJSON(Quaternion _rotation)
        {
            rotation = new float[] { _rotation.eulerAngles.x, _rotation.eulerAngles.y, _rotation.eulerAngles.z };
        }
    }

    [SerializeField]
    public class UserJSON
    {
        public string name;
        public float[] position;
        public float[] rotation;
        public string room;
        public string socketId;
        public int role;

        public static UserJSON CreateFromJson(string data)
        {
            return JsonUtility.FromJson<UserJSON>(data);
        }
    }   
    #endregion



}
