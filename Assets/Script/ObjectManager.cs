using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {
    public GameObject discriptionUI;
    public static ObjectManager instance;
    float speed = 0.2f;

    #region ActiveObject
    public void OpenDiscription(GameObject _obj)
    {
        StartCoroutine(DiscriptionUI(_obj));
    }

    public void SpinOpenObj(GameObject _obj)
    {
        StartCoroutine(CoroutineSpinOpenObj(_obj));
    }

    public void SpinCloseObj(GameObject _obj)
    {
        StartCoroutine(CoroutineSpinCloseObj(_obj));
    }

    public void DrawOpenObj(GameObject _obj)
    {
        StartCoroutine(CoroutineDrawOpenObj(_obj));
    }

    public void DrawCloseObj(GameObject _obj)
    {
        StartCoroutine(CoroutineDrawCloseObj(_obj));
    }
    #endregion

    #region Coroutine
    IEnumerator DiscriptionUI(GameObject _obj)
    {
        discriptionUI.SetActive(true);
        Transform child = discriptionUI.transform.GetChild(0);
        child.GetComponent<UnityEngine.UI.Text>().text = "안뇽하세요!!!";

        yield return new WaitForSeconds(3f);
        discriptionUI.SetActive(false);
    }
    IEnumerator CoroutineSpinOpenObj(GameObject _obj)
    {
        float startTime = Time.time;
        float currentTime;
        while (_obj.transform.eulerAngles.y != 90)
        {
            currentTime = Time.time - startTime;
            _obj.transform.rotation = Quaternion.Lerp(_obj.transform.rotation, Quaternion.Euler(0, 90, 0), currentTime * speed);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator CoroutineSpinCloseObj(GameObject _obj)
    {
        float startTime = Time.time;
        float currentTime;
        while (_obj.transform.eulerAngles.y != 0)
        {
            currentTime = Time.time - startTime;
            _obj.transform.rotation = Quaternion.Lerp(_obj.transform.rotation, Quaternion.Euler(0, 0, 0), currentTime * speed);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator CoroutineDrawOpenObj(GameObject _obj)
    {
        float startTime = Time.time;
        float currentTime;
        while (_obj.transform.localPosition.z !=0.2f)
        {
            currentTime = Time.time - startTime;
            _obj.transform.localPosition = Vector3.Lerp(_obj.transform.localPosition,new Vector3(0,0,0.2f), currentTime * speed*2);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator CoroutineDrawCloseObj(GameObject _obj)
    {
        float startTime = Time.time;
        float currentTime;
        while (_obj.transform.localPosition.z !=0)
        {
            currentTime = Time.time - startTime;
            _obj.transform.localPosition = Vector3.Lerp(_obj.transform.localPosition, new Vector3(0, 0, 0), currentTime * speed*2);
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

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
