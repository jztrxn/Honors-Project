using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KBControlls : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("A is pressed");
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F is Pressed");
        }
    }
}
