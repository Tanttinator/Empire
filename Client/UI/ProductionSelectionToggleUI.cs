using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Client
{
    public class ProductionSelectionToggleUI : MonoBehaviour
    {

        [SerializeField] TMP_Text unitName = default;
        [SerializeField] Image background = default;
        [SerializeField] Image unitIcon = default;

        Action<bool> onToggle;

        public void Setup(string unitType, Color color, Action<bool> onToggle)
        {
            unitName.text = unitType;

            background.color = color;
            unitIcon.color = color;

            unitIcon.sprite = SpriteRegistry.GetSprite(unitType).GetSprite(false, false, false, false).sprite;

            this.onToggle = onToggle;
        }

        public void Toggle(bool value)
        {
            onToggle?.Invoke(value);
        }
    }
}
