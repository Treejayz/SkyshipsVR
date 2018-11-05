using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRIO_Button : VRInteractableObject
{

    public float pressDistance;

    [HideInInspector]
    public bool pressed = false;

    float startY;
    float endY;

    private void Start()
    {
        startY = transform.localPosition.y;
        endY = startY - pressDistance;
    }

    public override void Grab(GameObject controller)
    {
        base.Grab(controller);
        pressed = true;
        SteamVR_Controller.Input((int)controller.GetComponent<SteamVR_TrackedObject>().index).TriggerHapticPulse(50000);
        StopAllCoroutines();
        StartCoroutine("Press");

    }

    public override void Release(GameObject controller)
    {
        pressed = false;
        base.Release(controller);
        StopAllCoroutines();
        StartCoroutine("UnPress");
    }


    IEnumerator Press()
    {
        while (transform.localPosition.y > endY + (pressDistance * 0.05f))
        {
            Vector3 newpos = transform.localPosition;
            newpos.y = Mathf.Lerp(transform.localPosition.y, endY, .1f);
            transform.localPosition = newpos;
            yield return new WaitForEndOfFrame();
        }
        transform.localPosition = new Vector3(transform.localPosition.x, endY, transform.localPosition.z);

    }

    IEnumerator UnPress()
    {
        while (transform.localPosition.y < startY - (pressDistance * 0.05f))
        {
            Vector3 newpos = transform.localPosition;
            newpos.y = Mathf.Lerp(transform.localPosition.y, startY, .1f);
            transform.localPosition = newpos;
            yield return new WaitForEndOfFrame();
        }
        transform.localPosition = new Vector3(transform.localPosition.x, startY, transform.localPosition.z);
    }
}
