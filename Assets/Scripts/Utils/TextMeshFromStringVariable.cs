using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextMeshFromStringVariable : BindableMonoBehavior
{
    [BindComponent]
    private TextMeshProUGUI text;
    
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
