
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
    //using UltEvents;
    using TMPro;
    
    public class TextMPButton : Button
    {
        public TMP_Text text;

        private void Reset()
        {
            text = text ? text : GetComponentInChildren<TMP_Text>();
        }
    }
}