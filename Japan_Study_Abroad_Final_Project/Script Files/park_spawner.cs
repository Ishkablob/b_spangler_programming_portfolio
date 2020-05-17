using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class park_spawner : MonoBehaviour {

    public GameObject prefab;
    public GameObject goal;
    float timer = 0;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;
        if (timer >= 30 && this.gameObject.name != "BayAreaSpawner")
        {
            timer = 0;
            if (this.gameObject.name == "DisneylandSpawner")
            {
                int temp2 = Random.Range(0, (int) (PlayerPrefs.GetInt("DisneylandValue")));
                for (int i = 0; i < temp2; i++)
                {
                    GameObject temp = Instantiate<GameObject>(prefab, transform.position, Quaternion.identity);
                    temp.GetComponent<NavMeshAgent>().SetDestination(goal.transform.position);
                    PlayerPrefs.SetInt("DisneylandValue", PlayerPrefs.GetInt("DisneylandValue") - 1);
                }
            }

            else if (this.gameObject.name == "DisneySeaSpawner")
            {
                int temp2 = Random.Range(0, (int) (PlayerPrefs.GetInt("DisneySeaValue")));
                for (int i = 0; i < temp2; i++)
                {
                    GameObject temp = Instantiate<GameObject>(prefab, transform.position, Quaternion.identity);
                    temp.GetComponent<NavMeshAgent>().SetDestination(goal.transform.position);
                    PlayerPrefs.SetInt("DisneySeaValue", PlayerPrefs.GetInt("DisneySeaValue") - 1);
                }
            }
        }


        if (timer >= 10 && this.gameObject.name == "BayAreaSpawner")
        {
            timer = 0;
            for (int i = 0; i < Random.Range(0, Mathf.Max((int)(15 * (PlayerPrefs.GetFloat("RushHourScaler")/2)),15)); i++)
            {
                GameObject temp = Instantiate<GameObject>(prefab, transform.position, Quaternion.identity);
                temp.GetComponent<NavMeshAgent>().SetDestination(goal.transform.position);
                PlayerPrefs.SetInt("TotalAgents", PlayerPrefs.GetInt("TotalAgents") + 1);
                PlayerPrefs.SetInt("TotalAgentsBayAreaResorts", PlayerPrefs.GetInt("TotalAgentsBayAreaResorts") + 1);
            }
        }
    }
}
