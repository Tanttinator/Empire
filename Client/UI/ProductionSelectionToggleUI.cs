using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Common;
using UnityEngine.Events;

namespace Client
{
    public class ProductionSelectionToggleUI : MonoBehaviour
    {
        [SerializeField] Toggle toggle = default;
        [SerializeField] TMP_Text unitName = default;
        [SerializeField] Image background = default;
        [SerializeField] Image unitIcon = default;

        public void Setup(UnitType unit, Color color, bool isSelected, UnityAction<bool> onToggle)
        {
            unitName.text = unit.name;

            background.color = color;
            unitIcon.color = color;

            unitIcon.sprite = SpriteRegistry.GetSprite(unit.name).GetSprite(false, false, false, false).sprite;

            toggle.isOn = isSelected;
            toggle.onValueChanged.AddListener(onToggle);
        }

        public void SetValue(bool value)
        {
            toggle.SetIsOnWithoutNotify(value);
        }
    }
}
