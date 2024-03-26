using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UniVRM10;
using System.Collections.Generic;
using VRMShaders;
using Object = UnityEngine.Object;
namespace Kurisu.Framework.VRM
{
    public readonly struct VRMHandle : IDisposable
    {
        private readonly int handleID;
        public VRMHandle(int handleID)
        {
            this.handleID = handleID;
        }
        public Task<GameObject> Task
        {
            get
            {
                if (VRMSpawnSystem.TryGetInstance(handleID, out var instance))
                {
                    return instance.Task;
                }
                return null;
            }
        }
        public bool IsValid()
        {
            return VRMSpawnSystem.IsValid(handleID);
        }
        public void Dispose()
        {
            VRMSpawnSystem.Release(handleID);
        }
    }
    public class VRMSpawnSystem
    {
        internal class VRMInstance : IDisposable
        {
            public string RootPath { get; }
            public Task<GameObject> Task { get; }
            public VRMInstance(string rootPath, Task<GameObject> task)
            {
                RootPath = rootPath;
                Task = task;
            }
            public void Dispose()
            {
                Object.Destroy(Task.Result);
            }
        }
        private static int handleIndex = 1;
        private static CancellationTokenSource _cancellationTokenSource;
        private static readonly Dictionary<string, int> countMap = new();
        private static readonly Dictionary<int, VRMInstance> instanceMap = new();
        private static readonly Dictionary<string, VRMInstance> managedModelMap = new();
        /// <summary>
        /// Load vrm model with cache
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static VRMHandle InstantiateVRMAsync(string path, Transform parent)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"VRM file not exist with path {path}");
            }
            return CreateHandle(path, InstantiateVRMAsyncInternal(path, parent));
        }
        /// <summary>
        /// Directly load vrm model without cache
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static async Task<GameObject> LoadVRMAsync(string path, Transform parent)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"VRM file not exist with path {path}");
            }
            var instance = await LoadVRMInternal(path, false);
            instance.transform.SetParent(parent);
            instance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            return instance;
        }
        private static VRMHandle CreateHandle(string path, Task<GameObject> task)
        {
            instanceMap.Add(++handleIndex, new(path, task));
            if (countMap.TryGetValue(path, out var count))
                countMap[path] = count + 1;
            else
                countMap[path] = 1;
            return new VRMHandle(handleIndex);
        }
        internal static bool IsValid(int handleID)
        {
            return instanceMap.TryGetValue(handleID, out _);
        }
        internal static bool TryGetInstance(int handleID, out VRMInstance instance)
        {
            return instanceMap.TryGetValue(handleID, out instance);
        }
        private static async Task<GameObject> InstantiateVRMAsyncInternal(string path, Transform parent)
        {
            if (!managedModelMap.TryGetValue(path, out var root))
            {
                root = managedModelMap[path] = new(path, LoadVRMInternal(path));
            }
            var instance = Object.Instantiate(await root.Task, parent);
            instance.SetActive(true);
            return instance;
        }
        internal static void Release(int handleID)
        {
            if (!instanceMap.TryGetValue(handleID, out var instance)) return;
            if (!countMap.TryGetValue(instance.RootPath, out var count)) return;
            --count;
            if (count == 0)
            {
                ReleaseInternal(instance.RootPath);
                countMap.Remove(instance.RootPath);
            }
            else
            {
                countMap[instance.RootPath] = count;
            }
            instance.Dispose();
            instanceMap.Remove(handleID);
        }
        private static void ReleaseInternal(string path)
        {
            Object.Destroy(managedModelMap[path].Task.Result);
            managedModelMap.Remove(path);
        }
        private static async Task<GameObject> LoadVRMInternal(string path, bool hideOnLoad = true)
        {
            _cancellationTokenSource ??= new();
            var cancellationToken = _cancellationTokenSource.Token;
            try
            {
                Vrm10Instance vrm10Instance = null;
                vrm10Instance = await Vrm10.LoadPathAsync(path,
                    canLoadVrm0X: true,
                    showMeshes: true,
                    awaitCaller: new RuntimeOnlyAwaitCaller(),
                    materialGenerator: new UrpVrm10MaterialDescriptorGenerator(),
                    ct: cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    UnityObjectDestroyer.DestroyRuntimeOrEditor(vrm10Instance.gameObject);
                    cancellationToken.ThrowIfCancellationRequested();
                }
                if (hideOnLoad)
                    vrm10Instance.gameObject.SetActive(false);
                return vrm10Instance.gameObject;
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning($"Loading was cancelled: {path}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load: {path}");
                Debug.LogException(ex);
                return null;
            }
        }
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void BeforeLoad()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
#endif
    }
}