using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRIO_ChestLid : VRInteractableObject
{

    Transform origin;

    Vector3 prevHandPosition;

    float openAmount = 0f;

    private void Start()
    {
        origin = transform.parent;
    }

    public override void Grab(GameObject controller)
    {
        base.Grab(controller);
        prevHandPosition = origin.transform.InverseTransformPoint(currentController.transform.position);
        prevHandPosition.x = 0;
        StopCoroutine("Return");
    }

    public override void Release(GameObject controller)
    {
        base.Release(controller);
        StopCoroutine("Return");
        StartCoroutine("Return");
    }

    private void Update()
    {
        if (grabbed)
        {
            Vector3 currentHandPosition = origin.transform.InverseTransformPoint(currentController.transform.position);
            currentHandPosition.x = 0;
            float angularDelta = (Mathf.Atan2(currentHandPosition.z, currentHandPosition.y) - Mathf.Atan2(prevHandPosition.z, prevHandPosition.y)) * Mathf.Rad2Deg;
            prevHandPosition = currentHandPosition;
            openAmount += angularDelta;
            openAmount = Mathf.Clamp(openAmount, -80f, 0f);

            transform.localRotation = Quaternion.Euler(openAmount, 0f, 0f);
        }
    }


    IEnumerator Return()
    {
        float angularSpeed = 0f;
        while (true)
        {
            if (openAmount > 0f && angularSpeed >= 0f)
            {
                if (angularSpeed < 40f)
                {
                    break;
                }
                else
                {
                    print(openAmount + ", " + angularSpeed);
                    openAmount = 0f;
                    float volume = angularSpeed / 540f;
                    Mathf.Clamp01(volume);
                    GetComponent<AudioSource>().volume = volume;
                    GetComponent<AudioSource>().Play();
                    angularSpeed *= -0.6f;
                }
            }
            else
            {

                if (angularSpeed < 0)
                {
                    angularSpeed += 360f * Time.deltaTime;
                }
                else
                {
                    angularSpeed += 360f * Time.deltaTime;
                }
            }
            transform.localRotation = Quaternion.Euler(openAmount, 0f, 0f);
            openAmount += angularSpeed * Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        openAmount = 0f;
        transform.localRotation = Quaternion.Euler(openAmount, 0f, 0f);
    }

}
