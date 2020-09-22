using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Client
{
    public class ActionButtonUI : MonoBehaviour
    {
        [SerializeField] TMP_Text text = default;

        ButtonAction action;

        public void Setup(ButtonAction action)
        {
            text.text = action.name;
            this.action = action;
        }

        public void FireAction()
        {
            action?.FireAction();
        }
    }
}
