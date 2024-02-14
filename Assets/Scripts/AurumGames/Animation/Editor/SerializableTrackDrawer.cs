using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AurumGames.Animation.Tracks;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AurumGames.Animation.Editor
{
    [CustomPropertyDrawer(typeof(SerializableTrackInfo))]
    public class SerializableTrackDrawer : PropertyDrawer
    {
        private static string[] _tracks = SerializableAnimationConverter.ConversionMap.Keys.ToArray();

        private int _selectedTrackType;
        private int _value;
        private Dictionary<string, State> _states = new();
        
        private class State
        {
            public TrackConversionInfo ConversionInfo;
            public ReorderableList ReorderableList;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect R()
            {
                return new Rect(position.position, new Vector2(position.width, 20));
            }
            
            var startY = position.y;
            
            SerializedProperty trackTypeProperty = property.FindPropertyRelative(nameof(SerializableTrackInfo.TrackType));
            SerializedProperty easingProperty = property.FindPropertyRelative(nameof(SerializableTrackInfo.Easing));
            SerializedProperty targetProperty = property.FindPropertyRelative(nameof(SerializableTrackInfo.Target));
            SerializedProperty framesProperty = property.FindPropertyRelative(nameof(SerializableTrackInfo.Frames));

            _selectedTrackType = Array.IndexOf(_tracks, trackTypeProperty.stringValue);
            if (_selectedTrackType == -1) 
                _selectedTrackType = 0;
            
            EditorGUI.BeginChangeCheck();
            var tempBool = EditorGUI.Foldout(R(), trackTypeProperty.isExpanded, _tracks[_selectedTrackType]);
            if (EditorGUI.EndChangeCheck())
                trackTypeProperty.isExpanded = tempBool;
            position.y += 2;
            _selectedTrackType = EditorGUI.Popup(R(), _selectedTrackType,
                _tracks);

            var trackName = _tracks[_selectedTrackType];
            if (trackName != trackTypeProperty.stringValue)
                trackTypeProperty.stringValue = trackName;
            TrackConversionInfo conversionInfo = SerializableAnimationConverter.ConversionMap[trackName];
            
            if (trackTypeProperty.isExpanded)
            {
                position.y += 25;
                if (conversionInfo.TrackType == typeof(VoidTrack))
                {
                    if (framesProperty.arraySize == 0)
                        framesProperty.InsertArrayElementAtIndex(0);
                    SerializedProperty firstFrameProperty = framesProperty.GetArrayElementAtIndex(0);
                    
                    SerializedProperty firstFrameValueProperty = firstFrameProperty.FindPropertyRelative(nameof(SerializableFrameInfo.Value));
                    var frameValue = firstFrameValueProperty.managedReferenceValue;
                    var tempInt = 0;
                    if (frameValue is not int)
                        frameValue = 1;
                    
                    EditorGUI.BeginChangeCheck();
                    tempInt = EditorGUI.IntField(R(), "Duration", (int)frameValue);
                    if (tempInt < 1)
                        tempInt = 1;
                    
                    if (EditorGUI.EndChangeCheck() || firstFrameValueProperty.managedReferenceValue is not int)
                        firstFrameValueProperty.managedReferenceValue = tempInt;
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    var tempString = EasingDrawer.Draw(position, easingProperty, false);
                    if (EditorGUI.EndChangeCheck())
                        easingProperty.stringValue = tempString;
                    position.y += 25;
                    
                    EditorGUI.BeginChangeCheck();
                    Object tempObject = EditorGUI.ObjectField(R(), "Target", targetProperty.objectReferenceValue, conversionInfo.TargetType, true);
                    if (EditorGUI.EndChangeCheck())
                        targetProperty.objectReferenceValue = tempObject;
                    if (conversionInfo.TargetType.IsAssignableFrom(targetProperty.objectReferenceValue?.GetType()) == false)
                        targetProperty.objectReferenceValue = null;
                    position.y += 25;
                    
                    while (framesProperty.arraySize < 2)
                    {
                        framesProperty.InsertArrayElementAtIndex(0);
                    }
                    ReorderableList reorderableList = GetList(conversionInfo, framesProperty);
                    reorderableList.DoList(position);
                    position.y += reorderableList.GetHeight();
                }
                
            }

            property.serializedObject.ApplyModifiedProperties();
        }

        private ReorderableList GetList(TrackConversionInfo trackConversionInfo, SerializedProperty framesProperty)
        {
            State state;
            if (_states.TryGetValue(framesProperty.propertyPath, out state))
            {
                if (state.ReorderableList != null &&
                    state.ConversionInfo == trackConversionInfo && 
                    state.ReorderableList.serializedProperty.serializedObject == framesProperty.serializedObject)
                    return state.ReorderableList;
            }

            _states.TryAdd(framesProperty.propertyPath, new State());
            state = _states[framesProperty.propertyPath];
            state.ConversionInfo = trackConversionInfo;
            
            var list = state.ReorderableList = new ReorderableList(framesProperty.serializedObject, framesProperty, true, true, true, true);
            list.elementHeight = EditorGUIUtility.singleLineHeight + 5;
            list.drawHeaderCallback = rect =>
            {
                var newRect = new Rect(rect.x, rect.y, rect.width, rect.height);
                EditorGUI.LabelField(newRect, "Frames");
            };

            list.multiSelect = true;
            list.draggable = true;
            
            list.elementHeightCallback = (int indexer) => 
            {
                return list.elementHeight;
            };
            
            list.drawElementCallback = (Rect position, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty frameProperty = framesProperty.GetArrayElementAtIndex(index);
                SerializedProperty frameValueProperty =
                    frameProperty.FindPropertyRelative(nameof(SerializableFrameInfo.Value));
                SerializedProperty frameEasingProperty =
                    frameProperty.FindPropertyRelative(nameof(SerializableFrameInfo.Easing));
                SerializedProperty frameTimeProperty = frameProperty.FindPropertyRelative(nameof(SerializableFrameInfo.Time));

                EditorGUI.BeginChangeCheck();
                var tempInt = EditorGUI.IntField(new Rect(position.x, position.y + 2, 40, position.height - 5),
                    frameTimeProperty.intValue);
                if (EditorGUI.EndChangeCheck())
                    frameTimeProperty.intValue = tempInt;

                var valuePosition = new Rect(position.x + 45, position.y + 2, position.width - 45 - 85, position.height - 5);
                EditorGUI.BeginChangeCheck();
                var valueTemp = frameValueProperty.managedReferenceValue;
                if (trackConversionInfo.KeyframeType == typeof(int))
                {
                    if (valueTemp is not int)
                        valueTemp = 0;

                    valueTemp = EditorGUI.IntField(valuePosition, (int)valueTemp);
                }
                else if (trackConversionInfo.KeyframeType == typeof(float))
                {
                    if (valueTemp is not float)
                        valueTemp = 0.0f;

                    valueTemp = EditorGUI.FloatField(valuePosition, (float)valueTemp);
                }
                else if (trackConversionInfo.KeyframeType == typeof(Color))
                {
                    if (valueTemp is not Color)
                        valueTemp = Color.white;

                    valueTemp = EditorGUI.ColorField(valuePosition, (Color)valueTemp);
                }
                else if (trackConversionInfo.KeyframeType == typeof(Vector2))
                {
                    if (valueTemp is not Vector2)
                        valueTemp = Vector2.zero;

                    valueTemp = EditorGUI.Vector2Field(valuePosition, "", (Vector2)valueTemp);
                }
                else if (trackConversionInfo.KeyframeType == typeof(Vector3))
                {
                    if (valueTemp is not Vector3)
                        valueTemp = Vector3.zero;

                    valueTemp = EditorGUI.Vector3Field(valuePosition, "", (Vector3)valueTemp);
                }
                else if (trackConversionInfo.KeyframeType == typeof(Quaternion))
                {
                    if (valueTemp is not Quaternion)
                        valueTemp = Quaternion.identity;

                    valueTemp = Quaternion.Euler(EditorGUI.Vector3Field(valuePosition, "", ((Quaternion)valueTemp).eulerAngles));
                }

                if (EditorGUI.EndChangeCheck() || 
                    trackConversionInfo.KeyframeType.IsAssignableFrom(frameValueProperty.managedReferenceValue?.GetType()) == false)
                    frameValueProperty.managedReferenceValue = valueTemp;

                EditorGUI.BeginChangeCheck();
                var tempString = EasingDrawer.DrawWithoutLabel(
                    new Rect(valuePosition.x + valuePosition.width + 5, position.y + 2, 80, position.height - 5),
                    frameEasingProperty);
                if (EditorGUI.EndChangeCheck())
                    frameEasingProperty.stringValue = tempString;
            };

            return list;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = 20;
            
            SerializedProperty trackTypeProperty = property.FindPropertyRelative(nameof(SerializableTrackInfo.TrackType));
            SerializedProperty framesProperty = property.FindPropertyRelative(nameof(SerializableTrackInfo.Frames));

            _selectedTrackType = Array.IndexOf(_tracks, trackTypeProperty.stringValue);
            if (_selectedTrackType == -1) 
                _selectedTrackType = 0;
            
            height += 2;

            var trackName = _tracks[_selectedTrackType];
            TrackConversionInfo conversionInfo = SerializableAnimationConverter.ConversionMap[trackName];
            
            if (trackTypeProperty.isExpanded)
            {
                height += 25;
                if (conversionInfo.TrackType == typeof(VoidTrack))
                {
                    
                }
                else
                {
                    height += 25;
                    height += 25;
                    
                    ReorderableList reorderableList = GetList(conversionInfo, framesProperty);
                    height += (int)reorderableList.GetHeight();
                }
                
            }

            
            return height;
        }
    }
}