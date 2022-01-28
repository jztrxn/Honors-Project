// (c) Copyright 2020 HP Development Company, L.P.

using HP.Omnicept.Unity;
using UnityEngine;

namespace HP.Glia.Examples.Display
{
    public class UGUIBaseDisplay : MonoBehaviour {
        private GliaBehaviour _gliaBehaviour;
        protected GliaBehaviour gliaBehaviour{
            get{
                if(_gliaBehaviour == null){
                    _gliaBehaviour = FindObjectOfType<GliaBehaviour>();
                }

                return _gliaBehaviour;
            }
        }
    }
}