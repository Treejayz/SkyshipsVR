using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour {

    public ShipControls shipController;

    private void OnCollisionEnter(Collision collision)
    {
        print("Hit");
        shipController.Collide(collision.impulse);
    }
}
