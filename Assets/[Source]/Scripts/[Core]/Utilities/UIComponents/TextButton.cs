
namespace Core.Utilities.UIComponents
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using System.Collections;
    using System.Collections.Generic;
    
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UltEvents;
    
    public class TextButton : Button
    {
        public Text text;

        private void Reset()
        {
            text = text ? text : GetComponentInChildren<Text>();
        }
    }
}