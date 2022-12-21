using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ExploreLocation : MonoBehaviour
{
    private SteamVR_Input_Sources rController = SteamVR_Input_Sources.RightHand; // Setting up the right hand controller as input source
    private SteamVR_Action_Boolean trigger = SteamVR_Input.GetBooleanAction("InteractUI"); //Creating an action to listen to trigger presses

    RaycastHit hit; //Raycast hit object to store hit from the raycast

    private GameObject sceneRightController; //Gameobject for controller from the scene
    private GameObject markingRegionObject; //Gameobject for the prefab to mark a region
    private GameObject instancedRegion; //variable to store the instance of the instantiated prefab. Also used to remove the previous region and hold the new one

    GameObject[] robotList; // Robot list. Puts all objects in the scene with the tag "Robot" into this list to move them around
    List<Vector3> targetPosList = new List<Vector3>(); // List of target locations for all the robots in the above list
    List<bool> towardsGoalList = new List<bool>(); // List of booleans to tell if the robot should move to target or not for each robot in the above list

    public float markSize = 20.0f; //Size of the selection area
    public float speed = 5.0f; // Speed at which the robot moves
    public float fixedTime = 0.5f; // Time after which the robot changes direction
    public float randomPositionSize = 10.0f; //The random position is selected between + and - of this number
    float timer = 0.0f; // Variable to control the timer
    int buttonid = -1; //Selection menu value - from 0 to 4 in order going from left to right

    // Start is called before the first frame update
    void Start()
    {
        instancedRegion = null;
        sceneRightController = GameObject.Find("Controller (right)"); //Finding the right controller in scene and assigning it to the variable
        trigger.AddOnChangeListener(triggerChanged, rController); //Listening to trigger change on right hand controller

        robotList = GameObject.FindGameObjectsWithTag("Robot"); //Finding all the objects in the scene with the tag "Robot". Add this tag to all the robots in the scene to make your life easier

        for (int i = 0; i < robotList.Length; i++) //Initializing the goal position and boolean lists
        {
            towardsGoalList.Add(false);
            targetPosList.Add(new Vector3(0, 0, 0));
        }
    }

    private void triggerChanged(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        buttonid = GameObject.Find("RadialMenu").gameObject.GetComponent<RadialMenuHandler>().highlightedButton; //Get the new finalized button from the RadialMenuHandler script to execute actions
        //Following bit instantiates/spawns the marking region if the trigger is pressed
        if (newState == true)
        {
            if(buttonid == 0)  //Checks if the selected button is the first one (reserving this for location marking)
            {
                instantiateExploreRegion();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < robotList.Length; i++) // If the robots need to move towards goal, set their target as the newly created waypoint
        {
            if (towardsGoalList[i] == true)
            {
                targetPosList[i] = hit.point;
            }
        }

        for (int i = 0; i < robotList.Length; i++)
        {
            if (Vector3.Distance(hit.point, robotList[i].transform.position) > markSize / 2) // Check if distance of robot is far from goal, if yes, then set it to move towards goal
            {
                towardsGoalList[i] = true;
            }
            else
            {
                towardsGoalList[i] = false;
            }
        }

        timer += Time.deltaTime; //Calculating how much time has passed. 

        for (int i = 0; i < robotList.Length; i++)
        {
            if (timer > fixedTime && towardsGoalList[i] == false) //If more than stipulated time is passed, reset timer and select a new random location for the robot to move
            {
                resetTimerAndPosition();
            }
        }

        for (int i = 0; i < robotList.Length; i++)
        {
            objectMove(robotList[i], targetPosList[i]); //Move the robot towards set target
        }
    }


    // Method to instantiate the exploring region
    void instantiateExploreRegion()
    {
        if(instancedRegion != null)
        {
            Destroy(instancedRegion);
        }

        Ray raycast = new Ray(sceneRightController.transform.localPosition, sceneRightController.transform.forward);
        bool bHit = Physics.Raycast(raycast, out hit);

        markingRegionObject = Resources.Load("Prefabs/Objects/RegionMark") as GameObject; //Initializing the marking region prefab
        markingRegionObject.transform.localScale = new Vector3(markSize, markSize, markSize); // Setting the scale of the marking region
        instancedRegion = Instantiate(markingRegionObject, hit.point, Quaternion.identity);
    }

    void objectMove(GameObject go, Vector3 pos) //Moves the robot towards the target position. 
    {
        go.transform.position = Vector3.MoveTowards(go.transform.position, pos, speed * Time.deltaTime);
    }

    void resetTimerAndPosition() //Reset the timer and randomize the target position.
    {
        timer = 0.0f;
        for (int i = 0; i < robotList.Length; i++)
        {
            targetPosList[i] = robotList[i].transform.position + new Vector3(Random.Range(-randomPositionSize, randomPositionSize), 0, Random.Range(-randomPositionSize, randomPositionSize));
        }
    }

}
