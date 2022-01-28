// (c) Copyright 2020 HP Development Company, L.P.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace HP.Glia.Examples.Common
{
    public class UIGraph : MonoBehaviour {
        public List<float> Data;
        public List<float> SD;

        public GameObject PointPrefab;
        public GameObject LinePrefab;

        public float LineWidth = 20;

        [Range(0f, 0.25f)]
        public float Margin = 0.1f;

        private List<GameObject> m_points = new List<GameObject>();
        private List<GameObject> m_lines = new List<GameObject>();
        private const string GraphPointID = "GraphPoint";
        private const string GraphLineID = "GraphLine";
        private const string SDLineID = "SDLine";

        GameObject pointsRoot;
        GameObject linesRoot;

        bool initialized = false;

        void Start()
        {
            ObjectPooler.Register(new ObjectPoolItem{id = GraphPointID + name,objectToPool = PointPrefab, shouldExpand = true, expandPoolSize = 100});
            ObjectPooler.Register(new ObjectPoolItem{id = GraphLineID + name, objectToPool = LinePrefab, shouldExpand = true, expandPoolSize = 100});
            PointPrefab.gameObject.SetActive(false);
            LinePrefab.gameObject.SetActive(false);
            
            linesRoot = new GameObject("Lines");
            linesRoot.transform.SetParent(transform);
            RectTransform linesRT = linesRoot.AddComponent<RectTransform>();
            linesRT.transform.localPosition = Vector3.zero;
            linesRT.transform.localRotation = Quaternion.identity;
            linesRT.transform.localScale = Vector3.one;
            linesRT.anchorMax = Vector2.one;
            linesRT.anchorMin = Vector2.zero;
            linesRT.anchoredPosition = Vector2.zero;
            linesRT.sizeDelta = Vector2.zero;

            pointsRoot = new GameObject("Points");
            pointsRoot.transform.SetParent(transform);
            RectTransform pointsRT = pointsRoot.AddComponent<RectTransform>();
            pointsRT.transform.localPosition = Vector3.zero;
            pointsRT.transform.localRotation = Quaternion.identity;
            pointsRT.transform.localScale = Vector3.one;
            pointsRT.anchorMax = Vector2.one;
            pointsRT.anchorMin = Vector2.zero;
            pointsRT.anchoredPosition = Vector2.zero;
            pointsRT.sizeDelta = Vector2.zero;

            initialized = true;
        }

        [ContextMenu("Draw graph")]
        public void Draw(){
            if(!initialized){ Start(); }
            
            Clear();

            GameObject prevPoint = null;
            GameObject currentPoint = null; 

            int count = Data.Count;

            for(int i = 0; i < count; i++){
                float x = (float) i / (float) (count - 1);
                float y = Data[i];

                x = (x * (1-Margin*2)) + Margin;
                y = (y * (1-Margin*2)) + Margin;

                Vector2 currentPointPosition = new Vector2(x,y);

                currentPoint = ObjectPooler.GetObject(GraphPointID + name);

                //Debug.Log(name + "  " + i, currentPoint);
                currentPoint.transform.SetParent(pointsRoot.transform, false);
                currentPoint.gameObject.SetActive(true);
                currentPoint.transform.localPosition = Vector3.zero;
                currentPoint.transform.localRotation = Quaternion.identity;
                currentPoint.transform.localScale = Vector3.one;

                //currentPoint.name = i.ToString();

                RectTransform pointRT = (RectTransform) currentPoint.transform;
                pointRT.localScale = Vector2.one;
                pointRT.anchorMax = currentPointPosition;
                pointRT.anchorMin = currentPointPosition;
                pointRT.anchoredPosition = Vector2.zero;

                if(prevPoint){
                    Vector2 previousPosition = ((RectTransform) prevPoint.transform).anchorMax;
                    float pointSize = ((RectTransform) prevPoint.transform).sizeDelta.y;
                    GameObject line = ObjectPooler.GetObject(GraphLineID + name);
                    line.transform.SetParent(linesRoot.transform, false);
                    line.gameObject.SetActive(true);
                    
                    line.transform.localPosition = Vector3.zero;
                    line.transform.localRotation = Quaternion.identity;
                    line.transform.localScale = Vector3.one;

                    RectTransform lineRT = (RectTransform) line.transform;
                    lineRT.localScale = Vector2.one;
                    lineRT.transform.localPosition = Vector3.zero;

                    lineRT.pivot = new Vector2(0, 0.5f);
                    lineRT.anchorMin = previousPosition;
                    lineRT.anchorMax = previousPosition;
                    lineRT.anchoredPosition = Vector2.zero;
                    lineRT.sizeDelta = new Vector2(Vector2.Distance(currentPoint.transform.localPosition, prevPoint.transform.localPosition), LineWidth);
                    
                    lineRT.localRotation = Quaternion.Euler(0,0, Mathf.Atan2(
                        (currentPoint.transform.localPosition.y  - (prevPoint.transform.localPosition.y)), 
                        (currentPoint.transform.localPosition.x  - prevPoint.transform.localPosition.x)) * Mathf.Rad2Deg);

                    m_lines.Add(line);
                }

                
                m_points.Add(currentPoint);
                prevPoint = currentPoint;
            }
        }

        private void Clear()
        {
            for(int i = 0; i < m_lines.Count; i++){
                ObjectPooler.Return(m_lines[i]);
            }

            m_lines.Clear();

            
            for(int i = 0; i < m_points.Count; i++){
                ObjectPooler.Return(m_points[i]);
            }

            m_points.Clear();
        }


        void Update()
        {
            if(Input.GetKeyDown(KeyCode.L)){
                Draw();
            }
        }
    }
}