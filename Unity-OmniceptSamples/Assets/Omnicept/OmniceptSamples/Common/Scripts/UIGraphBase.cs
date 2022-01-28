// (c) Copyright 2020 HP Development Company, L.P.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HP.Glia.Examples.Common
{
    public class UIGraphBase : Graphic
    {  
        public Vector2Int gridSize = new Vector2Int(1,1);
        public float thickness;

        float width;
        float height;
        float cellWidth;
        float cellHeight;

        protected override void OnPopulateMesh(VertexHelper vh){
            vh.Clear();

            width = rectTransform.rect.width;
            height = rectTransform.rect.height;
            
            cellHeight = height/(float) gridSize.y;
            cellWidth = width/(float) gridSize.x;

            int count = 0;
            for(int y = 0; y < gridSize.y; y++)
            {
                for(int x = 0; x < gridSize.x; x++)
                {
                    DrawCell(x,y, count, vh);
                    count ++;
                }
            }
        }

        private void DrawCell(int x, int y, int index, VertexHelper vh){
            float xPos = (cellWidth * x) - width/2f;
            float yPos = (cellHeight * y) - height/2f;

            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;

            vertex.position = new Vector3(xPos, yPos);
            vh.AddVert(vertex);

            vertex.position = new Vector3(xPos, yPos + cellHeight);
            vh.AddVert(vertex);

            vertex.position = new Vector3(xPos + cellWidth, yPos + cellHeight);
            vh.AddVert(vertex);

            
            vertex.position = new Vector3(xPos + cellWidth, yPos);
            vh.AddVert(vertex);


            float widthSqr = thickness * thickness;
            float distanceSqr = widthSqr / 2f;
            float distance = Mathf.Sqrt(distanceSqr);

            vertex.position = new Vector3(xPos + distance, yPos + distance);
            vh.AddVert(vertex);

            vertex.position = new Vector3(xPos + distance, yPos + (cellHeight - distance));
            vh.AddVert(vertex);
            
            vertex.position = new Vector3(xPos + (cellWidth - distance), yPos + (cellHeight- distance));
            vh.AddVert(vertex);
            
            vertex.position = new Vector3(xPos + (cellWidth - distance), yPos +  distance);
            vh.AddVert(vertex);

            int offset = index*8;

            //Left 
            vh.AddTriangle(offset + 0,offset + 1,offset + 5);
            vh.AddTriangle(offset + 5,offset + 4,offset + 0);

            //Top
            vh.AddTriangle(offset + 1,offset + 2,offset + 6);
            vh.AddTriangle(offset + 6,offset + 5,offset + 1);

            //Right
            vh.AddTriangle(offset + 2,offset + 3,offset + 7);
            vh.AddTriangle(offset + 7,offset + 6,offset + 2);

            //Bottom
            vh.AddTriangle(offset + 3,offset + 0,offset + 4);
            vh.AddTriangle(offset + 4,offset + 7,offset + 3);
        }

    }
}
