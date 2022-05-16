using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class StringVariableFromInputField : BindableMonoBehavior
{
    [BindComponent]
    private InputField input;

    public StringVariable variable;
    
    void Start()
    {
        input.onValueChanged.AddListener(Target);
    }

    private void OnDestroy()
    {
        input.onValueChanged.RemoveListener(Target);
    }

    private void Target(string arg0)
    {
        variable.Value = arg0;
    }
}
