using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AurumGames.Animation.Editor
{
    public class EasingDrawer
    {
        private static readonly string[] _easingFunctions = Easing.Functions.Keys.ToArray();
        private static readonly string[] _easingFunctionsWithDefault = new [] {"Default"}.Union(Easing.Functions.Keys).ToArray();
        
        public static string Draw(Rect position, SerializedProperty property, bool withDefault = true)
        {
            var functions = withDefault ? _easingFunctionsWithDefault : _easingFunctions;
            
            var selectedEasingIndex = Array.IndexOf(functions, property.stringValue);
            if (selectedEasingIndex < 0 || selectedEasingIndex >= functions.Length)
                selectedEasingIndex = 0;
            
            selectedEasingIndex = EditorGUI.Popup(new Rect(position.position, new Vector2(position.width, 20)), "Easing", selectedEasingIndex, _easingFunctions);
            return functions[selectedEasingIndex];
        }
        
        public static string DrawWithoutLabel(Rect position, SerializedProperty property, bool withDefault = true)
        {
            var functions = withDefault ? _easingFunctionsWithDefault : _easingFunctions;
            
            var selectedEasingIndex = Array.IndexOf(functions, property.stringValue);
            if (selectedEasingIndex < 0 || selectedEasingIndex >= functions.Length)
                selectedEasingIndex = 0;
            
            selectedEasingIndex = EditorGUI.Popup(position, selectedEasingIndex, functions);
            return functions[selectedEasingIndex];
        }
    }
}