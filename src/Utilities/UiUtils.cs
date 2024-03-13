using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine;
using UniverseLib.UI;
using UnityEngine.UI;

namespace MalumMenu.Utilities
{
    public static class UiUtils
    {
        public static Toggle CreateToggle(GameObject parent, string name, string text, UnityAction<bool> action, bool defaultValue = false, Color bgColor = default, int checkWidth = 20, int checkHeight = 20)
        {
            GameObject toggleObj = UIFactory.CreateToggle(parent, name, out Toggle toggle, out Text toggleText, bgColor, checkWidth, checkHeight);
            toggle.isOn = defaultValue;
            toggle.onValueChanged.AddListener(action);
            toggleText.text = text;
            return toggle;
        }
    }
}
