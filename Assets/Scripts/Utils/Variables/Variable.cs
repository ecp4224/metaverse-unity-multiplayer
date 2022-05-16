using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Variable<T> : ScriptableObject
{
    [SerializeField]
    private T _value;

    public T Value
    {
        get { return _value; }
        set
        {
            var oldValue = _value;
            _value = value;

            if (OnValueChanged == null) return;
            
            var args = new VariableChangeEventArgs(oldValue, _value);
            OnValueChanged(this, args);
        }
    }

    public event EventHandler<VariableChangeEventArgs> OnValueChanged;
    
    public class VariableChangeEventArgs : EventArgs
    {
        public T oldValue { get; private set; }
        public T newValue { get; private set; }

        public VariableChangeEventArgs(T oldValue, T newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
    }
}
