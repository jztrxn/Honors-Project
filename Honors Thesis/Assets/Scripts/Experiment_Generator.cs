using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class Experiment_Generator : MonoBehaviour
{
  
    public void Generate(Session session)
    {
        int numTrials = 10;
        session.CreateBlock(numTrials);
    }

}
