using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Valve.VR;
using System.IO;
using Valve.VR.Extras;

public class AllocateTask : MonoBehaviour
{
    private Canvas canvas; //variable to store reference to UI canvas
    int taskCount; //variable to store count of the number of tasks assigned

    private SteamVR_Input_Sources rController = SteamVR_Input_Sources.RightHand; // Setting up the right hand controller as input source
    private SteamVR_Action_Boolean trigger = SteamVR_Input.GetBooleanAction("InteractUI"); //Creating an action to listen to trigger presses
    private GameObject sceneRightController; //Gameobject for controller from the scene
    private GameObject textPrefab; //variable to hold the prefab of the text mesh pro to add new tasks to the panel

    RaycastHit hit; //Raycast hit object to store hit from the raycast

    GameObject[] robotList; // Robot list. Puts all objects in the scene with the tag "Robot" into this list to move them around

    int buttonid = -1; //Selection menu value - from 0 to 4 in order going from left to right

    string task_filepath = "Assets/Resources/tasks.txt"; //Path to file for storing task ids for a single request
    string allocation_filepath = "Assets/Resources/allocation.txt"; //Path to file for storing task ids for a single request


    void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>(); //instantiating the UI canvas from the scene
        sceneRightController = GameObject.Find("Controller (right)"); //Finding the right controller in scene and assigning it to the variable
        trigger.AddOnChangeListener(triggerChanged, rController); //Listening to trigger change on right hand controller
        textPrefab = Resources.Load("Prefabs/UI/Task_Text") as GameObject; //Initializing the text mesh pro prefab

        robotList = GameObject.FindGameObjectsWithTag("Robot"); //Finding all the objects in the scene with the tag "Robot". Add this tag to all the robots in the scene to make your life easier
    }

    private void triggerChanged(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        buttonid = GameObject.Find("RadialMenu").gameObject.GetComponent<RadialMenuHandler>().highlightedButton; //Get the new finalized button from the RadialMenuHandler script to execute actions
        //Following bit instantiates/spawns the marking region if the trigger is pressed
        if (newState == true)
        {
            if (buttonid == 1)  //Checks if the selected button is the second one (reserving this for task allocation)
            {
                //Do task allocation stuff here

                //Cleans the request file for storing new tasks
                StreamWriter writer = new StreamWriter(task_filepath, false);
                writer.Close();

                //Shows the canvas to add new tasks
                showTaskCanvas(newState);
            }
        }

        if (newState == false)
        {
            if (buttonid == 1)
            {
                showTaskCanvas(newState);
                allocateTasks();
            }
        }

    }
    void Update()
    {
        taskCount = canvas.transform.Find("TaskPanel").gameObject.transform.childCount; //Counts the total number of tasks in the task panel
        canvas.transform.Find("TaskCountPanel").gameObject.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().SetText(string.Format("{0} {1}", "Total Tasks: ",taskCount)); //This changes the total of the tasks in Canvas UI
    }

    private void selectObject(GameObject go)
    {
        if(go.GetComponent<Outline>().enabled == false) //Check if the object is selected or not. Only select it if the object is not selected
        {
            go.GetComponent<Outline>().enabled = true;
            GameObject.Find("Manager").GetComponent<RobotSelectionList>().gameObjects.Add(go);
        }
    }

    private void showTaskCanvas(bool state)
    {
        GameObject.Find("TaskListMenu").gameObject.GetComponent<Canvas>().enabled = state;
    }

    private void allocateTasks()
    {
        StreamReader reader = new StreamReader(allocation_filepath);
        while (!reader.EndOfStream)
        {
            string[] line = reader.ReadLine().Replace(" ", string.Empty).Split(',');

            if(canvas.transform.Find("TaskPanel").gameObject.transform.childCount != 0)
            {
                foreach (Transform child in canvas.transform.Find("TaskPanel").gameObject.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }

            foreach(string str in line)
            {
                GameObject go = Instantiate(textPrefab, canvas.transform.Find("TaskPanel").transform);
                go.GetComponent<TextMeshProUGUI>().SetText(str);
                Debug.Log(str);
            }
            // Do Something with the input. 
        }
    }
}
