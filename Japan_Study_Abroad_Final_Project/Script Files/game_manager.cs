using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class game_manager : MonoBehaviour {

    public Slider slider;
    public Slider slider2;
    public Text text;

    // Use this for initialization
    void Start ()
    {
        PlayerPrefs.SetInt("DisneylandValue", 0);
        PlayerPrefs.SetInt("DisneySeaValue", 0);
        PlayerPrefs.SetInt("MaihamaLineValue", 0);
        PlayerPrefs.SetInt("DisneyLineValue", 0);
        PlayerPrefs.SetInt("TotalAgents", 0);
        PlayerPrefs.SetInt("TotalAgentsDisneyland", 0);
        PlayerPrefs.SetInt("TotalAgentsDisneySea", 0);
        PlayerPrefs.SetInt("TotalAgentsMaihama", 0);
        PlayerPrefs.SetInt("TotalAgentsDisneyLine", 0);
        PlayerPrefs.SetInt("TotalAgentsBayAreaResorts", 0);

        PlayerPrefs.SetFloat("RushHourScaler", 1);
        PlayerPrefs.SetFloat("AgentSpeed", 1);
        PlayerPrefs.SetFloat("SimTime", 0);
    }
	
	// Update is called once per frame
	void Update ()
    {
        PlayerPrefs.SetFloat("RushHourScaler", slider.value);
        PlayerPrefs.SetFloat("AgentSpeed", slider2.value);
        PlayerPrefs.SetFloat("SimTime", Time.timeSinceLevelLoad);
        text.text = "Agents on DisneyLine: " + PlayerPrefs.GetInt("DisneyLineValue")
            + "\nAgents in Disneyland: " + PlayerPrefs.GetInt("DisneylandValue")
            + "\nAgents in DisneySea: " + PlayerPrefs.GetInt("DisneySeaValue")
            + "\nAgents on Maihama Line: " + PlayerPrefs.GetInt("MaihamaLineValue");
    }
}
