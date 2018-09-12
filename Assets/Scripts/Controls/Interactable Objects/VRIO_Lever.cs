using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRIO_Lever : VRInteractableObject
{
    public Transform console;
    public float minZ;
    public float maxZ;
    public float[] anchorPoints;

    float offsetZ;

    bool grabbed = false;
    GameObject currentController;

    public override void Grab(GameObject controller)
    {
        base.Grab(controller);
        if (grabbed)
        {
            Release(currentController);
        }
        //offsetZ = console.transform.InverseTransformPoint(currentController.transform.position).z - transform.localPosition.z;

        currentController = controller;
        grabbed = true;
    }

    public override void Release(GameObject controller)
    {
        base.Release(controller);

        if (controller == currentController)
        {
            currentController = null;
            grabbed = false;
            if (anchorPoints.Length != 0)
            {
                StopCoroutine("Snap");
                StartCoroutine("Snap");
            }
        }
    }

    private void Update()
    {
        if (grabbed)
        {
            float zPos = console.transform.InverseTransformPoint(currentController.transform.position).z;
            //Get the lever's current local position
            Vector3 position = transform.localPosition;
            //Set lever's z position to the Z of the converted controller position
            //Clamp it so the lever doesn't go too far either way
            position.z = Mathf.Clamp(zPos, minZ, maxZ);
            //Set lever to new position
            transform.localPosition = position;
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

        Vector3 position;

        while (dist > .01f)
        {
            position = transform.localPosition;
            position.z = Mathf.Lerp(position.z, anchorpoint, .2f);
            transform.localPosition = position;
            yield return new WaitForEndOfFrame();
        }
        position = transform.localPosition;
        position.z = anchorpoint;
        transform.localPosition = position;

    }

}
