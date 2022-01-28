// (c) Copyright 2020 HP Development Company, L.P.

using System;
using UnityEngine;

namespace HP.Glia.Examples.Common
{
    public class UnityObjectPoolItem : MonoBehaviour {
        [SerializeField]
        private ObjectPoolItem objectPoolItem;

        public string GetID(){
            return objectPoolItem.id;
        }

        public void Initialize(ObjectPoolItem objectPoolItem)
        {
            this.objectPoolItem = objectPoolItem;
        }
    }
}