#if !UNITY_EDITOR
using Comfort.Common;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace DrakiaXYZ.Hazardifier.Utils
{
    public class AssetUtils
    {
        private static MethodInfo _retainMethod;
        private static Type _resultType;
        private static PropertyInfo _loadingJobProp;
        private static MethodInfo _getAssetMethod;

        static AssetUtils()
        {
            _retainMethod = AccessTools.FirstMethod(typeof(DependencyGraph<IEasyBundle>), x => x.Name == "Retain" && x.GetParameters().Length == 2);
            _resultType = _retainMethod.ReturnType;
            _loadingJobProp = AccessTools.Property(_resultType, "LoadingJob");
            Type getAssetType = Aki.Reflection.Utils.PatchConstants.EftTypes.FirstOrDefault(type =>
            {
                return type.GetMethods().Any(method => method.Name == "GetAsset");
            });
            _getAssetMethod = AccessTools.FirstMethod(getAssetType, method =>
            {
                return (method.Name == "GetAsset" && method.GetParameters().Length == 3);
            });
        }

        public static Task Retain(string bundlePath)
        {
            var result = _retainMethod.Invoke(Singleton<IEasyAssets>.Instance.System, new object[] { bundlePath, null });

            return _loadingJobProp.GetValue(result) as Task;
        }

        public static T GetAsset<T>(string bundlePath, string assetName)
        {
            return (T)_getAssetMethod.MakeGenericMethod(typeof(T)).Invoke(null, new object[] { Singleton<IEasyAssets>.Instance, bundlePath, assetName });
        }

        public static T LoadAsset<T>(AssetBundle bundle, string assetPath) where T : UnityEngine.Object
        {
            T asset = bundle.LoadAsset<T>(assetPath);

            if (asset == null)
            {
                throw new Exception($"Error loading asset {assetPath}");
            }

            UnityEngine.Object.DontDestroyOnLoad(asset);
            return asset;
        }
    }
}
#endif