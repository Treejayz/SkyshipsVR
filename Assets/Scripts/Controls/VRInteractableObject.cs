using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRInteractableObject : MonoBehaviour {

    [HideInInspector]
    public bool grabbed = false;
    [HideInInspector]
    public GameObject currentController;

    public virtual void Grab(GameObject controller)
    {
        if (grabbed)
        {
            Release(currentController);
        }
        currentController = controller;
        grabbed = true;
    }
    public virtual void Release(GameObject controller)
    {
        if (controller == currentController)
        {
            currentController = null;
            grabbed = false;
        }
    }

}
