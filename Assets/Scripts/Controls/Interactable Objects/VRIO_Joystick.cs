using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRIO_Joystick : VRInteractableObject
{

    public GameObject center;
    public float maxAngle;

    [HideInInspector]
    public float xValue = 0; // ranges from -1 to 1
    [HideInInspector]
    public float zValue = 0; // ranges from -1 to 1

    Vector3 rotation;

    public override void Grab(GameObject controller)
    {
        base.Grab(controller);
        //offset = transform.InverseTransformPoint(controller.transform.position).normalized;
        StopCoroutine("Snap");
    }

    public override void Release(GameObject controller)
    {

        if (controller == currentController)
        {
            StopCoroutine("Snap");
            StartCoroutine("Snap");
        }
        base.Release(controller);
    }

    private void Update()
    {
        // Calculate the values
        Vector3 zRot = rotation;
        zRot.z = 0f;
        zRot.Normalize();
        zValue = Vector3.Angle(Vector3.up, zRot) / maxAngle;
        if (zRot.x < 0) { zValue *= -1f; }
        Vector3 xRot = rotation;
        xRot.x = 0f;
        xRot.Normalize();
        xValue = Vector3.Angle(Vector3.up, xRot) / maxAngle;
        if (xRot.z < 0) { xValue *= -1f; }

        // If we are grabbing something, set the z 
        if (grabbed)
        {
            Vector3 vec = center.transform.parent.InverseTransformPoint(currentController.transform.position).normalized;
            if (Vector3.Angle(Vector3.up, vec) > maxAngle)
            {
                vec = Vector3.RotateTowards(Vector3.up, vec, Mathf.Deg2Rad * maxAngle, 0f);
            }
            rotation = vec;
            center.transform.localRotation = Quaternion.FromToRotation(Vector3.up, rotation);
            //center.transform.LookAt(currentController.transform.position, center.transform.parent.transform.up);
        }
    }

    IEnumerator Snap()
    {
        while (Vector3.Angle(Vector3.up, rotation) > .5f)
        {
            rotation = Vector3.Lerp(rotation, Vector3.up, .1f);
            center.transform.localRotation = Quaternion.FromToRotation(Vector3.up, rotation);
            yield return new WaitForEndOfFrame();
        }
    }

}
