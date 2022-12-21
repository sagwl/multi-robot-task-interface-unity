using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleHighlight : MonoBehaviour
{

    Image highlight;

    // Start is called before the first frame update
    void Start()
    {
        highlight = GetComponent<Image>();
    }

    public void ToggleImage()
    {
        highlight.enabled = !highlight.enabled;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
