using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRIO_Pickup : VRInteractableObject
{

    bool grabbed;
    GameObject currentController;

    public override void Grab(GameObject controller)
    {
        base.Grab(controller);
        if (grabbed)
        {
            Release(currentController);
        }

        FixedJoint fx = controller.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        fx.connectedBody = GetComponent<Rigidbody>();
        currentController = controller;
        grabbed = true;
    }

    public override void Release(GameObject controller)
    {
        base.Release(controller);

        if (controller == currentController && controller.GetComponent<FixedJoint>())
        {
            controller.GetComponent<FixedJoint>().connectedBody = null;
            Destroy(controller.GetComponent<FixedJoint>());
            GetComponent<Rigidbody>().velocity = SteamVR_Controller.Input((int)controller.GetComponent<SteamVR_TrackedObject>().index).velocity;
            GetComponent<Rigidbody>().angularVelocity = SteamVR_Controller.Input((int)controller.GetComponent<SteamVR_TrackedObject>().index).angularVelocity;
            currentController = null;
            grabbed = false;
        }
    }

}
