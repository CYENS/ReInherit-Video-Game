using Cyens.ReInherit.Architect;
using UnityEditor;
using UnityEngine;

namespace Cyens.ReInherit.Source.Editor
{
    [CustomPropertyDrawer(typeof(DirectionList<>))]
    public class DirectionListDrawer : PropertyDrawer
    {
        private static readonly float VerticalSpacing = EditorGUIUtility.standardVerticalSpacing;
        private static readonly float FieldHeight = EditorGUIUtility.singleLineHeight;
        private static readonly float TotalFieldHeight = FieldHeight + VerticalSpacing;

        private const int LineCount = Direction.Length + 1;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return LineCount * TotalFieldHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var labelRect = new Rect(position.x, position.y, position.width, TotalFieldHeight);
            EditorGUI.LabelField(labelRect, label);

            ++EditorGUI.indentLevel;
            
            var dataProperty = property.FindPropertyRelative("data");
            var rect = new Rect(position.x, position.y + TotalFieldHeight, position.width, FieldHeight);

            if (dataProperty.arraySize != Direction.Length) {
                // This can happen if the DirectionList had not been properly serialized
                dataProperty.ClearArray();
                dataProperty.arraySize = Direction.Length;
                property.serializedObject.ApplyModifiedProperties();
            }

            for (var i = 0; i < Direction.Length; ++i) {
                var name = Direction.Names[i];
                EditorGUI.PropertyField(rect, dataProperty.GetArrayElementAtIndex(i), new GUIContent(name));
                rect.y += TotalFieldHeight;
            }

            --EditorGUI.indentLevel;

            EditorGUI.EndProperty();
        }
    }
}