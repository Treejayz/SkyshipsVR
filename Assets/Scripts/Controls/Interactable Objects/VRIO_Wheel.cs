using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRIO_Wheel : VRInteractableObject
{

    public float numTurns;
    public GameObject origin;
    public GameObject WheelMesh;

    [HideInInspector]
    public float value = 0; // ranges from -1 to 1
    float turnAmount;
    float angularVelocity;

    Vector3 prevHandPosition;

    int knotches = 0;

    public override void Grab(GameObject controller)
    {
        base.Grab(controller);
        StopCoroutine("Snap");
        prevHandPosition = origin.transform.InverseTransformPoint(currentController.transform.position);
        prevHandPosition.x = 0;
    }

    public override void Release(GameObject controller)
    {
        if (controller == currentController)
        {
            StopCoroutine("Snap");
            StartCoroutine("Snap");
        }
        base.Release(controller);
    }

    private void Update()
    {

        value = (Mathf.Clamp(turnAmount, -360f * numTurns, 360f * numTurns)) / (360f * numTurns);
        // If we are grabbing something, update the angle
        if (grabbed)
        {
            Vector3 currentHandPosition = origin.transform.InverseTransformPoint(currentController.transform.position);
            currentHandPosition.x = 0;
            float angularDelta = (Mathf.Atan2(currentHandPosition.z, currentHandPosition.y) - Mathf.Atan2(prevHandPosition.z, prevHandPosition.y)) * Mathf.Rad2Deg;
            prevHandPosition = currentHandPosition;
            turnAmount += angularDelta;
            turnAmount = Mathf.Clamp(turnAmount, numTurns * -360f, numTurns * 360f);
            angularVelocity = angularDelta * (1f / Time.deltaTime);
            if (angularDelta > 180f)
            {
                turnAmount -= 360f;
                angularVelocity = (angularDelta - 360) * (1f / Time.deltaTime);
            } else if (angularDelta < -180f)
            {
                turnAmount += 360f;
                angularVelocity = (angularDelta + 360) * (1f / Time.deltaTime);
            }
            if (turnAmount < 360f * numTurns && turnAmount > -360f * numTurns)
            {
                WheelMesh.transform.localRotation = Quaternion.Euler(turnAmount, 0f, 0f);
            }

            if ((int)turnAmount / 45 != knotches)
            {
                knotches = (int)turnAmount / 45;
                GetComponent<AudioSource>().Play();
                SteamVR_Controller.Input((int)currentController.GetComponent<SteamVR_TrackedObject>().index).TriggerHapticPulse(3000);
            }

        }
    }

    IEnumerator Snap()
    {
        bool returning = true;

        while (returning)
        {
            if (turnAmount > 0f)
            {
                if (turnAmount > 180f)
                {
                    angularVelocity = Mathf.Lerp(angularVelocity, -360f, .01f);
                    if (Mathf.Abs(angularVelocity + 360f) < 20)
                    {
                        returning = false;
                    }
                }
                else
                {
                    if (angularVelocity > 0f)
                    {
                        angularVelocity = Mathf.Lerp(angularVelocity, -360f, .01f);
                    } else if (angularVelocity < -360f)
                    {
                        angularVelocity = Mathf.Lerp(angularVelocity, 0, .01f);
                    }
                    else
                    {
                        angularVelocity = Mathf.Lerp(angularVelocity, -360f * (turnAmount / 180f), .01f);
                    }
                    if (Mathf.Abs(angularVelocity + (360f * (turnAmount/180f))) < 10)
                    {
                        returning = false;
                    }
                }
            }
            else if (turnAmount <= 0f)
            {
                if (turnAmount < -180f)
                {
                    angularVelocity = Mathf.Lerp(angularVelocity, 360f, .01f);
                    if (Mathf.Abs(angularVelocity - 360f) < 20)
                    {
                        returning = false;
                    }
                }
                else
                {
                    if (angularVelocity < 0f)
                    {
                        angularVelocity = Mathf.Lerp(angularVelocity, 360f, .01f);
                    }
                    else if (angularVelocity > 360f)
                    {
                        angularVelocity = Mathf.Lerp(angularVelocity, 0, .01f);
                    }
                    else
                    {
                        angularVelocity = Mathf.Lerp(angularVelocity, 360f * (turnAmount / -180f), .01f);
                    }
                    if (Mathf.Abs(angularVelocity - (360f * (turnAmount / -180f))) < 10)
                    {
                        returning = false;
                    }
                }
            }
            turnAmount += angularVelocity * Time.deltaTime;
            if (turnAmount > 360f * numTurns)
            {
                turnAmount = 360f * numTurns;
                angularVelocity = 0f;
            } else if (turnAmount < -360f * numTurns)
            {
                turnAmount = -360f * numTurns;
                angularVelocity = 0f;
            }

            if ((int)turnAmount / 45 != knotches)
            {
                knotches = (int)turnAmount / 45;
                GetComponent<AudioSource>().Play();
            }

            WheelMesh.transform.localRotation = Quaternion.Euler(turnAmount, 0f, 0f);
            yield return new WaitForEndOfFrame();
        }

        // Now we slowly return to what we should be
        turnAmount = Mathf.Clamp(turnAmount, numTurns * -360f, numTurns * 360f);
        if (turnAmount > 1f)
        {
            while (turnAmount > 180f)
            {
                turnAmount -= 360f * Time.deltaTime;

                if ((int)turnAmount / 45 != knotches)
                {
                    knotches = (int)turnAmount / 45;
                    GetComponent<AudioSource>().Play();
                }
                WheelMesh.transform.localRotation = Quaternion.Euler(turnAmount, 0f, 0f);
                yield return new WaitForEndOfFrame();
            }
            while (turnAmount > 1f)
            {
                float currentTurn = 360f * (turnAmount / 180f) * Time.deltaTime;
                turnAmount -= currentTurn;
                if ((int)turnAmount / 45 != knotches)
                {
                    knotches = (int)turnAmount / 45;
                    GetComponent<AudioSource>().Play();
                }
                WheelMesh.transform.localRotation = Quaternion.Euler(turnAmount, 0f, 0f);
                yield return new WaitForEndOfFrame();
            }
        } else if (turnAmount < -1f)
        {
            while (turnAmount < -180f)
            {
                turnAmount += 360f * Time.deltaTime;
                if ((int)turnAmount / 45 != knotches)
                {
                    knotches = (int)turnAmount / 45;
                    GetComponent<AudioSource>().Play();
                }
                WheelMesh.transform.localRotation = Quaternion.Euler(turnAmount, 0f, 0f);
                yield return new WaitForEndOfFrame();
            }
            while (turnAmount < -1f)
            {
                float currentTurn = 360f * (turnAmount / -180f) * Time.deltaTime;
                turnAmount += currentTurn;
                if ((int)turnAmount / 45 != knotches)
                {
                    knotches = (int)turnAmount / 45;
                    GetComponent<AudioSource>().Play();
                }
                WheelMesh.transform.localRotation = Quaternion.Euler(turnAmount, 0f, 0f);
                yield return new WaitForEndOfFrame();
            }
        }

        turnAmount = 0f;
        WheelMesh.transform.Rotate(0f, 0f, 0f);
    }

}
