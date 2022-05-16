using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextFromStringVariable : BindableMonoBehavior
{
    [BindComponent]
    private Text text;
    
    public StringVariable variable;

    void Start()
    {
        text.text = variable.Value;
        
        variable.OnValueChanged += VariableOnOnValueChanged;
    }

    private void OnDestroy()
    {
        variable.OnValueChanged -= VariableOnOnValueChanged;
    }

    private void VariableOnOnValueChanged(object sender, Variable<string>.VariableChangeEventArgs e)
    {
        text.text = variable.Value;
    }
}
