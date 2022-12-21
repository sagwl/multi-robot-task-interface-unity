using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using System.IO;

public class TaskMenuHandler : MonoBehaviour
{
    public SteamVR_Action_Boolean optionSelected;
    public SteamVR_Action_Boolean dpadUp;
    public SteamVR_Action_Boolean dpadDown;
    Canvas canvas;
    Button[] buttons;
    public int buttonIdx = -1;
    public int highlightedButton = -1;
    List<int> taskList;

    string filepath = "Assets/Resources/tasks.txt"; //Path to file for storing task ids for a single request

    // Start is called before the first frame update
    void Start()
    {
        dpadUp.AddOnChangeListener(dpadUpAction, SteamVR_Input_Sources.RightHand);
        dpadDown.AddOnChangeListener(dpadDownAction, SteamVR_Input_Sources.RightHand);
        optionSelected.AddOnChangeListener(OptionSelected, SteamVR_Input_Sources.LeftHand);
        canvas = GetComponent<Canvas>();
        buttons = GetComponentsInChildren<Button>();
        taskList = new List<int>();
        foreach (Button button in buttons)
        {
            button.GetComponent<Image>().color = Color.black;
        }
    }

    private void OptionSelected(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        if(canvas.enabled == true)
        {
            StreamWriter writer = new StreamWriter(filepath, true);
            if (newState == true)
            {
                if (taskList.Contains(highlightedButton))
                {
                    Debug.Log("Task already in list");
                }
                else
                {
                    taskList.Add(highlightedButton);
                    buttons[highlightedButton].GetComponentInChildren<ToggleHighlight>().ToggleImage();
                    writer.WriteLine(highlightedButton);
                }
            }
            writer.Close();
        }
    }


    private void SelectButton(int selectedButton)
    {
        if (buttonIdx >= 0)
            buttons[buttonIdx].GetComponent<Image>().color = Color.black;
        buttonIdx = selectedButton;
        buttons[buttonIdx].GetComponent<Image>().color = Color.green;
    }


    private void dpadUpAction(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        if(canvas.enabled == true)
        {
            if (newState == true)
            {
                if (highlightedButton < 4)
                {
                    highlightedButton += 1;
                    SelectButton(highlightedButton);
                }
                Debug.Log(highlightedButton);
            }
        }
    }

    private void dpadDownAction(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        if (canvas.enabled == true)
        {
            if (newState == true)
            {
                if (highlightedButton > 0)
                {
                    highlightedButton -= 1;
                    SelectButton(highlightedButton);
                }
                Debug.Log(highlightedButton);
            }
        }
    }
}
