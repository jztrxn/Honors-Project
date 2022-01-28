// (c) Copyright 2020 HP Development Company, L.P.

using System;
using HP.Omnicept.Messaging.Messages;
using UnityEngine;
using TMPro;

namespace HP.Glia.Examples.Display
{
    public class UGUIDisplayHR : UGUIBaseDisplay {

        [SerializeField] private TextMeshProUGUI hrText = null;
        void Start()
        {
            gliaBehaviour.OnHeartRate.AddListener(OnHeartRate);
        }

        private void OnHeartRate(HeartRate hr)
        {
            if(hr != null){
                hrText.text = hr.Rate.ToString();
            }
        }
    }
}