using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRControllerInput : MonoBehaviour {

    [HideInInspector]
    public GameObject currentHeld;

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
            }
            if (device.GetHairTriggerUp())
            {
                //Pick up object
                
                interactable.Release(this.gameObject);
                if (currentHeld != interactable && currentHeld != null)
                {
                    currentHeld.GetComponent<VRInteractableObject>().Release(this.gameObject);
                    currentHeld = null;
                }
            }
        }
    }


    private void Update()
    {
        if (device.GetHairTriggerUp() && currentHeld != null)
        {
            currentHeld.GetComponent<VRInteractableObject>().Release(this.gameObject);
            currentHeld = null;
        }
    }
}
