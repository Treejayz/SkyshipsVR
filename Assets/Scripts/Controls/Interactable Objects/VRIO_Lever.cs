using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRIO_Lever : VRInteractableObject
{
    public Transform console;
    public float minZ;
    public float maxZ;
    public float[] anchorPoints;

    [HideInInspector]
    public float value = 0; // ranges from -1 to 1

    float offsetZ = 0;

    bool grabbed = false;
    GameObject currentController;

    public override void Grab(GameObject controller)
    {
        base.Grab(controller);
        if (grabbed)
        {
            Release(currentController);
        }
        offsetZ = console.transform.InverseTransformPoint(controller.transform.position).z - transform.localPosition.z;
        StopCoroutine("Snap");
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
                print (value);
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
        print(value);
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
