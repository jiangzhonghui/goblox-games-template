using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace OctoberStudio.StageCreator
{
    public static class SerializedPropertyExtensions
    {
        #region Get serialized property value
        public static T GetValue<T>(this SerializedProperty property) where T : class
        {
            object obj = property.serializedObject.targetObject;
            var elements = property.propertyPath.Replace(".Array.data[", "[").Split('.');

            foreach (string element in elements)
            {
                if (element.Contains("["))
                {
                    var fieldName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetFieldValue(obj, fieldName, index);
                }
                else
                {
                    obj = GetFieldValue(obj, element);
                }

                if (obj == null)
                    return null;
            }

            return obj as T;
        }

        private static object GetFieldValue(object source, string fieldName, int index = -1)
        {
            if (source == null)
                return null;

            Type type = source.GetType();
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field == null)
                return null;

            object value = field.GetValue(source);

            if (index >= 0 && value is System.Collections.IList list)
                return list[index];

            return value;
        }

        #endregion

        #region Set serialized property value

        public static void SetValue(this SerializedProperty property, object value)
        {
            object targetObject = property.serializedObject.targetObject;
            object parent = GetParentObjectOfProperty(property.propertyPath, targetObject);

            string fieldName = GetFieldName(property.propertyPath);
            if (parent == null)
            {
                Debug.LogError("Parent object is null.");
                return;
            }

            if (IsArrayElement(fieldName, out string arrayFieldName, out int index))
            {
                FieldInfo field = parent.GetType().GetField(arrayFieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    var list = field.GetValue(parent) as IList;
                    if (list != null && index >= 0 && index < list.Count)
                    {
                        list[index] = value;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        Debug.LogError("List is null or index out of range.");
                    }
                }
                else
                {
                    Debug.LogError($"Field '{arrayFieldName}' not found on {parent.GetType()}");
                }
            }
            else
            {
                FieldInfo field = parent.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(parent, value);
                    property.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    Debug.LogError($"Field '{fieldName}' not found on {parent.GetType()}");
                }
            }
        }

        private static bool IsArrayElement(string fieldName, out string arrayName, out int index)
        {
            arrayName = null;
            index = -1;

            var match = System.Text.RegularExpressions.Regex.Match(fieldName, @"^(.+)\[(\d+)\]$");
            if (match.Success)
            {
                arrayName = match.Groups[1].Value;
                index = int.Parse(match.Groups[2].Value);
                return true;
            }
            return false;
        }

        public static object GetParentObjectOfProperty(string path, object root)
        {
            string[] elements = path.Split('.');
            object obj = root;

            foreach (var element in elements[..^1]) // everything except the last
            {
                if (element.Contains("["))
                {
                    // array element
                    string fieldName = element.Substring(0, element.IndexOf('['));
                    int index = int.Parse(element.Substring(element.IndexOf('[') + 1).TrimEnd(']'));

                    FieldInfo field = obj.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var list = field?.GetValue(obj) as IList;
                    obj = list?[index];
                }
                else
                {
                    FieldInfo field = obj.GetType().GetField(element, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    obj = field?.GetValue(obj);
                }

                if (obj == null)
                    break;
            }

            return obj;
        }

        public static string GetFieldName(string path)
        {
            string[] parts = path.Split('.');
            return parts[^1]; // last part
        }

        #endregion

        public static void CopyFromSerializedObject(this SerializedObject target, SerializedObject source)
        {
            var sourceProp = source.GetIterator();
            var targetProp = target.GetIterator();

            var enterChildren = true;
            while (sourceProp.NextVisible(enterChildren) && targetProp.NextVisible(enterChildren))
            {
                // Skip the script reference field
                if (sourceProp.name == "m_Script")
                {
                    enterChildren = false;
                    continue;
                }

                targetProp.serializedObject.CopyFromSerializedProperty(sourceProp);
                enterChildren = false;
            }

            target.ApplyModifiedProperties();
        }

        private static object[] m_ParametersExpand = new object[] { null, true };
        private static System.Type m_SceneHierarchyWindowType = null;
        private static System.Type SceneHierarchyWindowType
        {
            get
            {
                if (m_SceneHierarchyWindowType == null)
                {
                    var assembly = typeof(EditorWindow).Assembly;
                    m_SceneHierarchyWindowType = assembly.GetType("UnityEditor.SceneHierarchyWindow");
                }
                return m_SceneHierarchyWindowType;
            }
        }
        private static MethodInfo m_SetExpandedRecursive = null;
        private static MethodInfo SetExpandedRecursiveImpl
        {
            get
            {
                if (m_SetExpandedRecursive == null)
                    m_SetExpandedRecursive = m_SceneHierarchyWindowType.GetMethod("SetExpandedRecursive");
                return m_SetExpandedRecursive;
            }
        }
        private static void SetExpandedRecursive(int aInstanceID, bool aExpand)
        {
            var hierachyWindow = EditorWindow.GetWindow(SceneHierarchyWindowType);
            if (aExpand)
            {
                m_ParametersExpand[0] = aInstanceID;
                SetExpandedRecursiveImpl.Invoke(hierachyWindow, m_ParametersExpand);
            }
        }

        public static void Expand(this GameObject gameObject)
        {
            SetExpandedRecursive(gameObject.GetInstanceID(), true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect Shrink(this Rect rect, float amount)
        {
            return new Rect(rect.x + amount, rect.y + amount, rect.width - 2 * amount, rect.height - 2 * amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect Shrink(this Rect rect, float amountX, float amountY)
        {
            return new Rect(rect.x + amountX, rect.y + amountY, rect.width - 2 * amountX, rect.height - 2 * amountY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect Grow(this Rect rect, float amount)
        {
            return new Rect(rect.x - amount, rect.y - amount, rect.width + 2 * amount, rect.height + 2 * amount);
        }

        public static void DrawVerticalSeparator(float width = 2f)
        {
            EditorGUI.DrawRect(GUILayoutUtility.GetRect(width, 0, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(false)), new Color(0.1f, 0.1f, 0.1f));
        }

        public static void DrawHorizontalSeparator(float height = 2f)
        {
            EditorGUI.DrawRect(GUILayoutUtility.GetRect(0, height, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false)), new Color(0.1f, 0.1f, 0.1f));
        }

        public static void DrawVerticalSeparator(float width, Color color)
        {
            EditorGUI.DrawRect(GUILayoutUtility.GetRect(width, 0, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(false)), color);
        }

        public static void Set(this Transform transform, TransformData data)
        {
            if (data == null) return;
            transform.position = data.Position;
            transform.rotation = data.Rotation;
            transform.localScale = data.LocalScale;
        }
    }
}