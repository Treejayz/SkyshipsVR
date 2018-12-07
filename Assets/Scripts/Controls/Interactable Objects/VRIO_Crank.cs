using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRIO_Crank : VRInteractableObject
{

    public GameObject origin;
    public GameObject model;

    [HideInInspector]
    public float value = 0; // ranges from -1 to 1
    float turnAmount;
    float angularVelocity;

    Vector3 prevHandPosition;

    float currentAngle = 90f;


    public override void Grab(GameObject controller)
    {
        base.Grab(controller);
        StopCoroutine("Snap");
        prevHandPosition = origin.transform.InverseTransformPoint(currentController.transform.position);
        prevHandPosition.y = 0;
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
        // If we are grabbing something, update the angle
        if (grabbed)
        {
            Vector3 currentHandPosition = origin.transform.InverseTransformPoint(currentController.transform.position);
            currentHandPosition.y = 0;
            float angularDelta = (Mathf.Atan2(currentHandPosition.x, currentHandPosition.z) - Mathf.Atan2(prevHandPosition.x, prevHandPosition.z)) * Mathf.Rad2Deg;
            prevHandPosition = currentHandPosition;
            model.transform.Rotate(0f, angularDelta, 0f);
            angularVelocity = angularDelta * (1f / Time.deltaTime);
        }
        value = angularVelocity / 720f;
        value = Mathf.Clamp(value, -1f, 1f);

        float angle = (currentAngle - model.transform.eulerAngles.y);
        if (model.transform.eulerAngles.y > 45f && model.transform.eulerAngles.y < 135f &&  currentAngle == 360f)
        {
            currentAngle = 90f;
            if (grabbed)
            {
                SteamVR_Controller.Input((int)currentController.GetComponent<SteamVR_TrackedObject>().index).TriggerHapticPulse(4000);
            }
            GetComponent<AudioSource>().Play();
        }
        else if (angle > 45f && angle <= 315f)
        {
            if (grabbed)
            {
                SteamVR_Controller.Input((int)currentController.GetComponent<SteamVR_TrackedObject>().index).TriggerHapticPulse(4000);
            }
            GetComponent<AudioSource>().Play();
            currentAngle -= 90f;
            if (currentAngle <= 0f)
            {
                currentAngle += 360f;
            }
            print(model.transform.eulerAngles.y + ", " + angle + ", " + currentAngle);
        } else if (angle < -45f)
        {
            if (grabbed)
            {
                SteamVR_Controller.Input((int)currentController.GetComponent<SteamVR_TrackedObject>().index).TriggerHapticPulse(4000);
            }
            GetComponent<AudioSource>().Play();
            currentAngle += 90f;
            if (currentAngle > 360f)
            {
                currentAngle -= 360f;
            }
            print(model.transform.eulerAngles.y + " - " + angle + " - " + currentAngle);
        }

    }

    IEnumerator Snap()
    {
        angularVelocity = Mathf.Clamp(angularVelocity, -720f, 720f);
        if (angularVelocity > 0)
        {
            while (angularVelocity > 0)
            {
                model.transform.Rotate(0f, angularVelocity * Time.deltaTime, 0f);
                angularVelocity -= 360f * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        } else if (angularVelocity < 0)
        {
            while (angularVelocity < 0)
            {
                model.transform.Rotate(0f, angularVelocity * Time.deltaTime, 0f);
                angularVelocity += 360f * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        yield return null;
    }

}
