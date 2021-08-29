using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Tanttinator.ModularUI;
using System;

namespace Client
{
    public class PromptUI : MonoBehaviour
    {
        [SerializeField] TMP_Text text = default;
        [SerializeField] Hidable hidable = default;

        static Action callback;

        static PromptUI instance;

        public static void Show(string text, Color color, Action callback = null)
        {
            instance.text.text = text;
            instance.text.color = color;
            instance.hidable.Show();

            PromptUI.callback = callback;
        }

        public void Confirm()
        {
            callback?.Invoke();
            hidable.Hide();
        }

        private void Awake()
        {
            instance = this;
        }
    }
}
