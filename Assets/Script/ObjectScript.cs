using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScript : MonoBehaviour {
    float speed = 0.2f;

    bool on = false;

    public void InteractiveObj(bool b)
    {
        if (this.name == "Pillar_")
        {
            if (b)
            {
                StartCoroutine(CoroutineSpinOpenObj(this.gameObject));
            }
            else
            {
                StartCoroutine(CoroutineSpinCloseObj(this.gameObject));
            }

        }
        else if(this.name == "DrawerParent_")
        {
            if (b)
            {
                StartCoroutine(CoroutineDrawOpenObj(this.gameObject));
            }
            else
            {
                StartCoroutine(CoroutineDrawCloseObj(this.gameObject));
            }
        }
    }

    //IEnumerator DiscriptionUI(GameObject _obj)
    //{
    //    discriptionUI.SetActive(true);
    //    Transform child = discriptionUI.transform.GetChild(0);
    //    child.GetComponent<UnityEngine.UI.Text>().text = "안뇽하세요!!!";

    //    yield return new WaitForSeconds(3f);
    //    discriptionUI.SetActive(false);
    //}

    IEnumerator CoroutineSpinOpenObj(GameObject _obj)
    {
        float startTime = Time.time;
        float currentTime;
        on = true;
        while (_obj.transform.eulerAngles.y != 90 && on)
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
        on = false;
        while (_obj.transform.eulerAngles.y != 0 &&! on)
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
        on = true;
        while (_obj.transform.localPosition.z != 0.2f && on)
        {
            currentTime = Time.time - startTime;
            _obj.transform.localPosition = Vector3.Lerp(_obj.transform.localPosition, new Vector3(0, 0, 0.2f), currentTime * speed * 2);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator CoroutineDrawCloseObj(GameObject _obj)
    {
        float startTime = Time.time;
        float currentTime;
        on = false;
        while (_obj.transform.localPosition.z != 0 && !on)
        {
            currentTime = Time.time - startTime;
            _obj.transform.localPosition = Vector3.Lerp(_obj.transform.localPosition, new Vector3(0, 0, 0), currentTime * speed * 2);
            yield return new WaitForEndOfFrame();
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
