using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour
{
    // horizontal rotation speed
    float horizontalSpeed = 3f;
    // vertical rotation speed
    float verticalSpeed = 3f;
    float xRotation = 0.0f;
    float yRotation = 0.0f;
    public Transform cam;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * horizontalSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * verticalSpeed;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        cam.transform.localEulerAngles = new Vector3(xRotation, 0, 0.0f);
        transform.eulerAngles = new Vector3(0, yRotation, 0.0f);
    }
}
