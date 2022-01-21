// (c) Copyright 2020 HP Development Company, L.P.


using System.Collections.Generic;
using UnityEngine;

namespace HP.Glia.Examples.Common
{
    public class SDGraph : UILineRenderer
    {
        public List<float> Sd = new List<float>();
        public List<float> Cl = new List<float>();

        Vector2 temp;

        public void ShowResult(CognitiveLoadMonitor clMonitor)
        {
            //Grab float Lists of CL and SD data
            Sd = clMonitor.GetSDData();
            Cl = clMonitor.GetData();

            if(Cl.Count < points.Count){ points.Clear(); sdData.Clear(); }
        
        
            for (int i = points.Count; i < Cl.Count; i++)
            {
                //This creates the sample, CL-reading as a point for LineRenderer
                temp.x = (float)i;
                temp.y = Cl[i];

                points.Add(temp);
                sdData.Add(Sd[i]);
            }
            
            UpdateGeometry();
        }

    }
}