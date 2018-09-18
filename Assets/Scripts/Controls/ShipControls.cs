using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControls : MonoBehaviour {

    public VRIO_Lever forward;
    public VRIO_Lever up;
    public VRIO_Wheel turn;
    public VRIO_Button rotateLeft;
    public VRIO_Button rotateRight;
    public VRIO_Joystick strafe;


    Vector3 strafeVelocity;
    float strafeAcceleration = 2f;

    
    
    

    // Forward/Backward variables
    float forwardVelocity = 0;
    float maxForwardVelocity = 4f;
    float maxBackwardVelocity = 1f;
    float maxAcceleration = 1f;
    float dampenSpeed = .7f;

    // Vertical variables
    float verticalVelocity = 0;
    float maxVerticalVelocity = 2f;
    float maxVerticalAcceleration = 1f;

    // turning variables
    float turnSpeed = 0f;
    float maxTurnSpeed = 3f;
    float maxTurnAccel = 2f;

    //button stuff
    float rotationalVelocity;

    private void Update()
    {
        // First calculate the forward/backward velocity
        if (forward.value > 0)
        {
            float targetVelocity = forward.value * maxForwardVelocity;
            if (forwardVelocity >= .9f * targetVelocity)
            {
                forwardVelocity += ((targetVelocity - forwardVelocity) / (targetVelocity * .1f)) * maxAcceleration * Time.deltaTime;
            } else
            {
                forwardVelocity += maxAcceleration * Time.deltaTime;
            }
        }
        else if (forward.value < 0)
        {
            float targetVelocity = forward.value * maxBackwardVelocity;
            if (forwardVelocity <= .9f * targetVelocity)
            {
                forwardVelocity -= ((targetVelocity - forwardVelocity) / (targetVelocity * .1f)) * maxAcceleration * Time.deltaTime;
            }
            else
            {
                forwardVelocity -= maxAcceleration * Time.deltaTime;
            }
        }
        else
        {
            if (forwardVelocity > 0.05f)
            {
                if (forwardVelocity < 0.5f)
                {
                    forwardVelocity -= (forwardVelocity / .5f) * dampenSpeed * Time.deltaTime;
                }
                else
                {
                    forwardVelocity -= dampenSpeed * Time.deltaTime;
                }
            }
            else if (forwardVelocity < -0.05f)
            {
                if (forwardVelocity > -0.5f)
                {
                    forwardVelocity += (forwardVelocity / -.5f) * dampenSpeed * Time.deltaTime;
                }
                else
                {
                    forwardVelocity += dampenSpeed * Time.deltaTime;
                }
            }
            else
            {
                forwardVelocity = 0;
            }
        }

        // Now the same but for vertical
        if (up.value > 0)
        {
            float targetVelocity = up.value * maxVerticalVelocity;
            if (verticalVelocity >= .9f * targetVelocity)
            {
                verticalVelocity += ((targetVelocity - verticalVelocity) / (targetVelocity * .1f)) * maxVerticalAcceleration * Time.deltaTime;
            }
            else
            {
                verticalVelocity += maxVerticalAcceleration * Time.deltaTime;
            }
        }
        else if (up.value < 0)
        {
            float targetVelocity = up.value * maxVerticalVelocity;
            if (verticalVelocity <= .9f * targetVelocity)
            {
                verticalVelocity -= ((targetVelocity - verticalVelocity) / (targetVelocity * .1f)) * maxVerticalAcceleration * Time.deltaTime;
            }
            else
            {
                verticalVelocity -= maxVerticalAcceleration * Time.deltaTime;
            }
        }
        else
        {
            if (verticalVelocity > 0.05f)
            {
                if (verticalVelocity < 0.5f)
                {
                    verticalVelocity -= (verticalVelocity / .5f) * dampenSpeed * Time.deltaTime;
                }
                else
                {
                    verticalVelocity -= dampenSpeed * Time.deltaTime;
                }
            }
            else if (verticalVelocity < -0.05f)
            {
                if (verticalVelocity > -0.5f)
                {
                    verticalVelocity += (verticalVelocity / -.5f) * dampenSpeed * Time.deltaTime;
                }
                else
                {
                    verticalVelocity += dampenSpeed * Time.deltaTime;
                }
            }
            else
            {
                verticalVelocity = 0;
            }
        }

        // Turning stuff
        float targetTurn = turn.value * maxTurnSpeed;
        if (turnSpeed < targetTurn)
        {
            if (targetTurn - turnSpeed < .5f)
            {
                turnSpeed += ((targetTurn - turnSpeed) / 0.5f) * maxTurnAccel * Time.deltaTime;
            }
            else
            {
                turnSpeed += maxTurnAccel * Time.deltaTime;
            }

        } else
        {
            if (turnSpeed - targetTurn < .5f)
            {
                turnSpeed -= ((turnSpeed - targetTurn) / 0.5f) * maxTurnAccel * Time.deltaTime;
            }
            else
            {
                turnSpeed -= maxTurnAccel * Time.deltaTime;
            }
        }

    }

    // Update is called once per frame
    void FixedUpdate () {

        transform.Translate(forwardVelocity * Time.fixedDeltaTime, verticalVelocity * Time.fixedDeltaTime, 0);
        transform.Rotate(0f, forwardVelocity * turnSpeed * Time.fixedDeltaTime * -1f, 0f);

        // Standing still movement
        if (forwardVelocity < .2f)
        {
            // Strafing
            Vector3 targetvelocity = new Vector3(strafe.zValue, 0f, strafe.xValue);
            strafeVelocity += (targetvelocity - strafeVelocity) * strafeAcceleration * Time.fixedDeltaTime;
            transform.Translate( 0.6f * strafeVelocity * Time.fixedDeltaTime);

            // Rotation in place
            if (rotateLeft.pressed && !rotateRight.pressed)
            {
                rotationalVelocity = Mathf.Lerp(rotationalVelocity, -20f, .01f);
            }
            else if (rotateRight.pressed && !rotateLeft.pressed)
            {
                rotationalVelocity = Mathf.Lerp(rotationalVelocity, 20f, .01f);
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
