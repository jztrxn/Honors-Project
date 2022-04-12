using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHolder
{
    public static Dictionary<string, Vector3> boundaries = new Dictionary<string, Vector3>()
    {
        {"leftBound", new Vector3()},
        {"rightBound", new Vector3()},
        {"topBound", new Vector3()},
        {"botBound", new Vector3()}

    };
    public static Dictionary<string, float[]> boundaryPupilLoc = new Dictionary<string, float[]>()
    {
        {"leftBound", null},
        {"rightBound", null},
        {"topBound", null},
        {"botBound", null}

    };

    public static float deviceHeight { get; set; }
}
