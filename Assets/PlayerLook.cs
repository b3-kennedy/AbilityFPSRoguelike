using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerLook : NetworkBehaviour
{
    public float sensitivity;
    public GameObject player;
    Transform playerObj;
    public Transform orientation;
    public Transform cam;

    GameObject playerVisor;

    float xRot;
    float yRot;

    // Start is called before the first frame update
    void Start()
    {

        playerVisor = transform.GetChild(0).GetChild(0).gameObject;
        Cursor.lockState = CursorLockMode.Locked;
        if(!IsOwner)
        {
            cam.transform.GetChild(0).GetChild(0).GetComponent<Camera>().enabled = false;
            cam.transform.GetChild(0).GetChild(0).GetComponent<AudioListener>().enabled = false;
            playerVisor.layer = 0;
        }


    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;

        float mouseX = (Input.GetAxis("Mouse X") + Input.GetAxis("RightJoystickHorizontal")) * sensitivity;
        float mouseY = (Input.GetAxis("Mouse Y") + (Input.GetAxis("RightJoystickVertical")* -1)) * sensitivity;

        yRot += mouseX;
        xRot -= mouseY;

        xRot = Mathf.Clamp(xRot, - 90f, 90f);

        if(cam != null)
        {
            cam.transform.rotation = Quaternion.Euler(xRot, yRot, 0);
        }
        
        orientation.rotation = Quaternion.Euler(0, yRot,0);
        player.transform.rotation = orientation.rotation;
    }
}