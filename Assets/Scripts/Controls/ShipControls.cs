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

    public float windForce;
    public Vector3 windDirection;


    Vector3 strafeVelocity;
    float strafeAcceleration = 2f;



    // Velocity variable
    Vector3 velocity = Vector3.zero;
    float maxVelocity = 4f;

    // Forward/Backward variables
    float forwardAcceleration = 1.5f;
    float dampenSpeed = .5f;

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
        // New Movement Test

        // First calculate acceleration in the direction we are facing

        Vector3 acceleration = transform.right * ((forward.value + 1f)/2f) * forwardAcceleration;

        velocity += acceleration * Time.deltaTime;

        // Now wind stuff

        float angle = Vector3.Angle(windDirection.normalized, transform.right);
        velocity += transform.right * windForce * Time.deltaTime * (1f - (angle/180f)) * 0.5f;
        velocity += windDirection.normalized * windForce * Time.deltaTime * 0.5f;
        float direction = Vector3.Cross(windDirection.normalized, transform.right).y;
        float targetTurnSpeed;
        if (angle < 30f)
        {
            targetTurnSpeed = windForce * (angle/30f);
        } else
        {
            targetTurnSpeed = windForce;
        }
        if (direction < 0) { targetTurnSpeed *= -1; }

        targetTurnSpeed *= 2f;

        if (turnSpeed < targetTurnSpeed)
        {
            if (turnSpeed < 0 && targetTurnSpeed > 0)
            {
                turnSpeed += windForce * 2f * Time.deltaTime;
            }
            else if (targetTurnSpeed - turnSpeed < .5f)
            {
                turnSpeed += ((targetTurnSpeed - turnSpeed) / 0.5f) * windForce * Time.deltaTime;
            }
            else
            {
                turnSpeed += windForce * Time.deltaTime;
            }

        }
        else
        {
            if (turnSpeed > 0 && targetTurnSpeed < 0)
            {
                turnSpeed -= windForce * 2f * Time.deltaTime;
            }
            if (turnSpeed - targetTurnSpeed < .5f)
            {
                turnSpeed -= ((turnSpeed - targetTurnSpeed) / 0.5f) * windForce * Time.deltaTime;
            }
            else
            {
                turnSpeed -= windForce * Time.deltaTime;
            }
        }



        // Last we dampen the speed based on how fast we are going
        Vector3 drag = velocity.normalized * velocity.magnitude * -1f * dampenSpeed;
        velocity += drag * Time.deltaTime;

        //if (velocity.magnitude > maxVelocity)

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
        angle = Vector3.Angle(velocity.normalized, transform.right);
        float forwardSpeed;
        if (angle > 90) { forwardSpeed = -1f* velocity.magnitude * (1f - ((180f - angle)/90f)); }
        else { forwardSpeed = velocity.magnitude * (1f - (angle / 90f)); }
        float targetTurn = turn.value * maxTurnSpeed * (forwardSpeed / 3f);

        if (turnSpeed < targetTurn)
        {
            if (targetTurn - turnSpeed < .5f)
            {
                turnSpeed += ((targetTurn - turnSpeed) / 0.5f) * maxTurnAccel * Time.deltaTime * (forwardSpeed / 3f);
            }
            else
            {
                turnSpeed += maxTurnAccel * Time.deltaTime * (forwardSpeed / 3f);
            }

        } else
        {
            if (turnSpeed - targetTurn < .5f)
            {
                turnSpeed += ((turnSpeed - targetTurn) / 0.5f) * maxTurnAccel * Time.deltaTime * (forwardSpeed / 3f);
            }
            else
            {
                turnSpeed += maxTurnAccel * Time.deltaTime * (forwardSpeed / 3f);
            }
        }
        turnSpeed = Mathf.Clamp(turnSpeed, maxTurnSpeed * -1, maxTurnSpeed);
    }

    // Update is called once per frame
    void FixedUpdate () {

        transform.position += velocity * Time.deltaTime;
        transform.position += new Vector3(0, verticalVelocity) * Time.deltaTime;

        //transform.Translate(velocity * Time.fixedDeltaTime);
        //transform.Translate(forwardVelocity * Time.fixedDeltaTime, verticalVelocity * Time.fixedDeltaTime, 0);

        transform.Rotate(0f,  turnSpeed * Time.fixedDeltaTime * -5f, 0f);

        Quaternion rot = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        transform.rotation = rot;


        // Standing still movement
        if (velocity.magnitude < .2f)
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

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            normal += collision.contacts[i].normal;
        }
        normal.Normalize();


        Collide(normal);
    }

    public void Collide(Vector3 hitNormal)
    {
        Vector3 totalVelocity = velocity + new Vector3(0, verticalVelocity);

        Vector3 impulse = hitNormal.normalized * totalVelocity.magnitude * 0.7f;

        verticalVelocity = impulse.y;
        velocity = new Vector3(impulse.x, 0, impulse.z);

    }

}
