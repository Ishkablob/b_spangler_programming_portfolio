using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class agent_manager : MonoBehaviour {

    public GameObject[] goals;
    public Vector3 targetGoal;
    bool waiting = false;
    bool waiting2 = false;
    public GameObject stationWaitingAt;
    float timer = 0;
    float speed;

    // Use this for initialization
    void Start ()
    {
        speed = Random.Range(3, 6);
        this.GetComponent<NavMeshAgent>().speed = speed * PlayerPrefs.GetFloat("AgentSpeed");
        targetGoal = GetComponent<NavMeshAgent>().destination;
    }
	
	// Update is called once per frame
	void Update ()
    {
        this.GetComponent<NavMeshAgent>().speed = speed * PlayerPrefs.GetFloat("AgentSpeed");
        if ((waiting || waiting2) && stationWaitingAt.GetComponent<station_manager>().isActive)
        {
            timer += Time.deltaTime;
            if (timer > 0.5f)
            {
                Destroy(this.gameObject);
                if (waiting)
                {
                    PlayerPrefs.SetInt("DisneyLineValue", PlayerPrefs.GetInt("DisneyLineValue") + 1);
                    PlayerPrefs.SetInt("TotalAgentsDisneyLine", PlayerPrefs.GetInt("TotalAgentsDisneyLine") + 1);
                }
            }
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            if (other.gameObject.name == "DisneyGoal")
            {
                PlayerPrefs.SetInt("TotalAgentsDisneyland", PlayerPrefs.GetInt("TotalAgentsDisneyland") + 1);
                PlayerPrefs.SetInt("DisneylandValue", PlayerPrefs.GetInt("DisneylandValue") + 1);
                Destroy(gameObject);
            }
            else if (other.gameObject.name == "DisneySeaGoal")
            {
                PlayerPrefs.SetInt("TotalAgentsDisneySea", PlayerPrefs.GetInt("TotalAgentsDisneySea") + 1);
                PlayerPrefs.SetInt("DisneySeaValue", PlayerPrefs.GetInt("DisneySeaValue") + 1);
                Destroy(gameObject);
            }
            else if (other.gameObject.name == "BayAreaGoal")
            {
                Destroy(gameObject);
            }
            else if (other.gameObject.name == "DisneyLineStationGoal" || other.gameObject.name == "DisneylandStationGoal"
                  || other.gameObject.name == "DisneySeaStationGoal" || other.gameObject.name == "BayAreaStationGoal")
            {
                waiting = true;
                stationWaitingAt = GameObject.Find(other.gameObject.name.Substring(0, other.gameObject.name.Length - 4));
                //GetComponent<NavMeshAgent>().speed = 0;
            }
            else if (other.gameObject.name == "MaihamaStationGoal")
            {
                waiting2 = true;
                stationWaitingAt = GameObject.Find(other.gameObject.name.Substring(0, other.gameObject.name.Length - 4));
                //GetComponent<NavMeshAgent>().speed = 0;
            }
        }
    }
}
