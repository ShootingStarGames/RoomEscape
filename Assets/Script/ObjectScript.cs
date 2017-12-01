using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScript : MonoBehaviour {

    public int index;

    float speed = 0.2f;

    bool on = false;
    bool draged = false;
    public bool locked = true;
    private Camera camera;

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

    public void free()
    {
        locked = false;
    }
    public void locking()
    {
        locked = true;
    }
    public int getIndex()
    {
        return index;
    }

    public void ActiveObject()
    {
        if (locked)
            return;
        NetworkManager.instance.SendObject(index);
    }

    public void Drag(Camera camera)
    {
        this.camera = camera;
        draged = true;
    }
    public void Drop()
    {
        draged = false;
    }

    private void Dragging()
    {
        transform.position = this.camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1f));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "lockObj")
        {
            collision.transform.parent.gameObject.SendMessage("free");
            Destroy(this.gameObject);
        }
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(draged)
            Dragging();
        if (transform.position.y <= -10)
            transform.position = new Vector3(8, 2,5);
    }
}
