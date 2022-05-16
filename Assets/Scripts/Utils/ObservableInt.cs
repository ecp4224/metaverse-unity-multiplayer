using System;
using UnityEngine;

[Serializable]
public class ObservableInt
{
    public event EventHandler<ValueChangedEventArgs<int>> OnValueChanged;

    [SerializeField]
    private int _value;

    public int Value
    {
        get { return _value; }
        set
        {
            if (_value != value && OnValueChanged != null)
            {
                OnValueChanged(this, new ValueChangedEventArgs<int>(_value, value));
            }

            _value = value;
        }
    }

    #region Operations

    public static explicit operator int(ObservableInt observableInt)
    {
        return observableInt.Value;
    }
    
    public static explicit operator ObservableInt(int observableInt)
    {
        return new ObservableInt()
        {
            Value = observableInt
        };
    }

    
    public static ObservableInt operator +(ObservableInt observable, int value)
    {
        observable.Value += value;
        return observable;
    }

    public static ObservableInt operator -(ObservableInt observable, int value)
    {
        observable.Value -= value;
        return observable;
    }

    public static ObservableInt operator /(ObservableInt observable, int value)
    {
        observable.Value /= value;
        return observable;
    }

    public static ObservableInt operator *(ObservableInt observable, int value)
    {
        observable.Value *= value;
        return observable;
    }
    
    public static ObservableInt operator ^(ObservableInt observable, int value)
    {
        observable.Value ^= value;
        return observable;
    }
    
    public static ObservableInt operator |(ObservableInt observable, int value)
    {
        observable.Value |= value;
        return observable;
    }
    
    public static ObservableInt operator &(ObservableInt observable, int value)
    {
        observable.Value &= value;
        return observable;
    }
    
    public static ObservableInt operator +(ObservableInt observable, ObservableInt value)
    {
        observable.Value += value.Value;
        return observable;
    }

    public static ObservableInt operator -(ObservableInt observable, ObservableInt value)
    {
        observable.Value -= value.Value;
        return observable;
    }

    public static ObservableInt operator /(ObservableInt observable, ObservableInt value)
    {
        observable.Value /= value.Value;
        return observable;
    }

    public static ObservableInt operator *(ObservableInt observable, ObservableInt value)
    {
        observable.Value *= value.Value;
        return observable;
    }
    
    public static ObservableInt operator ^(ObservableInt observable, ObservableInt value)
    {
        observable.Value ^= value.Value;
        return observable;
    }
    
    public static ObservableInt operator |(ObservableInt observable, ObservableInt value)
    {
        observable.Value |= value.Value;
        return observable;
    }
    
    public static ObservableInt operator &(ObservableInt observable, ObservableInt value)
    {
        observable.Value &= value.Value;
        return observable;
    }

    public static bool operator <(ObservableInt observableInt, int value)
    {
        return observableInt.Value < value;
    }
    
    public static bool operator >(ObservableInt observableInt, int value)
    {
        return observableInt.Value > value;
    }
    
    public static bool operator <=(ObservableInt observableInt, int value)
    {
        return observableInt.Value <= value;
    }
    
    public static bool operator >=(ObservableInt observableInt, int value)
    {
        return observableInt.Value >= value;
    }
    
    public static bool operator ==(ObservableInt observableInt, int value)
    {
        return observableInt.Value == value;
    }
    
    public static bool operator !=(ObservableInt observableInt, int value)
    {
        return observableInt.Value != value;
    }
    
    public static bool operator <(ObservableInt observableInt, ObservableInt value)
    {
        return observableInt.Value < value.Value;
    }
    
    public static bool operator >(ObservableInt observableInt, ObservableInt value)
    {
        return observableInt.Value > value.Value;
    }
    
    public static bool operator <=(ObservableInt observableInt, ObservableInt value)
    {
        return observableInt.Value <= value.Value;
    }
    
    public static bool operator >=(ObservableInt observableInt, ObservableInt value)
    {
        return observableInt.Value >= value.Value;
    }
    
    public static bool operator ==(ObservableInt observableInt, ObservableInt value)
    {
        return observableInt.Value == value.Value;
    }
    
    public static bool operator !=(ObservableInt observableInt, ObservableInt value)
    {
        return observableInt.Value != value.Value;
    }
    #endregion
}