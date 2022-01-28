// (c) Copyright 2020 HP Development Company, L.P.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace HP.Glia.Examples.Common
{
    public class UILineRenderer : Graphic
    {
        public Vector2Int gridSize;
        public Color SDAreaColor;

        public float lineThickness;
        public List<Vector2> points = new List<Vector2>();

        public List<float> sdData = new List<float>();



        protected float width;
        protected float height;
        protected float unitWidth;
        protected float unitHeight;
        Vector2 pointA;
        Vector2 pointB;
        float sdDataA;
        float sdDataB;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            width = rectTransform.rect.width;
            height = rectTransform.rect.height;

            if (points.Count >= gridSize.x)
            {
                gridSize.x = (int)(points.Count * 1.75);
            }
            unitHeight = height / (float)gridSize.y;
            unitWidth = width / (float)gridSize.x;

            int offset = OnBeforeDrawing(vh);
            if (points.Count < 2)
            {
                return;
            }

            for (int i = 0; i < points.Count - 1; i++)
            {

                pointA = points[i];
                pointB = points[i + 1];
                sdDataA = sdData[i];
                sdDataB = sdData[i + 1];
                // Debug.Log("Populate Mesh .x, y: (" + points[i].x + ", " + points[i].y + ")    SD.A: " + sdDataA);

                VertexsForLineBetweenPoints(pointA, pointB, lineThickness, color, vh);
                VertexsForPointCap(pointA, lineThickness, color, vh);

                //Draws SD bar graphs on CL points
                UncertaintyAreaOnPoint(pointA, lineThickness, sdDataA, color, vh);

                //Draws Uncertainty Area
            // UncertaintyAreaBetweenPoints(pointA, pointB, lineThickness, sdDataA, sdDataB, color, vh);

            }

            for (int i = 0; i < points.Count - 1; i++)
            {
                int index = offset + i * 12;
                //Draws Uncertainty             
            vh.AddTriangle(index + 8, index + 9, index + 10);
            vh.AddTriangle(index + 11, index + 10, index + 8);

                //Draws "Cog Load" Line
                vh.AddTriangle(index + 0, index + 1, index + 2);
                vh.AddTriangle(index + 1, index + 2, index + 3);

                //Draws Caps
                vh.AddTriangle(index + 4, index + 5, index + 6);
                vh.AddTriangle(index + 7, index + 6, index + 4);

            }

            OnAfterDrawing(vh, (points.Count - 1) * 8);
        }

        Color sdDefault = new Color(1f, 1f, 0f);
        Color sdLow = new Color(1f, 0f, 0f);
        Color sdHigh = new Color(0f, 1f, 0f);

        private void UncertaintyAreaOnPoint(Vector2 point, float thickness, float StD, Color color, VertexHelper vh)
        {
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = sdDefault;


            if (StD >= .4f) vertex.color = sdLow;
            if (StD <= .10f) vertex.color = sdHigh;



            // vertex.position = new Vector3(thickness / 1.5f, thickness / 1.5f, 0);
            vertex.position = new Vector3(thickness / 1.5f, StD * unitHeight, 0);
            vertex.position += new Vector3(unitWidth * point.x - width / 2f, unitHeight * point.y - height / 2f);
            vertex.position.y = Mathf.Clamp(vertex.position.y, -height/2.0f, height/2.0f);
            vh.AddVert(vertex);

            //vertex.position = new Vector3(thickness / 1.5f, -thickness / 1.5f, 0);
            vertex.position = new Vector3(thickness / 1.5f, -StD * unitHeight, 0);
            vertex.position += new Vector3(unitWidth * point.x - width / 2f, unitHeight * point.y - height / 2f);
            vertex.position.y = Mathf.Clamp(vertex.position.y, -height/2.0f, height/2.0f);
            vh.AddVert(vertex);

            //vertex.position = new Vector3(-thickness / 1.5f, -thickness / 1.5f, 0);
            vertex.position = new Vector3(-thickness / 1.5f, -StD * unitHeight, 0);
            vertex.position += new Vector3(unitWidth * point.x - width / 2f, unitHeight * point.y - height / 2f);
            vertex.position.y = Mathf.Clamp(vertex.position.y, -height/2.0f, height/2.0f);
            vh.AddVert(vertex);

            //vertex.position = new Vector3(-thickness / 1.5f, thickness / 1.5f, 0);
            vertex.position = new Vector3(-thickness / 1.5f, StD * unitHeight, 0);
            vertex.position += new Vector3(unitWidth * point.x - width / 2f, unitHeight * point.y - height / 2f);        
            vertex.position.y = Mathf.Clamp(vertex.position.y, -height/2.0f, height/2.0f);
            vh.AddVert(vertex);

        }

        public float GetAngle(Vector2 o, Vector2 target)
        {
            return (float)(Mathf.Atan2(unitHeight * (target.y - o.y), unitWidth * (target.x - o.x)) * (180 / Mathf.PI));
        }

        UIVertex vertex = UIVertex.simpleVert;
        
        protected void VertexsForLineBetweenPoints(Vector2 pointA, Vector2 pointB, float thickness, Color color, VertexHelper vh)
        {
            vertex.color = color;

            float angle = GetAngle(pointA, pointB) + 45f;
            //vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
            vertex.position = Quaternion.Euler(0, 0, 25f) * new Vector3(-thickness / 2, 0);
            vertex.position += new Vector3(unitWidth * pointA.x - width / 2f, unitHeight * pointA.y - height / 2f);
            vh.AddVert(vertex);

            //vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
            vertex.position = Quaternion.Euler(0, 0, 25f) * new Vector3(thickness / 2, 0);
            vertex.position += new Vector3(unitWidth * pointA.x - width / 2f, unitHeight * pointA.y - height / 2f);
            vh.AddVert(vertex);

            //vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
            vertex.position = Quaternion.Euler(0, 0, 25) * new Vector3(-thickness / 2, 0);
            vertex.position += new Vector3(unitWidth * pointB.x - width / 2f, unitHeight * pointB.y - height / 2f);
            vh.AddVert(vertex);

            //vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
            vertex.position = Quaternion.Euler(0, 0, 25f) * new Vector3(thickness / 2, 0);
            vertex.position += new Vector3(unitWidth * pointB.x - width / 2f, unitHeight * pointB.y - height / 2f);
            vh.AddVert(vertex);
        }

        protected void VertexsForPointCap(Vector2 point, float thickness, Color color, VertexHelper vh)
        {
            vertex.color = color;

            vertex.position = new Vector3(thickness / 1.5f + unitWidth * point.x - width / 2f, thickness / 1.5f + (unitHeight * point.y - height / 2f), 0);
            vh.AddVert(vertex);

            vertex.position = new Vector3(thickness / 1.5f + unitWidth * point.x - width / 2f, -thickness / 1.5f + (unitHeight * point.y - height / 2f), 0);
            vh.AddVert(vertex);

            vertex.position = new Vector3(-thickness / 1.5f + unitWidth * point.x - width / 2f, -thickness / 1.5f + (unitHeight * point.y - height / 2f), 0);
            vh.AddVert(vertex);

            vertex.position = new Vector3(-thickness / 1.5f + unitWidth * point.x - width / 2f, thickness / 1.5f + (unitHeight * point.y - height / 2f), 0);
            vh.AddVert(vertex);
        }

        protected void UncertaintyAreaBetweenPoints(Vector2 pointA, Vector2 pointB, float thickness, float StDA, float StDB, Color color, VertexHelper vh)
        {

            vertex.color = color;
            vertex.color = SDAreaColor;

            // if (StDA >= .4f) vertex.color = new Color(1f, 0f, 0f);
            // if (StDA <= .15f) vertex.color = new Color(0f, 1f, 0f);

            vertex.position = new Vector3(0 + unitWidth * pointA.x - width / 2f, unitHeight * StDA + unitHeight * pointA.y - height / 2f, 0);
            vh.AddVert(vertex);

            vertex.position = new Vector3(0 + unitWidth * pointA.x - width / 2f, -StDA * unitHeight + unitHeight * pointA.y - height / 2f, 0);
            vh.AddVert(vertex);

            vertex.position = new Vector3(0 + unitWidth * pointB.x - width / 2f, -StDB * unitHeight + unitHeight * pointB.y - height / 2f, 0);
            vh.AddVert(vertex);

            vertex.position = new Vector3(0 + unitWidth * pointB.x - width / 2f, StDB * unitHeight + unitHeight * pointB.y - height / 2f, 0);
            vh.AddVert(vertex);


        }
        public virtual int OnBeforeDrawing(VertexHelper vh) { return 0; }

        public virtual void OnAfterDrawing(VertexHelper vh, int offset) { }
    }
}