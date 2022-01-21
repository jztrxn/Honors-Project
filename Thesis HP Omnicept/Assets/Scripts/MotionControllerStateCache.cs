using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Input;
using System;

public class MotionControllerStateCache : MonoBehaviour
{
    /// <summary> 
    /// Internal helper class which associates a Motion Controller 
    /// and its known state 
    /// </summary> 
    private class MotionControllerState
    {
        /// <summary> 
        /// Construction 
        /// </summary> 
        /// <param name="mc">motion controller</param>` 
        public MotionControllerState(MotionController mc)
        {
            this.MotionController = mc;
        }

        /// <summary> 
        /// Motion Controller that the state represents 
        /// </summary> 
        public MotionController MotionController { get; private set; }

        /// <summary> 
        /// Update the current state of the motion controller 
        /// </summary> 
        /// <param name="when">time of the reading</param> 
        public void Update(DateTime when)
        {
            this.CurrentReading = this.MotionController.TryGetReadingAtTime(when);
        }

        /// <summary> 
        /// Last reading from the controller 
        /// </summary> 
        public MotionControllerReading CurrentReading { get; private set; }

    }

    /// <summary> 
    /// Updates the input states of the known motion controllers 
    /// </summary> 
    public void Update()
    {
        var now = DateTime.Now;

        lock (_controllers)
        {
            foreach (var controller in _controllers)
            {
                controller.Value.Update(now);
            }
        }
    }


    private MotionControllerWatcher _watcher;
    private Dictionary<Handedness, MotionControllerState>
        _controllers = new Dictionary<Handedness, MotionControllerState>();

    /// <summary> 
    /// Starts monitoring controller's connections and disconnections 
    /// </summary> 
    public void Start()
    {
        _watcher = new MotionControllerWatcher();
        _watcher.MotionControllerAdded += _watcher_MotionControllerAdded;
        _watcher.MotionControllerRemoved += _watcher_MotionControllerRemoved;
        var nowait = _watcher.StartAsync();
    }

    /// <summary> 
    /// Stops monitoring controller's connections and disconnections 
    /// </summary> 
    public void Stop()
    {
        if (_watcher != null)
        {
            _watcher.MotionControllerAdded -= _watcher_MotionControllerAdded;
            _watcher.MotionControllerRemoved -= _watcher_MotionControllerRemoved;
            _watcher.Stop();
        }
    }

    /// <summary> 
    /// called when a motion controller has been removed from the system: 
    /// Remove a motion controller from the cache 
    /// </summary> 
    /// <param name="sender">motion controller watcher</param> 
    /// <param name="e">motion controller </param> 
    private void _watcher_MotionControllerRemoved(object sender, MotionController e)
    {
        lock (_controllers)
        {
            _controllers.Remove(e.Handedness);
        }
    }

    /// <summary> 
    /// called when a motion controller has been added to the system: 
    /// Remove a motion controller from the cache 
    /// </summary> 
    /// <param name="sender">motion controller watcher</param> 
    /// <param name="e">motion controller </param> 
    private void _watcher_MotionControllerAdded(object sender, MotionController e)
    {
        lock (_controllers)
        {
            _controllers[e.Handedness] = new MotionControllerState(e);
        }
    }

    /// <summary> 
    /// Returns the current value of a controller input such as button or trigger 
    /// </summary> 
    /// <param name="handedness">Handedness of the controller</param> 
    /// <param name="input">Button or Trigger to query</param> 
    /// <returns>float value between 0.0 (not pressed) and 1.0 
    /// (fully pressed)</returns> 
    public float GetValue(Handedness handedness, ControllerInput input)
    {
        MotionControllerReading currentReading = null;

        lock (_controllers)
        {
            if (_controllers.TryGetValue(handedness, out MotionControllerState mc))
            {
                currentReading = mc.CurrentReading;
            }
        }

        return (currentReading == null) ? 0.0f : currentReading.GetPressedValue(input);
    }

    /*
    /// <summary> 
    /// Returns the current value of a controller input such as button or trigger 
    /// </summary> 
    /// <param name="handedness">Handedness of the controller</param> 
    /// <param name="input">Button or Trigger to query</param> 
    /// <returns>float value between 0.0 (not pressed) and 1.0 
    /// (fully pressed)</returns>  
    
    public float GetValue(UnityEngine.XR.WSA.Input.InteractionSourceHandedness handedness, ControllerInput input)
    {
        return GetValue(Convert(handedness), input);
    }

    /// <summary> 
    /// Returns a boolean indicating whether a controller input such as button or trigger is pressed 
    /// </summary> 
    /// <param name="handedness">Handedness of the controller</param> 
    /// <param name="input">Button or Trigger to query</param> 
    /// <returns>true if pressed, false if not pressed</returns> 
    public bool IsPressed(Handedness handedness, ControllerInput input)
    {
        return GetValue(handedness, input) >= PressedThreshold;
    }
    */
}