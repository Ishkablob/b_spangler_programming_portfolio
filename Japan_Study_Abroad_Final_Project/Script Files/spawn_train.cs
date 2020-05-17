using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawn_train : MonoBehaviour {

    public Transform trainPrefab;
    public Transform trainTarget;
    public GameObject trainStation;

    float timer = 20;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;
        if (timer >= 20)
        {
            timer = 0;
            PlayerPrefs.SetInt("MaihamaLineValue", (int)(20 * PlayerPrefs.GetFloat("RushHourScaler")));
            Transform temp = Instantiate(trainPrefab, this.transform.position, trainPrefab.rotation);
            temp.GetComponent<move_two>().station = trainStation;
            temp.GetComponent<move_two>().targetA = this.gameObject.transform;
            temp.GetComponent<move_two>().targetB = trainTarget;
        }
	}
}
