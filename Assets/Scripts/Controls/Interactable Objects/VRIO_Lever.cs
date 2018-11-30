using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRIO_Lever : VRInteractableObject
{
    public Transform console;

    public Transform grabber;
    public float grabberAngle;

    public float minZ;
    public float maxZ;
    public float[] anchorPoints;

    [HideInInspector]
    public float value = 0; // ranges from -1 to 1

    int anchorIndex = 0;

    float offsetZ = 0;

    public override void Grab(GameObject controller)
    {
        base.Grab(controller);
        offsetZ = console.transform.InverseTransformPoint(controller.transform.position).z - transform.localPosition.z;
        StopAllCoroutines();
        StartCoroutine("Press");

    }

    public override void Release(GameObject controller)
    {

        if (controller == currentController)
        {
            if (anchorPoints.Length != 0)
            {
                StopAllCoroutines();
                StartCoroutine("Snap");
                StartCoroutine("UnPress");
            }
        }
        base.Release(controller);
    }

    private void Start()
    {
        StartCoroutine("Snap");
    }

    private void Update()
    {
        // If we are grabbing something, set the z 
        if (grabbed)
        {
            float zPos = console.transform.InverseTransformPoint(currentController.transform.position).z - offsetZ;
            Vector3 position = transform.localPosition;
            position.z = Mathf.Clamp(zPos, minZ, maxZ);
            transform.localPosition = position;
            if (anchorPoints.Length == 0)
            {
                value = ((maxZ - position.z) / (maxZ - minZ)) * -2 + 1;
            } else
            {
                float anchorpoint = anchorPoints[0];
                float dist = 99999f;
                int temp = anchorIndex;
                int i = 0;
                foreach (float anchor in anchorPoints)
                {
                    if (Mathf.Abs(transform.localPosition.z - anchor) < dist)
                    {
                        anchorIndex = i;
                        anchorpoint = anchor;
                        dist = Mathf.Abs(transform.localPosition.z - anchor);
                    }
                    i += 1;
                }
                if (anchorIndex != temp)
                {
                    SteamVR_Controller.Input((int)currentController.GetComponent<SteamVR_TrackedObject>().index).TriggerHapticPulse(4000);
                    GetComponent<AudioSource>().Play();

                }
                value = ((maxZ - anchorpoint) / (maxZ - minZ)) * -2 + 1;
            }
        }
    }

    IEnumerator Snap()
    {
        float anchorpoint = anchorPoints[0];
        float dist = 99999f;
        foreach (float anchor in anchorPoints)
        {
            if (Mathf.Abs(transform.localPosition.z - anchor) < dist)
            {
                anchorpoint = anchor;
                dist = Mathf.Abs(transform.localPosition.z - anchor);
            }
        }

        value = ((maxZ - anchorpoint) / (maxZ - minZ)) * -2 + 1;
        Vector3 position;

        while (dist > .03f)
        {
            position = transform.localPosition;
            position.z = Mathf.Lerp(position.z, anchorpoint, .2f);
            transform.localPosition = position;
            yield return new WaitForEndOfFrame();
        }
        GetComponent<AudioSource>().Play();
        position = transform.localPosition;
        position.z = anchorpoint;
        transform.localPosition = position;

    }


    IEnumerator Press()
    {
        while (grabber.localRotation.eulerAngles.x < grabberAngle - (grabberAngle * 0.01f))
        {
            grabber.localRotation = Quaternion.Euler(Mathf.Lerp(grabber.localRotation.eulerAngles.x, grabberAngle, .2f), 0f, 0f);
            yield return new WaitForEndOfFrame();
        }
        grabber.localRotation = Quaternion.Euler(grabberAngle, 0f, 0f);

    }

    IEnumerator UnPress()
    {
        while (grabber.localRotation.eulerAngles.x > (grabberAngle * 0.01f))
        {
            grabber.localRotation = Quaternion.Euler(Mathf.Lerp(grabber.localRotation.eulerAngles.x, 0f, .2f), 0f, 0f);
            yield return new WaitForEndOfFrame();
        }
        grabber.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

}
