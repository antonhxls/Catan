using Assets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    bool pressed = false;
    public float swing_angle;
    public float elevate_angle;

    // Use this for initialization
    void Start()
    {
        swing_angle = -50.0f;
        elevate_angle = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.Instance.IsSettingsPanelOpen || GiveAwayResources.GiveAwayResourcePanel.activeSelf || (Game.Instance.StartGamePanel != null && Game.Instance.StartGamePanel.activeSelf) || (Game.Instance.StartGamePanel != null && Game.Instance.QuickGuidePanel.activeSelf))
        {
            return;
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Input.GetAxis("Vertical") * transform.forward * 0.02f;
            limitCameraPosition();
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += Input.GetAxis("Vertical") * transform.forward * 0.02f;
            limitCameraPosition();
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += Input.GetAxis("Horizontal") * transform.right * 0.02f;
            limitCameraPosition();
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Input.GetAxis("Horizontal") * transform.right * 0.02f;
            limitCameraPosition();
        }
        if (Input.GetMouseButtonDown(1))
        {
            pressed = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            pressed = false;
        }
        if (pressed)
        {
            swing_angle += (3 * Input.GetAxis("Mouse Y"));
            elevate_angle += (3 * Input.GetAxis("Mouse X"));
            transform.localRotation = Quaternion.identity;
            Quaternion newRot = Quaternion.Euler(-swing_angle, elevate_angle, 0);
            transform.localRotation = newRot;
        }
    }

    void limitCameraPosition() 
    {
        //Limit height
        if (transform.position.y < 1)
        {
            transform.position = new Vector3(transform.position.x, 1.0f, transform.position.z);
        }
        if (transform.position.y > 10)
        {
            transform.position = new Vector3(transform.position.x, 10.0f, transform.position.z);
        }

        //Limit width
        if (transform.position.x < -10)
        {
            transform.position = new Vector3(-10.0f, transform.position.y, transform.position.z);
        }
        if (transform.position.x > 10)
        {
            transform.position = new Vector3(10.0f, transform.position.y, transform.position.z);
        }

        //Limit depth
        if (transform.position.z < -10)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -10.0f);
        }
        if (transform.position.z > 10)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 10.0f);
        }
    }

}
