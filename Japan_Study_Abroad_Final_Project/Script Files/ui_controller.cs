using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ui_controller : MonoBehaviour {

    public Text resultsText;
    string fileName;

    void Start()
    {
        resultsText.text = "Simulation Runtime: " + (int) PlayerPrefs.GetFloat("SimTime") + " seconds"
            + "\nTotal Agents Spawned in Sim: " + PlayerPrefs.GetInt("TotalAgents")
            + "\nDisneyland Visitors: " + PlayerPrefs.GetInt("TotalAgentsDisneyland")
            + "\nDisneySea Visitors: " + PlayerPrefs.GetInt("TotalAgentsDisneySea")
            + "\nVisitors that Entered via Maihama JR Line: " + PlayerPrefs.GetInt("TotalAgentsMaihama")
            + "\nVisitors that Entered via Bay Area Resorts: " + PlayerPrefs.GetInt("TotalAgentsBayAreaResorts")
            + "\nNumber of Disney Train Line Users:" + PlayerPrefs.GetInt("TotalAgentsDisneyLine");
    }

    public void StartSimButton()
    {
        Application.LoadLevel("Main Scene");
    }

    public void ResultPageButton()
    {
        Application.LoadLevel("Sim Results");
    }

    public void ReturnToTitlePageButton()
    {
        Application.LoadLevel("Title Page");
    }

    public void InstructionsButton()
    {
        Application.LoadLevel("Instructions");
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void SaveFileButton()
    {
        fileName = Application.dataPath + "/output.txt";
        if (File.Exists(fileName))
        {
            Debug.Log(fileName + " already exists");
            return;
        }

        StreamWriter sw = File.CreateText(fileName);
        sw.WriteLine("Tokyo Disney Resort Train Simulation Results:");
        sw.WriteLine("Total Simulation Time: " + PlayerPrefs.GetFloat("SimTime"));
        sw.WriteLine("Total Agents Spawned in Sim: " + PlayerPrefs.GetInt("TotalAgents"));
        sw.WriteLine("Disneyland Visitors: " + PlayerPrefs.GetInt("TotalAgentsDisneyland"));
        sw.WriteLine("DisneySea Visitors: " + PlayerPrefs.GetInt("TotalAgentsDisneySea"));
        sw.WriteLine("Visitors that Entered via Maihama JR Line: " + PlayerPrefs.GetInt("TotalAgentsMaihama"));
        sw.WriteLine("Visitors that Entered via Bay Area Resorts: " + PlayerPrefs.GetInt("TotalAgentsBayAreaResorts"));
        sw.WriteLine("Number of Disney Train Line Users: " + PlayerPrefs.GetInt("TotalAgentsDisneyLine"));
        sw.Close();

        print("File saved");
    }
}
