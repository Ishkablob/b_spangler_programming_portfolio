using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_multiple_final : MonoBehaviour {

    public Transform[] targets;
    public GameObject[] stations;

    Transform targetA;
    Transform targetB;
    public float speed = 0;
    float speed_max = 0.2f;
    float distanceAB;
    public int targetIndex = 0;

    bool atStation = false;
    float timer = 0;
    public GameObject stationAt;

	// Use this for initialization
	void Start ()
    {
        targetA = targets[targetIndex];
        targetIndex++;
        targetB = targets[targetIndex];
        targetIndex++;
        transform.position = targetA.position;
        distanceAB = Vector3.Distance(targetA.position, targetB.position);
    }

    void Update()
    {
        if (!atStation)
            NormalTranslate();
        else if (atStation)
            StationStop();
    }
	
	// Update is called once per frame
	void NormalTranslate()
    {
        float distancePB = Vector3.Distance(transform.position, targetB.position);
        if (distancePB < 0.1/* && speed == 0*/)
        {
            targetA = targetB;
            targetB = targets[(int)((targetIndex++) % targets.Length)];
            distanceAB = Vector3.Distance(targetA.position, targetB.position);
            return;
        }
        //if (distancePB < 0.1) speed = 0;
        //else if (distancePB < .5f) speed -= 0.002f;
        if (speed >= speed_max) speed = speed_max;
        else speed += 0.001f;

        if (speed < 0) speed = 0;
        float t = ((distanceAB - distancePB + speed) / distanceAB);
        transform.position = ((1 - t) * targetA.position) + (t * targetB.position);

        transform.rotation = Quaternion.LookRotation(Vector3.Normalize(targetB.position - transform.position));
        transform.Rotate(new Vector3(0, 90, 0));
    }

    void StationStop()
    {
        float distancePB = Vector3.Distance(transform.position, targetB.position);
        speed -= 0.002f;
        if (speed <= 0)
        {
            if (timer == 0)
                stationAt.GetComponent<station_manager>().isActive = true;
            speed = 0;
            timer += Time.deltaTime;
        }

        float t = ((distanceAB - distancePB + speed) / distanceAB);
        transform.position = ((1 - t) * targetA.position) + (t * targetB.position);
        

        if (timer > 4)
        {
            timer = 0;
            atStation = false;
            stationAt.GetComponent<station_manager>().isActive = false;
            stationAt = null;
            print("Leaving station.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        foreach (GameObject station in stations)
        {
            if (other == station.GetComponent<Collider>())
            {
                atStation = true;
                stationAt = station;
                station.GetComponent<station_manager>().spawner.GetComponent<station_spawner>().spawned = false;
                print("Arrived at " + station.name + ", stopping");
                break;
            }
        }
    }
}
