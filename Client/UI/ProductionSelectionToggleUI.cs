using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Common;

namespace Client
{
    public class ProductionSelectionToggleUI : MonoBehaviour
    {

        [SerializeField] TMP_Text unitName = default;
        [SerializeField] Image background = default;
        [SerializeField] Image unitIcon = default;

        Action<bool> onToggle;

        public void Setup(UnitType unit, Color color, Action<bool> onToggle)
        {
            unitName.text = unit.name;

            background.color = color;
            unitIcon.color = color;

            unitIcon.sprite = SpriteRegistry.GetSprite(unit.name).GetSprite(false, false, false, false).sprite;

            this.onToggle = onToggle;
        }

        public void Toggle(bool value)
        {
            onToggle?.Invoke(value);
        }
    }
}
