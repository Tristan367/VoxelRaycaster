using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementControl : MonoBehaviour
{
    [System.NonSerialized] public float movementSpeed = 10;
    float currentMovementSpeed;
    float sprintSpeed;
    float boostSpeed;
    float boostSwitchTimer = 0;
    float boostSwitchTime = .5f;
    bool boostTimingOn = false;
    bool boosting = false;

    Vector3 moveDir;
    public Transform cam;
    Vector3 forward;
    CustomRigidbody rb;

    float flySwitchTime = .5f;
    float flySwitchTimer = 0;
    bool timingOn = false;

    bool flying = true;

    bool pressingKey = false;

    float drag = .25f;


    private void Start()
    {
        moveDir = new Vector3(0, 0, 0);
        rb = GetComponent<CustomRigidbody>();
        rb.mass = 2;
        rb.drag = drag;
        forward = transform.forward;
        if (flying)
        {
            rb.useGravity = false;
        }

        currentMovementSpeed = movementSpeed;
        sprintSpeed = movementSpeed * 7;
        boostSpeed = movementSpeed * 25;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !timingOn)
        {
            timingOn = true;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && timingOn)
        {
            timingOn = false;
            flySwitchTimer = 0;
            flying = !flying;
            rb.useGravity = !rb.useGravity;
            if (!flying)
            {
                forward = Vector3.forward;
            }
        }
        if (timingOn)
        {
            flySwitchTimer += Time.deltaTime;
            if (flySwitchTimer >= flySwitchTime)
            {
                flySwitchTimer = 0;
                timingOn = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !boostTimingOn)
        {
            boostTimingOn = true;
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && boostTimingOn)
        {
            boostTimingOn = false;
            boosting = true;
            boostSwitchTimer = 0;
        }
        if (boostTimingOn)
        {
            boostSwitchTimer += Time.deltaTime;
            if (boostSwitchTimer >= boostSwitchTime)
            {
                boostSwitchTimer = 0;
                boostTimingOn = false;
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            boosting = false;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            pressingKey = true;
            if (boosting)
            {
                currentMovementSpeed = boostSpeed;
            }
            else
            {
                currentMovementSpeed = sprintSpeed;
            }
        }
        else
        {
            currentMovementSpeed = movementSpeed;
        }
    }

    void FixedUpdate()
    {
        if (flying)
        {
            forward = transform.InverseTransformDirection(cam.transform.forward);
            Vector3 relativeUp = cam.transform.up;
            if (Input.GetKey(KeyCode.Space))
            {
                pressingKey = true;
                rb.AddForce(relativeUp * currentMovementSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                pressingKey = true;
                rb.AddForce(-relativeUp * currentMovementSpeed * Time.deltaTime);
            }
        }
        else
        {
            // shoot ray at ground and add force while it hits something
        }

        if (Input.GetKey(KeyCode.W))
        {
            pressingKey = true;
            moveDir += forward * currentMovementSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            pressingKey = true;
            moveDir += -forward * currentMovementSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            pressingKey = true;
            moveDir += Vector3.right * currentMovementSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            pressingKey = true;
            moveDir += Vector3.left * currentMovementSpeed;
        }
        if (pressingKey)
        {
            rb.AddForce(transform.TransformDirection(moveDir) * Time.deltaTime);
            moveDir *= 0;
        }

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            rb.drag = 10;
        }
        else
        {
            rb.drag = drag;

        }


        pressingKey = false;
    }
}