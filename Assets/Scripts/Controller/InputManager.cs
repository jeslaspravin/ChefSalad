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

        public void setIsActiveSet(bool active)
        {
            bIsActiveSet = active;
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
    public bool stopListening(string setName)
    {
        return inputSet.Remove(setName);
    }
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
            inputSet[setName] = availableSet;
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
            inputSet[setName] = availableSet;
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
