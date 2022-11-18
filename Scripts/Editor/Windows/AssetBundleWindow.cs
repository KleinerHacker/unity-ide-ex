using System.Linq;
using UnityCommonEx.Runtime.common_ex.Scripts.Runtime.Utils.Extensions;
using UnityEditor;
using UnityEditorEx.Editor.editor_ex.Scripts.Editor.Utils;
using UnityEngine;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Assets;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Windows.AssetBundle;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Windows
{
    public sealed class AssetBundleWindow : EditorWindow
    {
        [MenuItem("Window/General/IDE Extensions/Asset Bundles")]
        public static void ShowWindow()
        {
            var window = CreateInstance<AssetBundleWindow>();
            window.Show();
        }

        private AssetBundleList _assetList;

        private AssetBundleItem[] _bundles;
        private int _selectedBundle;

        private SerializedObject _serializedObject;
        private SerializedProperty _itemsProperty;

        private void OnEnable()
        {
            _serializedObject = AssetBundleSettings.SerializedSingleton;
            _itemsProperty = _serializedObject.FindProperty("items");

            titleContent = new GUIContent("Asset Bundles", EditorGUIUtility.IconContent("d_Profiler.NetworkOperations").image);
        }

        private void OnFocus()
        {
            RebuildBundles();
            RebuildList(); 
        }

        private void OnGUI()
        {
            _serializedObject.Update();
            var assetBundleSettings = (AssetBundleSettings)_serializedObject.targetObject;

            EditorGUILayout.BeginHorizontal();
            {
                var newSelection = EditorGUILayout.Popup(GUIContent.none, _selectedBundle, _bundles.Select(x => x.AssetBundleName).ToArray());
                if (_selectedBundle != newSelection)
                {
                    _selectedBundle = newSelection;
                    RebuildList();
                }

                if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus").image, new GUIStyle(EditorStyles.iconButton) { contentOffset = new Vector2(0f, 3f) }))
                {
                    var name = EditorUtilityEx.DisplayInputDialog("Add Asset Bundle", "Asset Bundle Name:", "OK", "Cancel");
                    if (string.IsNullOrEmpty(name))
                        return;

                    Debug.Log("[ASSET BUNDLE] Add new asset bundle " + name);
                    assetBundleSettings.Items = assetBundleSettings.Items.Append(new AssetBundleItem { AssetBundleName = name }).ToArray();
                    AssetBundleUtility.CreateAssetBundle(name);

                    RebuildBundles();
                    RebuildList();

                    return;
                }

                EditorGUI.BeginDisabledGroup(_selectedBundle < 0 || _selectedBundle >= _bundles.Length);
                if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus").image, new GUIStyle(EditorStyles.iconButton) { contentOffset = new Vector2(0f, 3f) }))
                {
                    if (AssetDatabase.GetUnusedAssetBundleNames().Contains(_bundles[_selectedBundle].AssetBundleName) ||
                        EditorUtility.DisplayDialog("Delete Asset Bundle", "You are sure to delete this Asset Bundle? It is in use and will be removed!", "Yes", "No"))
                    {
                        Debug.Log("[ASSET BUNDLE] Remove asset bundle " + _bundles[_selectedBundle].AssetBundleName);
                        if (!AssetDatabase.RemoveAssetBundleName(_bundles[_selectedBundle].AssetBundleName, true))
                        {
                            EditorUtility.DisplayDialog("Failed to delete Asset Bundle", "The asset bundle cannot deleted!", "OK");
                        }
                        else
                        {
                            assetBundleSettings.Items = assetBundleSettings.Items.Remove(_bundles[_selectedBundle]).ToArray();

                            RebuildBundles();

                            return;
                        }
                    }
                }

                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();

            if (_selectedBundle >= 0 && _selectedBundle < _bundles.Length)
            {
                EditorGUILayout.Space();
                _bundles[_selectedBundle].BuildAssetBundle = EditorGUILayout.Toggle("Build in pipeline", _bundles[_selectedBundle].BuildAssetBundle);
                _bundles[_selectedBundle].BuildSubPath = EditorGUILayout.TextField("Sub path in binary folder", _bundles[_selectedBundle].BuildSubPath);
                _bundles[_selectedBundle].Options = (BuildAssetBundleOptions) EditorGUILayout.EnumFlagsField("Build Options", _bundles[_selectedBundle].Options);
                EditorUtility.SetDirty(assetBundleSettings);
                EditorGUILayout.Space();

                if (Event.current.type == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    Event.current.Use();
                }


                if (Event.current.type == EventType.DragPerform)
                {
                    foreach (var reference in DragAndDrop.objectReferences)
                    {
                        if (AssetBundleUtility.HasBundle(reference) &&
                            !EditorUtility.DisplayDialog("Double Bundle", "The asset '" + reference.name + "' is already in another bundle. Continue?", "Yes", "No"))
                            return;

                        var assetPath = AssetDatabase.GetAssetPath(reference);
                        AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant(_bundles[_selectedBundle].AssetBundleName, "");
                    }

                    RebuildList();
                    Event.current.Use();
                }

                _assetList?.DoLayoutList();
            }

            _serializedObject.ApplyModifiedProperties();
        }

        private void RebuildList()
        {
            _assetList = null;

            if (_selectedBundle < 0 || _selectedBundle >= _bundles.Length)
                return;

            var assets = AssetDatabase.GetAssetPathsFromAssetBundle(_bundles[_selectedBundle].AssetBundleName)
                .SelectMany(AssetDatabase.LoadAllAssetsAtPath)
                .ToList();
            _assetList = new AssetBundleList(assets, _bundles[_selectedBundle].AssetBundleName);
        }

        private void RebuildBundles()
        {
            var assetBundleSettings = (AssetBundleSettings)_serializedObject.targetObject;
            
            var addList = AssetDatabase.GetAllAssetBundleNames().Where(x => !AssetBundleSettings.Singleton.Items.Any(y => string.Equals(x, y.AssetBundleName)));
            var removeList = assetBundleSettings.Items.Where(x => !AssetDatabase.GetAllAssetBundleNames().Contains(x.AssetBundleName));

            AssetBundleSettings.Singleton.Items = assetBundleSettings.Items
                .RemoveAll(removeList.ToArray())
                .Concat(addList.Select(x => new AssetBundleItem { AssetBundleName = x }))
                .ToArray();
            EditorUtility.SetDirty(assetBundleSettings);

            _bundles = assetBundleSettings.Items;
        }
    }
}