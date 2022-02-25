using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusionLockScaling : MonoBehaviour
{

    public GameObject fusionLock;
    public float scaleFactor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private float zPos = 0f;
    //public GameObject cube;
    // Update is called once per frame
    void Update()
    {
        zPos = this.gameObject.transform.position.z;
        this.gameObject.transform.localScale = new Vector3(0.1f * zPos, 0.1f * zPos, 1f);
        this.gameObject.transform.localPosition = new Vector3(0f, 0f, zPos);
        //cube.transform.position = new Vector3(0, 0, zPos);

    }
}
