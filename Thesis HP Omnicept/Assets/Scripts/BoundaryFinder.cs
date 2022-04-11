using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoundaryFinder : MonoBehaviour
{
    public GameObject marker;
    public TMP_Text myText;
    public float acceleration;

    private float[,] boundaries;
    private int currBoundary;

    private bool canSpace;
    private float hspeed;
    private float vspeed;
    private float maxSpeed = 0.02f;
    private float[] currCoord;

    // Start is called before the first frame update
    void Start()
    {
        boundaries = new float[4,4];
        
        StartCoroutine(GetBoundaries());
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.A))
        {
            hspeed = hspeed - acceleration * Time.deltaTime;
            if (Mathf.Abs(hspeed) > maxSpeed)
            {
                hspeed = -maxSpeed;
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            hspeed = hspeed + acceleration * Time.deltaTime;
            if (Mathf.Abs(hspeed) > maxSpeed)
            {
                hspeed = maxSpeed;
            }
        }
        else
        {
            hspeed = 0;
        }
        //marker.transform.Translate(vspeed, 0f, 0f);
        if (Input.GetKey(KeyCode.S))
        {
            vspeed = vspeed - acceleration * Time.deltaTime;
            if (Mathf.Abs(vspeed) > maxSpeed)
            {
                vspeed = -maxSpeed;
            }
        }
        else if (Input.GetKey(KeyCode.W))
        {
            vspeed = vspeed + acceleration * Time.deltaTime;
            if (Mathf.Abs(vspeed) > maxSpeed)
            {
                vspeed = maxSpeed;
            }
        }
        else
        {
            vspeed = 0;
        }
        marker.transform.Translate(hspeed, vspeed, 0f);

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > nextTrialTimer && canSpace)
        {
            nextTrialTimer = Time.time + waitSeconds;
            currCoord = this.GetComponent<GliaEyeTracking>().getEyeTracking();
            Debug.Log("Space Pressed");
        }
    }

    public float waitSeconds;
    private float nextTrialTimer = 0.0f;

    IEnumerator GetBoundaries()
    {
        List<string> keys = new List<string>(DataHolder.boundaries.Keys);
        foreach(string key in keys)
        {
            placeMarker();
            yield return new WaitForSeconds(1);
            Debug.LogFormat("Boundary Key being used: {0}", key);
            myText.text = "Getting " + key + ": Use A, D Keys";
            while (!Input.GetKeyDown(KeyCode.Space))
            {
                yield return null;
            }
            Debug.LogFormat("currCoords are: {0}", currCoord);
            DataHolder.boundaries[key] = marker.transform.position;
            DataHolder.boundaryPupilLoc[key] = currCoord;
            Debug.LogFormat("Marker Boundary: {0}", DataHolder.boundaries[key]);
        }

        //FindObjectOfType<ExperimentHandler>().startNextTrial();
        

    }

    public void placeMarker()
    {
        marker.transform.localPosition = new Vector3(0, 0, 3.0f);
        Debug.Log("Marker Placed");
    }
}
