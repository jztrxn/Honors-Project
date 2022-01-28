// (c) Copyright 2020 HP Development Company, L.P.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HP.Glia.Examples.Common
{
    public class CLGraph : UILineRenderer {

        public List<float> Sd;

        public Color SDColor;

        public override int OnBeforeDrawing(VertexHelper vh){ 
            if(points.Count < 2){ 
                return 0;
            }

            for(int i = 0; i < points.Count - 1; i++){
                Vector2 pointA = points[i]; // +- SD
                Vector2 pointB = points[i+1]; // +- SD
                
            }

            for(int i = 0; i < points.Count-1; i++){
                int index = i * 4;
            }
            
            return 0;
        }
        
    }
}