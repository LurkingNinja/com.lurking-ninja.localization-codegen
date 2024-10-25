/***
 * Localization Codegen
 * Copyright (c) 2022-2024 Lurking Ninja.
 *
 * MIT License
 * https://github.com/LurkingNinja/com.lurking-ninja.localization-codegen
 */
namespace LurkingNinja.Localization.Editor
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using Object = UnityEngine.Object;

    // To detect creation and saving an asset. We do not care about moving.
    public class OnAssetPostProcessor : AssetPostprocessor
    {
        private static readonly Dictionary<Type, List<Action<Object, string>>> _CHANGE_CALLBACKS = new();
        internal static readonly Dictionary<Type, List<Action<Object, string>>> DELETE_CALLBACKS = new();

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var path in importedAssets)
            {
                foreach (var keyValue in _CHANGE_CALLBACKS)
                {
                    var asset = AssetDatabase.LoadAssetAtPath(path, keyValue.Key);
                    if (asset is null) continue;
                    foreach (var action in keyValue.Value)
                    {
                        action?.Invoke(asset, path);
                    }
                }
            }
        }

        public static void AddListener(Type key,
            Action<Object, string> changeCallback, Action<Object, string> deleteCallback)
        {
            AddChangeListener(key, changeCallback);
            AddDeletionListener(key, deleteCallback);
        }

        public static void RemoveListener(Type key,
            Action<Object, string> changeCallback, Action<Object, string> deleteCallback)
        {
            RemoveChangeListener(key, changeCallback);
            RemoveDeletionListener(key, deleteCallback);
        }

        private static void AddChangeListener(Type key, Action<Object, string> callback)
        {
            if (!_CHANGE_CALLBACKS.ContainsKey(key)) _CHANGE_CALLBACKS[key] = new List<Action<Object, string>>();
            if (_CHANGE_CALLBACKS[key].Contains(callback)) return;
            _CHANGE_CALLBACKS[key].Add(callback);
        }

        private static void RemoveChangeListener(Type key, Action<Object, string> callback)
        {
            if (!_CHANGE_CALLBACKS.TryGetValue(key, out var changeCallback)) return;
            changeCallback.Remove(callback);
        }

        private static void AddDeletionListener(Type key, Action<Object, string> callback)
        {
            if (!DELETE_CALLBACKS.ContainsKey(key)) DELETE_CALLBACKS[key] = new List<Action<Object, string>>();
            if (DELETE_CALLBACKS[key].Contains(callback)) return;
            DELETE_CALLBACKS[key].Add(callback);
        }

        private static void RemoveDeletionListener(Type key, Action<Object, string> callback)
        {
            if (!DELETE_CALLBACKS.TryGetValue(key, out var deleteCallback)) return;
            deleteCallback.Remove(callback);
        }
    }

    // To detect asset removal.
    public class CustomAssetModificationProcessor : AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions rao)
        {
            foreach (var keyValue in OnAssetPostProcessor.DELETE_CALLBACKS)
            {
                var asset = AssetDatabase.LoadAssetAtPath(path, keyValue.Key);
                if (asset is null) continue;
                foreach (var action in keyValue.Value)
                {
                    action?.Invoke(asset, path);
                }
            }
            return AssetDeleteResult.DidNotDelete;
        }
    }
}