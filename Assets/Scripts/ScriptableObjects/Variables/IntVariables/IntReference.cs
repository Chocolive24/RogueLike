using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IntReference
{
    [SerializeField] private bool UseConstant = false;
    public int ConstantValue;
    public IntVariable Variable;

    public int Value { get => UseConstant ? ConstantValue : Variable.Value; }


    public void SetValue(int newValue)
    {
        if (UseConstant)
        {
            ConstantValue = newValue;
        }
        else
        {
            Variable.Value = newValue;
        }
    }

    public void AddValue(int value)
    {
        if (UseConstant)
        {
            ConstantValue += value;
        }
        else
        {
            Variable.Value += value;
        }
    }
    
    public void SubstractValue(int value)
    {
        if (UseConstant)
        {
            ConstantValue -= value;
        }
        else
        {
            Variable.Value -= value;
        }
    }
    
    
    
}
