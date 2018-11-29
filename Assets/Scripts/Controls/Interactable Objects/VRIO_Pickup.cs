using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRIO_Pickup : VRInteractableObject
{

    Vector3 prevPos = new Vector3(0, 0, 0);
    Vector3 velocity;

    public override void Grab(GameObject controller)
    {
        base.Grab(controller);
        prevPos = controller.transform.position;
        FixedJoint fx = controller.AddComponent<FixedJoint>();
        fx.breakForce = 500;
        fx.breakTorque = 500;
        fx.connectedBody = GetComponent<Rigidbody>();
    }

    public override void Release(GameObject controller)
    {
        if (controller == currentController && controller.GetComponent<FixedJoint>())
        {
            controller.GetComponent<FixedJoint>().connectedBody = null;
            Destroy(controller.GetComponent<FixedJoint>());
            GetComponent<Rigidbody>().velocity = velocity;
            GetComponent<Rigidbody>().angularVelocity = SteamVR_Controller.Input((int)controller.GetComponent<SteamVR_TrackedObject>().index).angularVelocity;
        }
        base.Release(controller);
    }

    private void FixedUpdate()
    {
        if (grabbed)
        {
            velocity = (currentController.transform.position - prevPos) * (1f/Time.fixedDeltaTime);
            prevPos = currentController.transform.position;
        }
    }
}
