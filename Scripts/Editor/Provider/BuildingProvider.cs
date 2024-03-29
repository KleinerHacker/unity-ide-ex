using System;
using System.Linq;
using UnityEditor;
using UnityEditorEx.Editor.editor_ex.Scripts.Editor.Utils.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Assets;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Provider
{
    public sealed class BuildingProvider : SettingsProvider
    {
        #region Static Area

        [SettingsProvider]
        public static SettingsProvider CreateGameSettingsProvider()
        {
            return new BuildingProvider();
        }

        #endregion

        private SerializedObject _settings;
        private SerializedProperty _appNameProperty;
        private SerializedProperty _windowsProperties;
        private SerializedProperty _linuxProperties;
        private SerializedProperty _macOSProperties;
        private SerializedProperty _androidProperties;
        private SerializedProperty _iosProperties;
        private SerializedProperty _webGLProperties;

        private int _selectedTarget;
        private int _windowsBuildingGroup;
        private int _linuxBuildingGroup;
        private int _macOSBuildingGroup;
        private int _androidBuildingGroup;
        private int _iosBuildingGroup;
        private int _webGLBuildingGroup;

        public BuildingProvider()
            : base("Project/Player/Building", SettingsScope.Project, new[] { "Build", "Building", "Tool", "Tooling", "Run", "Running", "Compile", "Compiling" })
        {
        }

        #region Builtin Methods

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            _settings = BuildingSettings.SerializedSingleton;
            if (_settings == null)
                return;
            _appNameProperty = _settings.FindProperty("appName");
            _windowsProperties = _settings.FindProperty("windows");
            _linuxProperties = _settings.FindProperty("linux");
            _macOSProperties = _settings.FindProperty("macOS");
            _androidProperties = _settings.FindProperty("android");
            _iosProperties = _settings.FindProperty("ios");
            _webGLProperties = _settings.FindProperty("webGL");
        }

        public override void OnGUI(string searchContext)
        {
            if (_settings == null || _appNameProperty == null || _windowsProperties == null)
                return;

            _settings.Update();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_appNameProperty, new GUIContent("Name of application"), GUILayout.ExpandWidth(true));
            GUILayout.Space(15f);
            if (GUILayout.Button("Reset", GUILayout.Width(100f)))
            {
                _appNameProperty.stringValue = Application.productName;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(15f);
            _selectedTarget = GUILayout.Toolbar(_selectedTarget, new[]
            {
                new GUIContent(EditorGUIUtility.IconContent("BuildSettings.Metro On").image, "Windows"),
                new GUIContent(Resources.Load<Texture2D>("linux"), "Linux"),
                new GUIContent(EditorGUIUtility.IconContent("BuildSettings.Standalone On").image, "Mac OS"),
                new GUIContent(EditorGUIUtility.IconContent("BuildSettings.Android On").image, "Android"),
                new GUIContent(EditorGUIUtility.IconContent("BuildSettings.iPhone On").image, "IOS"),
                new GUIContent(EditorGUIUtility.IconContent("BuildSettings.WebGL On").image, "Web GL"),
            });

            switch (_selectedTarget)
            {
                case 0:
                    LayoutWindows();
                    break;
                case 1:
                    LayoutLinux();
                    break;
                case 2:
                    LayoutMacOS();
                    break;
                case 3:
                    LayoutAndroid();
                    break;
                case 4:
                    LayoutIOS();
                    break;
                case 5:
                    LayoutWebGL();
                    break;
            }

            _settings.ApplyModifiedProperties();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Space(25f);
            EditorGUILayout.LabelField("Common Build Data", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Scenes:");
            EditorGUILayout.LabelField(string.Join(Environment.NewLine, EditorBuildSettings.scenes.Select(x => x.path).ToArray()), EditorStyles.wordWrappedLabel);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Asset Bundles:");
            EditorGUILayout.LabelField(string.Join(Environment.NewLine, AssetBundleSettings.Singleton.Items.Select(x => x.AssetBundleName + " [" + (x.BuildAssetBundle ? "X" : "O") + "] (Binary Path: " + x.BuildSubPath + ")")), EditorStyles.wordWrappedLabel);
            EditorGUI.EndDisabledGroup();
        }

        #endregion

        private void LayoutWindows()
        {
            EditorGUILayout.PropertyField(_windowsProperties, new GUIContent("Settings"), true);
        }

        private void LayoutLinux()
        {
            EditorGUILayout.PropertyField(_linuxProperties, new GUIContent("Settings"), true);
        }

        private void LayoutMacOS()
        {
            EditorGUILayout.PropertyField(_macOSProperties, new GUIContent("Settings"), true);
        }

        private void LayoutAndroid()
        {
            EditorGUILayout.PropertyField(_androidProperties, new GUIContent("Settings"), true);
        }

        private void LayoutIOS()
        {
            EditorGUILayout.PropertyField(_iosProperties, new GUIContent("Settings"), true);
        }

        private void LayoutWebGL()
        {
            EditorGUILayout.PropertyField(_webGLProperties, new GUIContent("Settings"), true);
        }
    }
}