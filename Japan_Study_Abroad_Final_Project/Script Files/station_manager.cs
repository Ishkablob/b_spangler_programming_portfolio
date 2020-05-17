using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class station_manager : MonoBehaviour {

    public GameObject spawner;
    public bool isActive = false;

	// Use this for initialization
	void Start ()
    {
        GetComponent<Renderer>().enabled = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
	}

    void OnTriggerStay(Collider other)
    {
    }
}
