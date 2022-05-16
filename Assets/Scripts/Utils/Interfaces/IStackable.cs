using System;

public interface IStackable
{
    event EventHandler<ValueChangedEventArgs<int>> OnStackChanged; 
    
    int Stack { get; set; }
    
    bool IsStackable { get; set; }
    
    int MaxStack { get; set; }
}