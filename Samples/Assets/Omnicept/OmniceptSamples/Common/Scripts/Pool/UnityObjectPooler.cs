// (c) Copyright 2020 HP Development Company, L.P.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace HP.Glia.Examples.Common
{
    public class UnityObjectPooler : MonoBehaviour
    {
        private ObjectPoolItem objectPoolItem;

        private Queue<UnityObjectPoolItem> pool = new Queue<UnityObjectPoolItem>();

        public void Initialize(ObjectPoolItem item)
        {
            objectPoolItem = item;
            GrowPool(objectPoolItem.initialPoolSize);
        }


        public GameObject GetObject(){
            if(pool.Count > 0){
                return pool.Dequeue().gameObject;
            }
            else if(objectPoolItem.shouldExpand){
                GrowPool(objectPoolItem.expandPoolSize);
                return pool.Dequeue().gameObject;
            }
            else{
                return null;
            }
        }

        public void GrowPool(int size){
            for(int i = 0; i < size; i++){
                GameObject instance = Instantiate(objectPoolItem.objectToPool, transform);
                UnityObjectPoolItem unityObjectPoolItem = instance.AddComponent<UnityObjectPoolItem>();
                unityObjectPoolItem.Initialize(objectPoolItem);
                unityObjectPoolItem.gameObject.SetActive(false);
                pool.Enqueue(unityObjectPoolItem);
            }
        }

        public void Return(UnityObjectPoolItem poolItem)
        {
            if(poolItem.GetID() != objectPoolItem.id){
                Debug.LogError("Returning object to the wrong pool");
            }
            else{
                poolItem.transform.SetParent(transform);
                poolItem.gameObject.SetActive(false);
                pool.Enqueue(poolItem);
            }
        }
    }
}