using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllManager : MonoBehaviour {

    public bool isMine = false;
    private int id;
    private enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    private RotationAxes axes = RotationAxes.MouseXAndY;
    private float sensitivityX = 5F;
    private float sensitivityY = 5F;
    //private float minimumX = -360F;
    //private float maximumX = 360F;
    private float minimumY = -80F;
    private float maximumY = 60F;
    private float speed = 2f;
    private bool t1door = false;
    public Camera myCamera;
    Vector3 currentPositon, oldPositon;
    Quaternion currentRotation, oldRotation;
    float rotationY = 0F;
    GameObject obj;
    
    #region Controll
    void InputFunc()
    {
        InputMovement();
        InputMouseMove();
        InputMouseClick();
    }

    void InputMovement()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            speed = 4f;
        else
            speed = 2f;

        if (Input.GetKey(KeyCode.W))
            transform.localPosition += transform.forward * Time.deltaTime * speed;

        if (Input.GetKey(KeyCode.S))
            transform.localPosition -= transform.forward * Time.deltaTime * speed;

        if (Input.GetKey(KeyCode.D))
            transform.localPosition += transform.right * Time.deltaTime * speed;

        if (Input.GetKey(KeyCode.A))
            transform.localPosition -= transform.right * Time.deltaTime * speed;

        if (Input.GetKey(KeyCode.Space))
            this.GetComponent<Rigidbody>().MovePosition(transform.localPosition + Vector3.up * 2f * Time.deltaTime);

        currentPositon = transform.position;
        currentRotation = transform.rotation;

        if (currentPositon != oldPositon)
        {
            NetworkManager.instance.CommandMove(transform.position);
            oldPositon = currentPositon;
        }
        if(currentRotation != oldRotation)
        {
            NetworkManager.instance.CommandTurn(transform.rotation);
            oldRotation = currentRotation;
        }

    }

    void InputMouseMove()
    {
        if (axes == RotationAxes.MouseXAndY)
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
        else if (axes == RotationAxes.MouseX)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
        }
        else
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
        }
    }

    void InputMouseClick()
    {
        if (Input.GetKeyDown(KeyCode.E)) // press E button
        {
            RaycastHit hitInfo = new RaycastHit();
            float distance = 1f;
            Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);
            int index = 0;
            if (Physics.Raycast(ray, out hitInfo, distance))
            {
                switch (hitInfo.transform.gameObject.name)
                {
                    case "Door_":
                        obj = hitInfo.transform.parent.gameObject;
                        index = ObjectManager.instance.getObjectIndex(obj);
                        NetworkManager.instance.SendObject(index);

                        //if (obj.transform.eulerAngles.y == 0 && t1door)
                        //    ObjectManager.instance.SpinOpenObj(obj);
                        //else if (obj.transform.eulerAngles.y == 90)
                        //    ObjectManager.instance.SpinCloseObj(obj);
                        break;
                    case "Drawer_":
                        obj = hitInfo.transform.parent.gameObject;
                        index = ObjectManager.instance.getObjectIndex(obj);
                        NetworkManager.instance.SendObject(index);

                        //if (obj.transform.localPosition.z == 0)
                        //    ObjectManager.instance.DrawOpenObj(obj);
                        //else if (obj.transform.localPosition.z == 0.2f)
                        //    ObjectManager.instance.DrawCloseObj(obj);
                        break;
                    default:
                        break;
                }
            }
        }

        //if (Input.GetMouseButtonDown(0))
        //{
        //    RaycastHit hitInfo = new RaycastHit();
        //    float distance = 1f;
        //    Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);
        //    if (Physics.Raycast(ray, out hitInfo, distance))
        //    {
        //        switch (hitInfo.transform.gameObject.name)
        //        {
        //            case "table_":
        //                obj = hitInfo.transform.gameObject;
        //                if (obj.transform.eulerAngles.y == 0) { 
        //                    ObjectManager.instance.SpinOpenObj(obj);
        //                    t1door = true;
        //                }
        //                else if (obj.transform.eulerAngles.y == 90) { 
        //                    ObjectManager.instance.SpinCloseObj(obj);
        //                    t1door = false;
        //                }
        //                break;
        //            case "st1_memo1_":
        //                obj = hitInfo.transform.gameObject;
        //                ObjectManager.instance.OpenDiscription(obj);
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //}
    }
    #endregion
    // Use this for initialization
    void Start()
    {
        oldPositon = transform.position;
        oldRotation = transform.rotation;
        currentPositon = oldPositon;
        currentRotation = oldRotation;

        this.GetComponent<MeshRenderer>().materials[0].color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        if (isMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Update is called once per frame
    void Update () {
        if (!isMine)
            return;
        if (Input.GetKey(KeyCode.Escape))
        {
            UIManager.instance.OpenStopPanel();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isMine = false;
        }
        InputFunc();
    }
}
