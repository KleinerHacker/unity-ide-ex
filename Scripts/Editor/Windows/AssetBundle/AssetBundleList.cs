using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorEx.Editor.editor_ex.Scripts.Editor.Utils;
using UnityEditorInternal;
using UnityEngine;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Windows.AssetBundle
{
    internal sealed class AssetBundleList : ReorderableList
    {
        private readonly string _bundle;

        public AssetBundleList(IList<Object> elements, string bundle) : base(elements.ToList(), typeof(Object), false, true, true, true)
        {
            _bundle = bundle;

            drawHeaderCallback += DrawHeaderCallback;
            drawElementCallback += DrawElementCallback;
            onRemoveCallback += OnRemoveCallback;
            onCanRemoveCallback += OnCanRemoveCallback;
            onAddCallback += OnAddCallback;
            onSelectCallback += OnSelectCallback;
        }

        private void OnSelectCallback(ReorderableList reorderableList)
        {
            Selection.activeObject = (Object) list[index];
        }

        private void OnAddCallback(ReorderableList reorderableList)
        {
            EditorGUIUtility.ShowObjectPicker<Object>(null, false, null, 0);
            var o = EditorGUIUtility.GetObjectPickerObject();
            
            if (AssetBundleUtility.HasBundle(o) && 
                !EditorUtility.DisplayDialog("Double Bundle", "The asset '" + o.name + "' is already in another bundle. Continue?", "Yes", "No"))
                return;

            var assetPath = AssetDatabase.GetAssetPath(o);
            AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant(_bundle, "");

            list.Add(o);
        }

        private bool OnCanRemoveCallback(ReorderableList reorderableList) => index >= 0 && index < list.Count;

        private void OnRemoveCallback(ReorderableList reorderableList)
        {
            var o = (Object)list[index];
            var assetPath = AssetDatabase.GetAssetPath(o);
            AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant("", "");

            list.RemoveAt(index);
        }

        private void DrawHeaderCallback(Rect rect)
        {
            GUI.Label(rect, "Asset Bundle Content Files");
        }

        private void DrawElementCallback(Rect rect, int i, bool isactive, bool isfocused)
        {
            var o = (Object)list[i];
            var assetPath = AssetDatabase.GetAssetPath(o);

            var icon = AssetDatabase.GetCachedIcon(assetPath);
            GUI.DrawTexture(new Rect(rect.x, rect.y + 4f, 16f, 16f), icon);
            GUI.Label(new Rect(rect.x + 25f, rect.y, rect.width - 25f, rect.height), o.name);
        }
    }
}