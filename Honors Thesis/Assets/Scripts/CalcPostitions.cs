using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalcPostitions : MonoBehaviour
{

    public GameObject leftObj;
    public GameObject rightObj;
    public GameObject centerObj;

    public Text valuesObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetUp(OVRInput.Button.Three))
        {
            float distFromUser = leftObj.transform.position.z;
            float leftPos = leftObj.transform.position.x;
            float rightPos = rightObj.transform.position.x;
            float separation = leftPos - rightPos;
            float distFromCenter = Mathf.Abs((leftPos + rightPos)/2);

            valuesObj.text = "Distance: " + distFromUser.ToString() + 
                "\nLeft: " + leftPos.ToString() + "\nRight: " + rightPos.ToString() + 
                "\nSep: " + separation.ToString() + "\ndistCenter: " + distFromCenter.ToString();
        }
    }
}
