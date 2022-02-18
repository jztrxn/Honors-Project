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

    // Update is called once per frame
    void Update()
    {
        float zLoc = fusionLock.transform.position.z;
        scaleFactor *= zLoc;
        Vector3 scale = fusionLock.transform.localScale;
        fusionLock.transform.localScale = scale * zLoc;
    }
}
