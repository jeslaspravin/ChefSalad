using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    struct InputSetStruct
    {
        //public string setName;
        public List<string> inputName;
        public List<Action<float>> callbacks;
        public bool bIsActiveSet;
        public InputSetStruct(/*string name,*/List<string> inputNameList,List<Action<float>> callbackList)
        {
            //setName = name;
            bIsActiveSet = true;
            inputName = inputNameList;
            callbacks = callbackList;
        }
        public InputSetStruct(/*string name,*/ string firstInputName, Action<float> firstCallback)
        {
            //setName = name;
            bIsActiveSet = true;
            inputName = new List<string>();
            callbacks = new List<Action<float>>();
            inputName.Add(firstInputName);
            callbacks.Add(firstCallback);
        }

        //public override bool Equals(object obj)
        //{
        //    return obj is InputSetStruct && this == (InputSetStruct)obj;
        //}

        //public override int GetHashCode()
        //{
        //    return setName.GetHashCode();
        //}
        //public static bool operator ==(InputSetStruct x, InputSetStruct y)
        //{
        //    return x.setName.Equals(y.setName);
        //}
        //public static bool operator !=(InputSetStruct x, InputSetStruct y)
        //{
        //    return !(x == y);
        //}
    }

    private Dictionary<string, InputSetStruct> inputSet = new Dictionary<string, InputSetStruct>();

    private bool bGlobalPause;

    public delegate void InputManagerDelegate();

    public event InputManagerDelegate onInputProcessed;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        foreach (KeyValuePair<string, InputSetStruct> entry in inputSet)
        {
            for(int i=0;i< entry.Value.callbacks.Count;i++)
            {
                entry.Value.callbacks[i]((!bGlobalPause || entry.Value.bIsActiveSet)? Input.GetAxis(entry.Value.inputName[i]):0);
            }
        }
        onInputProcessed.Invoke();
    }

    public void addToListenerSet(string setName,string inputName,Action<float> methodCallback)
    {
        InputSetStruct set = new InputSetStruct(inputName, methodCallback);

        InputSetStruct availableSet = new InputSetStruct();

        if (inputSet.TryGetValue(setName,out availableSet))
        {
            availableSet.inputName.Add(inputName);
            availableSet.callbacks.Add(methodCallback);
        }
        else
        {
            inputSet.Add(setName, set);
        }        
    }
    public bool stopListening(string setName)
    {
        return inputSet.Remove(setName);
    }
    public bool stopListening(string setName,string inputName)
    {
        InputSetStruct availableSet = new InputSetStruct();
        if (inputSet.TryGetValue(setName, out availableSet))
        {
            int index=availableSet.inputName.FindIndex((string val)=> { return val.Equals(inputName); });
            if(index != -1)
            {
                availableSet.inputName.RemoveAt(index);
                availableSet.callbacks.RemoveAt(index);
            }
        }
        return false;
    }

    public void pauseInputs()
    {
        bGlobalPause = true;
    }

    public void resumeInputs()
    {
        bGlobalPause = false;
    }

    public bool isInputsActive()
    {
        return !bGlobalPause;
    }

    public bool pauseInputSet(string setName)
    {
        InputSetStruct availableSet = new InputSetStruct();
        if (inputSet.TryGetValue(setName, out availableSet))
        {
            availableSet.bIsActiveSet = false;
            return true;
        }
        return false;
    }

    public bool resumeInputSet(string setName)
    {
        InputSetStruct availableSet = new InputSetStruct();
        if (inputSet.TryGetValue(setName, out availableSet))
        {
            availableSet.bIsActiveSet = true;
            return true;
        }
        return false;
    }

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
