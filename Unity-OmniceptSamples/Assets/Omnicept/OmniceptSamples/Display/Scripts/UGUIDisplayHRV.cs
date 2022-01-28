// (c) Copyright 2020 HP Development Company, L.P.
using UnityEngine;
using System;
using HP.Omnicept.Messaging.Messages;
using TMPro;

namespace  HP.Glia.Examples.Display{
    public class UGUIDisplayHRV : UGUIBaseDisplay {

        [SerializeField] private TextMeshProUGUI hrText = null;

        void Start()
        {
            gliaBehaviour.OnHeartRateVariability.AddListener(OnHeartRateVariability);
        }

        private void OnHeartRateVariability(HeartRateVariability hrv)
        {
            if(hrv != null){
                hrText.text = $"HRV: {hrv.Sdnn.ToString("0.00")}";
            }
        }
    }
}