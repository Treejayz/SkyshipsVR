using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControls : MonoBehaviour {

    public VRIO_Lever forward;
    public VRIO_Lever up;
    public VRIO_Wheel turn;
    public VRIO_Button rotateLeft;
    public VRIO_Button rotateRight;

    Vector3 velocity;
    float rotationalVelocity = 0f;

    private void Update()
    {
        velocity = Vector3.Lerp(velocity, new Vector3(forward.value, up.value * .7f, 0), .005f);
    }

    // Update is called once per frame
    void FixedUpdate () {
        transform.Translate(velocity * Time.fixedDeltaTime);
        transform.Rotate(0f, velocity.x * turn.value * Time.fixedDeltaTime * -15f, 0f);

        // Standing still movement
        if (velocity.magnitude < .2f)
        {
            if (rotateLeft.pressed && !rotateRight.pressed)
            {
                rotationalVelocity = Mathf.Lerp(rotationalVelocity, -30f, .01f);
            }
            else if (rotateRight.pressed && !rotateLeft.pressed)
            {
                rotationalVelocity = Mathf.Lerp(rotationalVelocity, 30f, .01f);
            }else
            {
                rotationalVelocity = Mathf.Lerp(rotationalVelocity, 0f, .01f);
            }
            transform.Rotate(0f, Time.fixedDeltaTime * rotationalVelocity, 0f);
        }
        else
        {
            if (Mathf.Abs(rotationalVelocity) > 0.1f)
            {
                rotationalVelocity = Mathf.Lerp(rotationalVelocity, 0f, .01f);
                transform.Rotate(0f, Time.fixedDeltaTime * rotationalVelocity, 0f);
            }
            else
            {
                rotationalVelocity = 0f;
            }
        }
    }
}
