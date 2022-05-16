using System;

public class ValueChangedEventArgs<T> : EventArgs
{
    public T OldValue { get; private set; }
    public T NewValue { get; private set; }
        
    public ValueChangedEventArgs(T old, T @new)
    {
        this.OldValue = old;
        this.NewValue = @new;
    }
}