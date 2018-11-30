using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepInBounds : MonoBehaviour {

    Vector3 min;

    Vector3 max;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        min = MainManager.minBounds;
        max = MainManager.maxBounds;
    }

    // Update is called once per frame
    void Update () {

        Vector3 vel = rb.velocity;
		if (transform.position.x > max.x)
        {
            vel.x *= -1;
        }
        if (transform.position.x < min.x)
        {
            vel.x *= -1;
        }
        if (transform.position.y > max.y)
        {
            vel.y *= -1;
        }
        if (transform.position.y < min.y)
        {
            vel.y *= -1;
        }
        if (transform.position.z > max.z)
        {
            vel.z *= -1;
        }
        if (transform.position.x < min.z)
        {
            vel.z *= -1;
        }


        if (vel != rb.velocity)
        {
            rb.velocity = vel;
        }
	}
}
