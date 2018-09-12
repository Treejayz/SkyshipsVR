using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControls : MonoBehaviour {

    public VRIO_Lever forward;
    public VRIO_Lever turn;
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.Translate(forward.value * Time.fixedDeltaTime, 0f, 0f);
        transform.Rotate(0f, forward.value * turn.value * Time.fixedDeltaTime * 90f, 0f);
    }
}
