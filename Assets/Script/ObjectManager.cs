using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {
    public GameObject[] ObjectList; // on off obejct;

    public GameObject discriptionUI;
    public static ObjectManager instance;
    float speed = 0.2f;

    //#region ActiveObject
    //public void OpenDiscription(GameObject _obj)
    //{
    //    StartCoroutine(DiscriptionUI(_obj));
    //}

    //public void SpinOpenObj(GameObject _obj)
    //{
    //    StartCoroutine(CoroutineSpinOpenObj(_obj));
    //}

    //public void SpinCloseObj(GameObject _obj)
    //{
    //    StartCoroutine(CoroutineSpinCloseObj(_obj));
    //}

    //public void DrawOpenObj(GameObject _obj)
    //{
    //    StartCoroutine(CoroutineDrawOpenObj(_obj));
    //}

    //public void DrawCloseObj(GameObject _obj)
    //{
    //    StartCoroutine(CoroutineDrawCloseObj(_obj));
    //}
    //#endregion

    IEnumerator DiscriptionUI(GameObject _obj)
    {
        discriptionUI.SetActive(true);
        Transform child = discriptionUI.transform.GetChild(0);
        child.GetComponent<UnityEngine.UI.Text>().text = "안뇽하세요!!!";

        yield return new WaitForSeconds(3f);
        discriptionUI.SetActive(false);
    }

    public void setObjectList(bool[] _obj)
    {
        int i = 0;

        foreach (GameObject obj in ObjectList)
        {
            obj.GetComponent<ObjectScript>().InteractiveObj(_obj[i]);
            i++;
        }
    }

    // Use this for initialization
    void Start () {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        
    }
}
