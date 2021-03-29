using System;
using UnityEngine;

public class CustomRigidbody : MonoBehaviour
{

    [NonSerialized] public Vector3 velocity = new Vector3(0, 0, 0);
    [NonSerialized] public Vector3 angularVelocity = new Vector3(0, 0, 0);
    [NonSerialized] public Vector3 originOffset = new Vector3(0, 0, 0);
    [NonSerialized] public float mass = 1;
    [NonSerialized] public float drag = .05f;
    [NonSerialized] public float angularDrag = 0.01f;
    [NonSerialized] public Vector3 gravity = new Vector3(0, -.05f, 0);
    [NonSerialized] public bool useGravity = true;
    [NonSerialized] public bool dynamic = true;

    [NonSerialized] public float angularMagnitude;

    private void FixedUpdate()
    {
        if (dynamic)
        {
            if (useGravity)
            {
                AddGravityForce(gravity);
            }

            velocity -= drag * velocity;

            angularMagnitude = Mathf.Abs(angularVelocity.x) + Mathf.Abs(angularVelocity.y) + Mathf.Abs(angularVelocity.z);
            angularVelocity -= angularDrag * angularVelocity;


            transform.Translate(transform.InverseTransformVector(velocity)); // moving the rigidbody
            transform.RotateAround(transform.TransformPoint(originOffset), angularVelocity, angularMagnitude); // rotating the rigidbody around an origin
        }

    }

    public void AddGravityForce(Vector3 force)
    {
        velocity += force;
    }

    public void AddForce(Vector3 force)
    {
        velocity += force / mass;
    }

    public void AddTorque(Vector3 force)
    {
        angularVelocity += force / mass;
    }

    public void AddForceAtPosition(Vector3 force, Vector3 point)
    {
        Vector3 r = point - transform.TransformPoint(originOffset); // direction from origin to point
        Vector3 rot = new Vector3((r.y * force.z) - (r.z * force.y), (r.z * force.x) - (r.x * force.z), (r.x * force.y) - (r.y * force.x));
        angularVelocity += rot / mass;

        float rotMag = Mathf.Abs(rot.x) + Mathf.Abs(rot.y) + Mathf.Abs(rot.z);
        Vector3 f = force / (rotMag + 1);
        velocity += f / mass;

    }



    public float GetVelocityMagnitude()
    {
        return Mathf.Abs(velocity.x) + Mathf.Abs(velocity.y) + Mathf.Abs(velocity.z);
    }

}