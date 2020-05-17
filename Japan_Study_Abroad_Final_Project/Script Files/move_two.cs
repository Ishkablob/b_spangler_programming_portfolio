using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_two : MonoBehaviour {
    
    public GameObject station;

    public Transform targetA;
    public Transform targetB;
    public float speed = 10;
    float speed_max = 10f;
    float distanceAB;

    bool atStation = false;
    float timer = 0;
    float destroyTimer = 0;

    // Use this for initialization
    void Start()
    {
        transform.position = targetA.position;
        distanceAB = Vector3.Distance(targetA.position, targetB.position);
    }

    void Update()
    {
        destroyTimer += Time.deltaTime;
        if (destroyTimer >= 20)
            Destroy(this.gameObject);
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
            Destroy(this.gameObject);
            return;
        }
        //if (distancePB < 0.1) speed = 0;
        //else if (distancePB < .5f) speed -= 0.002f;
        if (speed >= speed_max) speed = speed_max;
        else speed += 0.002f;

        if (speed < 0) speed = 0;
        float t = ((distanceAB - distancePB + speed) / distanceAB) * Time.timeScale;
        transform.position = ((1 - t) * targetA.position) + (t * targetB.position);

        transform.rotation = Quaternion.LookRotation(Vector3.Normalize(targetB.position - transform.position));
        transform.Rotate(new Vector3(0, 90, 0));
    }

    void StationStop()
    {
        float distancePB = Vector3.Distance(transform.position, targetB.position);
        speed -= 0.004f;
        if (speed <= 0)
        {
            if (timer == 0)
                station.GetComponent<station_manager>().isActive = true;
            speed = 0;
            timer += Time.deltaTime;
        }

        float t = ((distanceAB - distancePB + speed) / distanceAB);
        transform.position = ((1 - t) * targetA.position) + (t * targetB.position);


        if (timer > 2)
        {
            timer = 0;
            atStation = false;
            station.GetComponent<station_manager>().isActive = false;
            print("Leaving station.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other == station.GetComponent<Collider>())
        {
            atStation = true;
            station.GetComponent<station_manager>().spawner.GetComponent<station_spawner>().spawned = false;
        }
    }
}
