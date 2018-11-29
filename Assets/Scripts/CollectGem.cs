using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectGem : MonoBehaviour {

    public GameObject poof;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Collectable")
        {
            Instantiate(poof, other.transform.position, Quaternion.identity);
            Destroy(other.gameObject);
        } 
    }
}
