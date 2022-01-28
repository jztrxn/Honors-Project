using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour
{
    public GameObject cube;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        //HeartSubscribe.
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void moveCube()
    {
        cube.transform.Translate(0, 0, 0.01f);
    }
    
}
