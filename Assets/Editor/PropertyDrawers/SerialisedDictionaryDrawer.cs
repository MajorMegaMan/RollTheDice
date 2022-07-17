using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Serialisable.Drawable;

[CustomPropertyDrawer(typeof(DrawableDictionaryHelper), true)]
public class SerialisedDictionaryDrawer : PropertyDrawer
{
    const string KEY_MEMBER_NAME = "key";
    const string VALUE_MEMBER_NAME = "value";

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
        {
            var keyValuesProp = property.FindPropertyRelative("m_keyValues");

            int count = keyValuesProp.arraySize;
            float height = 0.0f;
            for (int i = 0; i < count; i++)
            {
                var valueProp = keyValuesProp.GetArrayElementAtIndex(i).FindPropertyRelative(VALUE_MEMBER_NAME);
                float valueHeight = EditorGUI.GetPropertyHeight(valueProp);

                float propHeight = EditorGUIUtility.singleLineHeight + 1f;
                if (valueHeight > EditorGUIUtility.singleLineHeight)
                {
                    propHeight = valueHeight + 1.0f;
                }
                height += propHeight;
            }

            // Make room for labels and foldout
            height += 2 * (EditorGUIUtility.singleLineHeight + 1f);

            return height;
        }
        else
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        bool expanded = property.isExpanded;
        Rect rect = GetNextRect(ref position);
        property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);

        if (expanded)
        {
            EditorGUI.BeginChangeCheck();

            int lvl = EditorGUI.indentLevel;
            EditorGUI.indentLevel = lvl + 1;

            var keyValuesProp = property.FindPropertyRelative("m_keyValues");

            rect = GetNextRect(ref position);
            DrawControls(rect, keyValuesProp);
            DrawHeaders(rect);

            int cnt = keyValuesProp.arraySize;

            for (int i = 0; i < cnt; i++)
            {
                var keyProp = keyValuesProp.GetArrayElementAtIndex(i).FindPropertyRelative(KEY_MEMBER_NAME);
                var valueProp = keyValuesProp.GetArrayElementAtIndex(i).FindPropertyRelative(VALUE_MEMBER_NAME);
                float valueHeight = EditorGUI.GetPropertyHeight(valueProp);

                //r = GetNextRect(ref position);
                rect = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
                float h = EditorGUIUtility.singleLineHeight + 1f;
                float positionY = position.yMin + h;
                if (valueHeight > EditorGUIUtility.singleLineHeight)
                {
                    positionY = position.yMin + valueHeight + 1.0f;
                }
                position = new Rect(position.xMin, positionY, position.width, position.height = h);

                //r = EditorGUI.IndentedRect(r);
                float w0 = EditorGUIUtility.labelWidth; // r.width / 2f;
                float w1 = rect.width - w0;

                Rect r0 = new Rect(rect.xMin, rect.yMin, w0, rect.height);
                Rect r1 = new Rect(r0.xMax, rect.yMin, w1, valueHeight);

                EditorGUI.PropertyField(r0, keyProp, GUIContent.none, true);
                EditorGUI.PropertyField(r1, valueProp, GUIContent.none, true);

                
            }

            EditorGUI.indentLevel = lvl;

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
        }
    }

    private Rect GetNextRect(ref Rect position)
    {
        Rect rect = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
        float height = EditorGUIUtility.singleLineHeight + 1f;
        position = new Rect(position.xMin, position.yMin + height, position.width, position.height = height);
        return rect;
    }

    void DrawHeaders(Rect position)
    {
        position = EditorGUI.IndentedRect(position);
        float w0 = EditorGUIUtility.labelWidth; // r.width / 2f;
        float w1 = position.width - w0;

        Rect r0 = new Rect(position.xMin, position.yMin, w0, position.height);
        Rect r1 = new Rect(r0.xMax, position.yMin, w1, position.height);

        GUI.Label(r0, "Key");
        GUI.Label(r1, "Value");
    }

    void DrawControls(Rect position, SerializedProperty keyValuesProp)
    {
        var pRect = new Rect(position.xMax - 60f, position.yMin, 30f, EditorGUIUtility.singleLineHeight);
        var mRect = new Rect(position.xMax - 30f, position.yMin, 30f, EditorGUIUtility.singleLineHeight);

        if (GUI.Button(pRect, "+"))
        {
            AddElement(keyValuesProp);
        }
        if (GUI.Button(mRect, "-"))
        {
            keyValuesProp.arraySize = Mathf.Max(keyValuesProp.arraySize - 1, 0);
        }
    }

    public void AddElement(SerializedProperty keyValuesProp)
    {
        keyValuesProp.arraySize++;

        int count = keyValuesProp.arraySize;
        var newKeyProp = keyValuesProp.GetArrayElementAtIndex(count - 1).FindPropertyRelative(KEY_MEMBER_NAME);

        switch (newKeyProp.propertyType)
        {
            case SerializedPropertyType.Integer:
                {
                    int highest = int.MinValue;
                    for (int i = 0; i < count - 1; i++)
                    {
                        var keyProp = keyValuesProp.GetArrayElementAtIndex(i).FindPropertyRelative(KEY_MEMBER_NAME);
                        int keyPropValue = keyProp.intValue;
                        highest = Mathf.Max(highest, keyPropValue);
                    }
                    newKeyProp.intValue = highest + 1;
                }
                break;
            case SerializedPropertyType.Boolean:
                {
                    bool value = false;
                    for (int i = 0; i < count - 1; i++)
                    {
                        if(value)
                        {
                            // Reached limit
                            keyValuesProp.arraySize--;
                            Debug.LogWarning("Exceeded Key Limit");
                        }

                        var keyProp = keyValuesProp.GetArrayElementAtIndex(i).FindPropertyRelative(KEY_MEMBER_NAME);
                        if (value.Equals(keyProp.boolValue))
                        {
                            value = true;
                        }
                    }
                    newKeyProp.boolValue = value;
                }
                break;
            case SerializedPropertyType.Float:
                {
                    float highest = float.MinValue;
                    for (int i = 0; i < count - 1; i++)
                    {
                        var keyProp = keyValuesProp.GetArrayElementAtIndex(i).FindPropertyRelative(KEY_MEMBER_NAME);
                        float keyPropValue = keyProp.floatValue;
                        highest = Mathf.Max(highest, keyPropValue);
                    }
                    newKeyProp.floatValue = highest + 1;
                }
                break;
            case SerializedPropertyType.String:
                {
                    string toAdd = "New Key " + (count - 1).ToString();
                    newKeyProp.stringValue = toAdd;
                }
                break;
            case SerializedPropertyType.Color:
                {
                    int toAdd = 0;
                    for (int i = 0; i < count - 1; i++)
                    {
                        var keyProp = keyValuesProp.GetArrayElementAtIndex(i).FindPropertyRelative(KEY_MEMBER_NAME);
                        int keyPropValue = ToInt(keyProp.colorValue);

                        if(toAdd.Equals(keyPropValue))
                        {
                            toAdd++;
                            i = -1;
                        }
                    }
                    newKeyProp.colorValue = ToColour(toAdd);
                }
                break;
            case SerializedPropertyType.ObjectReference:
                {
                    newKeyProp.objectReferenceValue = null;
                }
                break;
            case SerializedPropertyType.LayerMask:
                {
                    newKeyProp.intValue = FindUniqueInt(keyValuesProp, -1);
                }
                break;
            case SerializedPropertyType.Enum:
                {
                    int value = 0;
                    int max = 0;
                    if (keyValuesProp.arraySize > 1)
                    {
                        var keyProp = keyValuesProp.GetArrayElementAtIndex(0).FindPropertyRelative(KEY_MEMBER_NAME);
                        max = keyProp.enumNames.Length - 1;
                    }

                    for (int i = 0; i < count - 1; i++)
                    {
                        var keyProp = keyValuesProp.GetArrayElementAtIndex(i).FindPropertyRelative(KEY_MEMBER_NAME);
                        if (keyProp.enumValueIndex.Equals(value))
                        {
                            value++;
                            if(value >= max)
                            {
                                value = max;
                                break;
                            }

                            i = -1;
                        }
                    }
                    newKeyProp.enumValueIndex = value;
                }
                break;
            case SerializedPropertyType.Vector2:
                {
                    Vector2 value = Vector2.zero;
                    newKeyProp.vector2Value = value;
                }
                break;
            case SerializedPropertyType.Vector3:
                {
                    Vector3 value = Vector3.zero;
                    newKeyProp.vector3Value = value;
                }
                break;
            case SerializedPropertyType.Vector4:
                {
                    Vector4 value = Vector4.zero;
                    newKeyProp.vector4Value = value;
                }
                break;
            case SerializedPropertyType.Rect:
                {
                    newKeyProp.rectValue = Rect.zero;
                }
                break;
            case SerializedPropertyType.ArraySize:
                {
                    int value = 0;
                    for (int i = 0; i < count - 1; i++)
                    {
                        var keyProp = keyValuesProp.GetArrayElementAtIndex(i).FindPropertyRelative(KEY_MEMBER_NAME);
                        if (keyProp.arraySize.Equals(value))
                        {
                            value++;
                            i = -1;
                        }
                    }
                    newKeyProp.arraySize = value;
                }
                break;
            case SerializedPropertyType.Character:
                {
                    newKeyProp.intValue = FindUniqueInt(keyValuesProp);
                }
                break;
            case SerializedPropertyType.AnimationCurve:
                {
                    newKeyProp.animationCurveValue = null;
                }
                break;
            case SerializedPropertyType.Bounds:
                {
                    newKeyProp.boundsValue = default(Bounds);
                }
                break;
            default:
                throw new System.InvalidOperationException("Key type is not supported.");
        }
    }

    int FindUniqueInt(SerializedProperty keyValuesProp, int startValue = 0)
    {
        int value = startValue;
        int count = keyValuesProp.arraySize;
        for (int i = 0; i < count - 1; i++)
        {
            var keyProp = keyValuesProp.GetArrayElementAtIndex(i).FindPropertyRelative(KEY_MEMBER_NAME);
            if (keyProp.intValue.Equals(value))
            {
                value++;
                i = -1;
            }
        }
        return value;
    }

    // Extra helpers for some of this code to work
    public static int ToInt(Color color)
    {
        return (Mathf.RoundToInt(color.a * 255) << 24) +
               (Mathf.RoundToInt(color.r * 255) << 16) +
               (Mathf.RoundToInt(color.g * 255) << 8) +
               Mathf.RoundToInt(color.b * 255);
    }

    public static Color ToColour(int value)
    {
        float a = (float)(value >> 24 & 0xFF) / 255f;
        float r = (float)(value >> 16 & 0xFF) / 255f;
        float g = (float)(value >> 8 & 0xFF) / 255f;
        float b = (float)(value & 0xFF) / 255f;
        return new Color(r, g, b, a);
    }
}
