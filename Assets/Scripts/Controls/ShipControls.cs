using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControls : MonoBehaviour {

    public VRIO_Lever forward;
    public VRIO_Lever up;
    public VRIO_Wheel turn;
    public VRIO_Button buttonDown;
    public VRIO_Button buttonUp;
    public VRIO_Crank rotate;
    public VRIO_Joystick strafe;

    public float windForce;
    public Vector3 windDirection;
    public GameObject mast;
    public GameObject sail;


    Vector3 strafeVelocity;
    float strafeAcceleration = 2f;



    // Velocity variable
    Vector3 velocity = Vector3.zero;
    float maxVelocity = 4f;

    // Forward/Backward variables
    float forwardAcceleration = 2f;
    float dampenSpeed = .5f;

    // Vertical variables
    float verticalVelocity = 0;
    float maxVerticalVelocity = .3f;
    float maxVerticalAcceleration = 1f;

    // turning variables
    float turnSpeed = 0f;
    float maxTurnSpeed = 3f;
    float maxTurnAccel = 2f;

    //button stuff
    float rotationalVelocity;


    //Sound stuff
    float pitch = 0f;


    Vector3 min;

    Vector3 max;

    private void Start()
    {
        min = MainManager.minBounds;
        max = MainManager.maxBounds;
    }


    private void Update()
    {
        // New Movement Test

        // First calculate acceleration in the direction we are facing

        Vector3 acceleration = transform.right * ((forward.value + 1f)/2f) * forwardAcceleration;

        if (pitch < ((forward.value + 1f) / 2f))
        {
            pitch += Time.deltaTime * .3f;
        } else if (pitch > ((forward.value + 1f) / 2f))
        {
            pitch -= Time.deltaTime * .3f;
        }
        GetComponent<AudioSource>().pitch = pitch;


        velocity += acceleration * Time.deltaTime;

        // Now wind stuff

        float angle = Vector3.Angle(windDirection.normalized, transform.right);
        velocity += transform.right * windForce * Time.deltaTime * (1f - (angle/180f)) * 0.5f;
        velocity += windDirection.normalized * windForce * Time.deltaTime * 0.5f;
        /*
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
        */


        // Last we dampen the speed based on how fast we are going
        Vector3 drag = velocity.normalized * velocity.magnitude * -1f * dampenSpeed;
        velocity += drag * Time.deltaTime;

        //if (velocity.magnitude > maxVelocity)


        float forwardSpeed;
        if (angle > 90) { forwardSpeed = -1f * velocity.magnitude * (1f - ((180f - angle) / 90f)); }
        else { forwardSpeed = velocity.magnitude * (1f - (angle / 90f)); }

        // Now the same but for vertical
        if (forwardSpeed != 0f)
        {
            if (up.value > 0)
            {
                float targetVelocity = up.value * maxVerticalVelocity * forwardSpeed;
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
                float targetVelocity = up.value * maxVerticalVelocity * forwardSpeed;
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
        }

        // Turning stuff
        angle = Vector3.Angle(velocity.normalized, transform.right);

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
                turnSpeed -= ((turnSpeed - targetTurn) / 0.5f) * maxTurnAccel * Time.deltaTime * (forwardSpeed / 3f);
            }
            else
            {
                turnSpeed -= maxTurnAccel * Time.deltaTime * (forwardSpeed / 3f);
            }
        }
        turnSpeed = Mathf.Clamp(turnSpeed, maxTurnSpeed * -1, maxTurnSpeed);


        // SAIL
        mast.transform.rotation = Quaternion.LookRotation(windDirection);
        sail.transform.localScale = new Vector3(1f, 1f, 1f + 3f * (windForce / 0.6f));
    }

    // Update is called once per frame
    void FixedUpdate () {


        Vector3 vel = velocity;
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


        if (vel != velocity)
        {
            velocity = vel;
        }



        transform.position += velocity * Time.deltaTime;
        transform.position += new Vector3(0, verticalVelocity) * Time.deltaTime;

        //transform.Translate(velocity * Time.fixedDeltaTime);
        //transform.Translate(forwardVelocity * Time.fixedDeltaTime, verticalVelocity * Time.fixedDeltaTime, 0);

        transform.Rotate(0f,  turnSpeed * Time.fixedDeltaTime * -5f, 0f);

        Quaternion rot = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        transform.rotation = rot;


        // Standing still movement
        if (forward.value == -1f)
        {
            // Strafing and vertical
            float yValue = 0f;
            if (buttonDown.pressed)
            {
                yValue -= 1f;
            } if (buttonUp.pressed)
            {
                yValue += 1f;
            }

            Vector3 targetvelocity = new Vector3(strafe.zValue, yValue, strafe.xValue);
            strafeVelocity += (targetvelocity - strafeVelocity) * strafeAcceleration * Time.fixedDeltaTime;
            transform.Translate( 0.6f * strafeVelocity * Time.fixedDeltaTime);

            // Rotation in place
            rotationalVelocity = Mathf.Lerp(rotationalVelocity, rotate.value * 20f, .01f);
            
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
        if (collision.gameObject.tag == "Untagged")
        {
            Vector3 normal = collision.contacts[0].normal;
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                normal += collision.contacts[i].normal;
            }
            normal.Normalize();


            Collide(normal);
        }
    }

    public void Collide(Vector3 hitNormal)
    {
        Vector3 totalVelocity = velocity + new Vector3(0, verticalVelocity);

        Vector3 impulse = hitNormal.normalized * totalVelocity.magnitude * 0.7f;

        verticalVelocity = impulse.y;
        velocity = new Vector3(impulse.x, 0, impulse.z);

    }

}
