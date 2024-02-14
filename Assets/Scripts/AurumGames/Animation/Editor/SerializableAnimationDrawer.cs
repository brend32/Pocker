using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using AurumGames.Animation.Tracks;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AurumGames.Animation.Editor
{
    [CustomPropertyDrawer(typeof(SerializableAnimation))]
    public class SerializableAnimationDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect R()
            {
                return new Rect(position.position, new Vector2(position.width, 20));
            }
            
            SerializedProperty timeScaleProperty = property.FindPropertyRelative(nameof(SerializableAnimation.TimeScale));
            SerializedProperty tracksProperty = property.FindPropertyRelative(nameof(SerializableAnimation.Tracks));

            property.isExpanded = EditorGUI.Foldout(R(), property.isExpanded, property.name);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                position.y += 20;
                EditorGUI.PropertyField(R(), timeScaleProperty);
                position.y += 25;
                EditorGUI.PropertyField(R(), tracksProperty);
                position.y += EditorGUI.GetPropertyHeight(tracksProperty);
                EditorGUI.indentLevel--;
                if (GUI.Button(R(), "Apply scale"))
                {
                    Undo.RecordObject(property.serializedObject.targetObject, "Apply scale");
                    var animation = property.GetValue<SerializableAnimation>();
                    animation = SerializableAnimationConverter.ApplyTimeScale(animation);
                    property.SetValue(animation);
                }
                position.y += 25;
                if (GUI.Button(R(), "To code"))
                {
                    var animation = property.GetValue<SerializableAnimation>();
                    var code = SerializableAnimationConverter.ConvertToCode(animation);
                    var textEditor = new TextEditor()
                    {
                        text = code
                    };
                    textEditor.SelectAll();
                    textEditor.Copy();
                    Debug.Log(code);
                }
            }

            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty timeScaleProperty = property.FindPropertyRelative(nameof(SerializableAnimation.TimeScale));
            SerializedProperty tracksProperty = property.FindPropertyRelative(nameof(SerializableAnimation.Tracks));

            if (property.isExpanded)
            {
                return EditorGUI.GetPropertyHeight(timeScaleProperty) + EditorGUI.GetPropertyHeight(tracksProperty) + 85;
            }
            else
            {
                return 20;
            }
        }
    }
    
    public static class SerializedPropertyExtensions
    {
        /// <summary>
        /// Get the object the serialized property holds by using reflection
        /// </summary>
        /// <typeparam name="T">The object type that the property contains</typeparam>
        /// <param name="property"></param>
        /// <returns>Returns the object type T if it is the type the property actually contains</returns>
        public static T GetValue<T>(this SerializedProperty property)
        {
            return GetNestedObject<T>(property.propertyPath, GetSerializedPropertyRootComponent(property));
        }

        /// <summary>
        /// Set the value of a field of the property with the type T
        /// </summary>
        /// <typeparam name="T">The type of the field that is set</typeparam>
        /// <param name="property">The serialized property that should be set</param>
        /// <param name="value">The new value for the specified property</param>
        /// <returns>Returns if the operation was successful or failed</returns>
        public static bool SetValue<T>(this SerializedProperty property, T value)
        {
            object obj = GetSerializedPropertyRootComponent(property);
            //Iterate to parent object of the value, necessary if it is a nested object
            string[] fieldStructure = property.propertyPath.Split('.');
            for (int i = 0; i < fieldStructure.Length - 1; i++)
            {
                var part = fieldStructure[i];
                if (part == "Array")
                {
                    const string pattern = @"data\[(.+)\]";
                    const RegexOptions options = RegexOptions.Singleline;

                    part = fieldStructure[++i];
                    Match match = Regex.Match(part, pattern, options);

                    obj = ((Array) obj).GetValue(int.Parse(match.Groups[1].Value));
                    continue;
                }
                obj = GetFieldOrPropertyValue<object>(fieldStructure[i], obj);
            }

            string fieldName = fieldStructure.Last();

            return SetFieldOrPropertyValue(fieldName, obj, value);
        }

        /// <summary>
        /// Get the component of a serialized property
        /// </summary>
        /// <param name="property">The property that is part of the component</param>
        /// <returns>The root component of the property</returns>
        public static Object GetSerializedPropertyRootComponent(SerializedProperty property)
        {
            return property.serializedObject.targetObject;
        }

        /// <summary>
        /// Iterates through objects to handle objects that are nested in the root object
        /// </summary>
        /// <typeparam name="T">The type of the nested object</typeparam>
        /// <param name="path">Path to the object through other properties e.g. PlayerInformation.Health</param>
        /// <param name="obj">The root object from which this path leads to the property</param>
        /// <param name="includeAllBases">Include base classes and interfaces as well</param>
        /// <returns>Returns the nested object casted to the type T</returns>
        public static T GetNestedObject<T>(string path, object obj, bool includeAllBases = true)
        {
            bool isArray = false;
            foreach (string part in path.Split('.'))
            {
                if (part == "Array")
                {
                    isArray = true;
                    continue;
                }

                if (isArray)
                {
                    const string pattern = @"data\[(.+)\]";
                    const RegexOptions options = RegexOptions.Singleline;
        
                    Match match = Regex.Match(part, pattern, options);

                    obj = ((Array) obj).GetValue(int.Parse(match.Groups[1].Value));
                    isArray = false;
                    continue;
                }
                
                obj = GetFieldOrPropertyValue<object>(part, obj, includeAllBases);
            }

            return (T) obj;
        }

        public static T GetFieldOrPropertyValue<T>(string fieldName, object obj, bool includeAllBases = false,
            BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                    BindingFlags.NonPublic)
        {
            FieldInfo field = obj.GetType().GetField(fieldName, bindings);
            if (field != null) return (T) field.GetValue(obj);

            PropertyInfo property = obj.GetType().GetProperty(fieldName, bindings);
            if (property != null) return (T) property.GetValue(obj, null);

            if (includeAllBases)
            {
                foreach (Type type in GetBaseClassesAndInterfaces(obj.GetType()))
                {
                    field = type.GetField(fieldName, bindings);
                    if (field != null) return (T) field.GetValue(obj);

                    property = type.GetProperty(fieldName, bindings);
                    if (property != null) return (T) property.GetValue(obj, null);
                }
            }

            return default(T);
        }

        public static bool SetFieldOrPropertyValue(string fieldName, object obj, object value,
            bool includeAllBases = false,
            BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                    BindingFlags.NonPublic)
        {
            FieldInfo field = obj.GetType().GetField(fieldName, bindings);
            if (field != null)
            {
                field.SetValue(obj, value);
                return true;
            }

            PropertyInfo property = obj.GetType().GetProperty(fieldName, bindings);
            if (property != null)
            {
                property.SetValue(obj, value, null);
                return true;
            }

            if (includeAllBases)
            {
                foreach (Type type in GetBaseClassesAndInterfaces(obj.GetType()))
                {
                    field = type.GetField(fieldName, bindings);
                    if (field != null)
                    {
                        field.SetValue(obj, value);
                        return true;
                    }

                    property = type.GetProperty(fieldName, bindings);
                    if (property != null)
                    {
                        property.SetValue(obj, value, null);
                        return true;
                    }
                }
            }

            return false;
        }

        public static IEnumerable<Type> GetBaseClassesAndInterfaces(this Type type, bool includeSelf = false)
        {
            List<Type> allTypes = new List<Type>();

            if (includeSelf) allTypes.Add(type);

            if (type.BaseType == typeof(object))
            {
                allTypes.AddRange(type.GetInterfaces());
            }
            else
            {
                allTypes.AddRange(
                    Enumerable
                        .Repeat(type.BaseType, 1)
                        .Concat(type.GetInterfaces())
                        .Concat(type.BaseType.GetBaseClassesAndInterfaces())
                        .Distinct());
            }

            return allTypes;
        }
    }
}