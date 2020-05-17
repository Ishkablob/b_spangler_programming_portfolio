using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class station_spawner : MonoBehaviour {

    public GameObject[] goals;
    // goals[0] = Maihama Station, goals[1] = DisneyLineStation1, goals[2] = DisneylandStation
    // goals[3] = Bay Area Station, goals[4] = DisneySeaStation, goals[5] = Disneyland
    // goals[6] = DisneySea, goals[7] = Bay Area Resorts
    public GameObject prefab;
    public GameObject station;
    float timer;
    public bool spawned = false;

	// Use this for initialization
	void Start ()
    {
        //goals[0] = GameObject.Find("MaihamaStation");
        //goals[1] = GameObject.Find("DisneyLineStation");
        //goals[2] = GameObject.Find("DisneylandStation");
        //goals[3] = GameObject.Find("BayAreaStation");
        //goals[4] = GameObject.Find("DisneySeaStation");
        //goals[5] = GameObject.Find("DisneyGoal");
        //goals[6] = GameObject.Find("DisneySeaGoal");
        //goals[7] = GameObject.Find("BayAreaGoal");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (station.GetComponent<station_manager>().isActive && !spawned)
        {
            if (station.name == "MaihamaStation")
            {
                int temp2 = Random.Range(0, PlayerPrefs.GetInt("MaihamaLineValue"));
                for (int i = 0; i < temp2; i++)
                {
                    GameObject temp = Instantiate<GameObject>(prefab, transform.position, Quaternion.identity);
                    temp.GetComponent<NavMeshAgent>().SetDestination(goals[1].transform.position);
                    PlayerPrefs.SetInt("MaihamaLineValue", PlayerPrefs.GetInt("MaihamaLineValue") - 1);
                    PlayerPrefs.SetInt("TotalAgents", PlayerPrefs.GetInt("TotalAgents") + 1);
                    PlayerPrefs.SetInt("TotalAgentsMaihama", PlayerPrefs.GetInt("TotalAgentsMaihama") + 1);
                }
            }

            else 
            {
                int temp2 = Random.Range(0, PlayerPrefs.GetInt("DisneyLineValue"));
                for (int i = 0; i < temp2; i++)
                {
                    GameObject temp = Instantiate<GameObject>(prefab, transform.position, Quaternion.identity);
                    if (station.name == "DisneyLineStation")
                        temp.GetComponent<NavMeshAgent>().SetDestination(goals[0].transform.position);
                    else if (station.name == "DisneylandStation")
                        temp.GetComponent<NavMeshAgent>().SetDestination(goals[5].transform.position);
                    else if (station.name == "BayAreaStation")
                        temp.GetComponent<NavMeshAgent>().SetDestination(goals[7].transform.position);
                    else if (station.name == "DisneySeaStation")
                        temp.GetComponent<NavMeshAgent>().SetDestination(goals[6].transform.position);
                    PlayerPrefs.SetInt("DisneyLineValue", PlayerPrefs.GetInt("DisneyLineValue") - 1);
                }
            }
            spawned = true;
        }
	}
}
