using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRUiKits.Utils;

public class DisableIfInputEmpty : BindableMonoBehavior
{
    [BindComponent]
    private Button _button;

    public UIKitInputField _InputField;

    void Update()
    {
        _button.interactable = _InputField.text.Length > 2;
    }
}
