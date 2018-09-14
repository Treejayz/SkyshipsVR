using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRIO_Button : VRInteractableObject
{

    [HideInInspector]
    public bool pressed = false;


    public override void Grab(GameObject controller)
    {
        base.Grab(controller);
        pressed = true;
    }

    public override void Release(GameObject controller)
    {
        pressed = false;
        base.Release(controller);
    }
}
