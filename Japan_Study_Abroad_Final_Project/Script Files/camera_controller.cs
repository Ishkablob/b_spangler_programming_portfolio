using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_controller : MonoBehaviour {

    Vector3 previousMousePosition = Input.mousePosition;
    Quaternion defaultRotation;

	// Use this for initialization
	void Start ()
    {
        defaultRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) transform.position += new Vector3(0.2f, 0, 0);
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) transform.position -= new Vector3(0.2f, 0, 0);
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) transform.position -= new Vector3(0, 0, 0.2f);
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) transform.position += new Vector3(0, 0, 0.2f);
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) transform.position += new Vector3(0, 0.2f, 0);
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) transform.position -= new Vector3(0, 0.2f, 0);
        if (Input.GetMouseButton(0))
        {
            if (Input.mousePosition.x > previousMousePosition.x) transform.Rotate(new Vector3(0, 0.5f, 0));
            if (Input.mousePosition.x < previousMousePosition.x) transform.Rotate(new Vector3(0, -0.5f, 0));
            if (Input.mousePosition.y > previousMousePosition.y) transform.Rotate(new Vector3(-0.5f, 0, 0));
            if (Input.mousePosition.y < previousMousePosition.y) transform.Rotate(new Vector3(0.5f, 0, 0));
        }
        if (Input.GetMouseButton(1))
        {
            if (Input.mousePosition.x > previousMousePosition.x) transform.Rotate(new Vector3(0, 0, 0.5f));
            if (Input.mousePosition.x < previousMousePosition.x) transform.Rotate(new Vector3(0, 0, -0.5f));
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.position = new Vector3(70, 55.75f, 127.3f);
            transform.rotation = defaultRotation;
        }
        previousMousePosition = Input.mousePosition;
    }
}
