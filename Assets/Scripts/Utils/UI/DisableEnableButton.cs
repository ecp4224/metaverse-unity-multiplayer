using System;
using UnityEngine;
using UnityEngine.UI;

namespace ERC721ContractLibrary.Contracts.ERC721PresetMinterPauserAutoId.ContractDefinition.Utils.UI
{
    public class DisableEnableButton : BindableMonoBehavior
    {
        [BindComponent]
        private Button _button;
        
        public Image disabledImage;
        public Image enabledImage;

        private void FixedUpdate()
        {
            disabledImage.enabled = !_button.interactable;
            enabledImage.enabled = _button.interactable;
        }
    }
}