using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityIdeEx.Editor.ide_ex.Scripts.Editor.Assets;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Windows
{
    public sealed class AssetBundleBuildWindow : EditorWindow
    {
        #region Properties

        public bool Result { get; private set; } = false;
        public IDictionary<string, bool> BuildStates => _states;

        #endregion
        
        private readonly IDictionary<string, bool> _states = new Dictionary<string, bool>();
        private Vector2 _scroll = Vector2.zero;
        
        private void OnEnable()
        {
            titleContent = new GUIContent("Build Asset Bundles", EditorGUIUtility.IconContent("d_Profiler.NetworkOperations").image);
            UpdateStates();
        }

        private void OnFocus()
        {
            UpdateStates();
        }

        private void OnGUI()
        {
            _scroll = GUILayout.BeginScrollView(_scroll);
            {
                foreach (var item in AssetBundleSettings.Singleton.Items)
                {
                    _states[item.AssetBundleName] = GUILayout.Toggle(_states[item.AssetBundleName], item.AssetBundleName);
                }
            }
            GUILayout.EndScrollView();
            
            GUILayout.Space(25f);

            if (GUILayout.Button("Build"))
            {
                Result = true;
                Close();
            }
        }

        private void UpdateStates()
        {
            var addedList = AssetBundleSettings.Singleton.Items.Where(x => !_states.Keys.Contains(x.AssetBundleName));
            var removedList = _states.Keys.Where(x => !AssetBundleSettings.Singleton.Items.Any(y => string.Equals(x, y.AssetBundleName)));

            foreach (var item in addedList)
            {
                _states.Add(item.AssetBundleName, item.BuildAssetBundle);
            }

            foreach (var item in removedList)
            {
                _states.Remove(item);
            }
        }
    }
}