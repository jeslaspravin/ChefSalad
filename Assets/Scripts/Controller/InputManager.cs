using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Input manager that holds the input mapping and will be responsible for inputs data being passed on to listeners.
/// </summary>
public class InputManager : MonoBehaviour {

    /// <summary>
    /// Input set data struct ,Set of inputs that needs to be controllable in same way can be in single set.
    /// </summary>
    struct InputSetStruct
    {
        /// <summary>
        /// Name of input list this set listens to.
        /// </summary>
        public List<string> inputName;

        /// <summary>
        /// For each of inputName item there will be a callback that gets invoked with value of input.
        /// </summary>
        public List<Action<float>> callbacks;

        /// <summary>
        /// Can be used to control whether to listen to this set.
        /// </summary>
        public bool bIsActiveSet;
        public InputSetStruct(List<string> inputNameList,List<Action<float>> callbackList)
        {
            bIsActiveSet = true;
            inputName = inputNameList;
            callbacks = callbackList;
        }
        public InputSetStruct(string firstInputName, Action<float> firstCallback)
        {
            bIsActiveSet = true;
            inputName = new List<string>();
            callbacks = new List<Action<float>>();
            inputName.Add(firstInputName);
            callbacks.Add(firstCallback);
        }

        public void setIsActiveSet(bool active)
        {
            bIsActiveSet = active;
        }
    }
    /// <summary>
    /// Dictionary of all input sets that user controller listens to.
    /// </summary>
    private Dictionary<string, InputSetStruct> inputSet = new Dictionary<string, InputSetStruct>();

    /// <summary>
    /// Can be used to pause the controller all together using this global switch.
    /// </summary>
    private bool bGlobalPause;

    /// <summary>
    /// Delegate for processed input event
    /// </summary>
    public delegate void InputManagerDelegate();

    /// <summary>
    /// Event that gets triggered once all input events are processed.
    /// </summary>
    public event InputManagerDelegate onInputProcessed;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        foreach (string key in inputSet.Keys)
        {
            for(int i=0;i< inputSet[key].callbacks.Count;i++)
            {
                inputSet[key].callbacks[i]((!bGlobalPause && inputSet[key].bIsActiveSet)? Input.GetAxis(inputSet[key].inputName[i]):0);
            }
        }
        if(onInputProcessed!=null)
            onInputProcessed.Invoke();
    }

    /// <summary>
    /// Adds listener to set of particular name in the Input manager.
    /// </summary>
    /// <param name="setName">Name of Set</param>
    /// <param name="inputName">Input name to map into the set</param>
    /// <param name="methodCallback">Input callback to map into the set</param>
    public void addToListenerSet(string setName,string inputName,Action<float> methodCallback)
    {
        InputSetStruct set = new InputSetStruct(inputName, methodCallback); 

        if (inputSet.ContainsKey(setName))
        {
            inputSet[setName].inputName.Add(inputName);
            inputSet[setName].callbacks.Add(methodCallback);
        }
        else
        {
            inputSet.Add(setName, set);
        }        
    }

    /// <summary>
    /// Removes listening to the set of given name entirely.
    /// </summary>
    /// <param name="setName">Set name to stop listening to</param>
    /// <returns>True on successfully stopping</returns>
    public bool stopListening(string setName)
    {
        return inputSet.Remove(setName);
    }

    /// <summary>
    /// Stops listening to certain input of given name alone from set of given name.
    /// </summary>
    /// <param name="setName">Set name to look for input</param>
    /// <param name="inputName">Input name to remove</param>
    /// <returns>True on success</returns>
    public bool stopListening(string setName,string inputName)
    {
        if (inputSet.ContainsKey(setName))
        {
            int index= inputSet[setName].inputName.FindIndex((string val)=> { return val.Equals(inputName); });
            if(index != -1)
            {
                inputSet[setName].inputName.RemoveAt(index);
                inputSet[setName].callbacks.RemoveAt(index);
            }
        }
        return false;
    }

    /// <summary>
    /// Pauses listening,The entire input manager's global switch for input listening is turned off
    /// </summary>
    public void pauseInputs()
    {
        bGlobalPause = true;
    }

    /// <summary>
    /// Resumes the input manager.Flicks global switch back on.
    /// </summary>
    public void resumeInputs()
    {
        bGlobalPause = false;
    }

    public bool isInputsActive()
    {
        return !bGlobalPause;
    }

    /// <summary>
    /// Pauses input set of given name.
    /// </summary>
    /// <param name="setName">Name of input set to pause</param>
    /// <returns>True on success</returns>
    public bool pauseInputSet(string setName)
    {
        InputSetStruct availableSet = new InputSetStruct();
        if (inputSet.TryGetValue(setName, out availableSet))
        {
            availableSet.bIsActiveSet = false;
            inputSet[setName] = availableSet;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Resumes input set of given name.
    /// </summary>
    /// <param name="setName">Name of input set to resume</param>
    /// <returns>True on success</returns>
    public bool resumeInputSet(string setName)
    {
        InputSetStruct availableSet = new InputSetStruct();
        if (inputSet.TryGetValue(setName, out availableSet))
        {
            availableSet.bIsActiveSet = true;
            inputSet[setName] = availableSet;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Check whether input set is actively listening
    /// </summary>
    /// <param name="setName">Name of set to check</param>
    /// <returns>True when set is active</returns>
    public bool isInputSetActive(string setName)
    {
        InputSetStruct availableSet = new InputSetStruct();
        if (inputSet.TryGetValue(setName, out availableSet))
        {
            return !availableSet.bIsActiveSet;
        }
        return false;
    }


}
