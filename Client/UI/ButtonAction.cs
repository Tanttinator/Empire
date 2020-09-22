using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Client
{
    public class ButtonAction
    {
        public string name { get; protected set; }
        Action action;

        public ButtonAction(string name, Action action)
        {
            this.name = name;
            this.action = action;
        }

        public void FireAction()
        {
            action?.Invoke();
        }
    }
}
