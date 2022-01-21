using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonControllers : MonoBehaviour
{
    private InputAction buttonAction;

    // Start is called before the first frame update
    void Start()
    {
        //selectAction.action.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        
        var gamepad = Gamepad.current;
    }
    
}
