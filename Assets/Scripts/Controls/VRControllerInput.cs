using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRControllerInput : MonoBehaviour {

    public GameObject Tutorial;

    [HideInInspector]
    public GameObject currentHeld;

    Vector3 regularTriggerSize;
    Vector3 holdingTriggerSize;

    protected SteamVR_TrackedObject trackedObj;
    public SteamVR_Controller.Device device
    {
        get
        {
            return SteamVR_Controller.Input((int)trackedObj.index);
        }
    }

    void Awake()
    {
        //Instantiate lists
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        regularTriggerSize = GetComponent<BoxCollider>().size;
        holdingTriggerSize = (GetComponent<BoxCollider>().size) * 2.5f;
    }

    void OnTriggerStay(Collider collider)
    {
        //If object is an interactable item
        VRInteractableObject interactable = collider.GetComponent<VRInteractableObject>();
        if (interactable != null)
        {
            //If trigger button is down
            if (device.GetHairTriggerDown() && currentHeld == null)
            {
                //Pick up object
                interactable.Grab(this.gameObject);
                currentHeld = collider.gameObject;
                GetComponent<BoxCollider>().size = holdingTriggerSize;
            }
            if (device.GetHairTriggerUp())
            {
                //Pick up object
                
                interactable.Release(this.gameObject);
                if (currentHeld != interactable && currentHeld != null)
                {
                    GetComponent<BoxCollider>().size = regularTriggerSize;
                    currentHeld.GetComponent<VRInteractableObject>().Release(this.gameObject);
                    currentHeld = null;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentHeld == other.gameObject)
        {
            currentHeld.GetComponent<VRInteractableObject>().Release(this.gameObject);
            GetComponent<BoxCollider>().size = regularTriggerSize;
        }
    }

    private void Update()
    {
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            Tutorial.SetActive(!Tutorial.activeSelf);
        }

        if (device.GetHairTriggerUp() && currentHeld != null)
        {
            currentHeld.GetComponent<VRInteractableObject>().Release(this.gameObject);
            GetComponent<BoxCollider>().size = regularTriggerSize;
            currentHeld = null;
        }
    }
}
