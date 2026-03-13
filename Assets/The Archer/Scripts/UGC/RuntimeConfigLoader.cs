
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine.Networking;

namespace TheArcher.UGC
{
    /// <summary>
    /// UGC 配置设置
    /// </summary>
    [System.Serializable]
    public class UGCConfigSettings
    {
        /// <summary>
        /// CDN 基础 URL（如：https://cdn.example.com/ugc/）
        /// 留空则只使用本地配置
        /// </summary>
        public string cdnBaseUrl = "";
        
        /// <summary>
        /// 是否优先使用 CDN 配置
        /// true: 先尝试 CDN，失败后回退到本地
        /// false: 只使用本地配置
        /// </summary>
        public bool useCDN = false;
        
        /// <summary>
        /// CDN 请求超时时间（秒）
        /// </summary>
        public int cdnTimeout = 10;
        
        /// <summary>
        /// 是否缓存 CDN 下载的配置到本地
        /// </summary>
        public bool cacheToLocal = true;
    }
    
    /// <summary>
    /// CDN 配置清单格式
    /// </summary>
    [System.Serializable]
    public class UGCManifest
    {
        /// <summary>
        /// 配置版本号
        /// </summary>
        public string version = "1.0.0";
        
        /// <summary>
        /// 配置文件列表
        /// </summary>
        public List<UGCConfigFile> files = new List<UGCConfigFile>();
    }
    
    /// <summary>
    /// 单个配置文件信息
    /// </summary>
    [System.Serializable]
    public class UGCConfigFile
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string name;
        
        /// <summary>
        /// 文件 MD5 哈希（用于版本对比）
        /// </summary>
        public string hash;
        
        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        public long size;
    }
    
    /// <summary>
    /// 运行时配置加载器
    /// 支持从本地 StreamingAssets 或远程 CDN 加载 JSON 配置
    /// 并覆盖对应 ScriptableObject 的属性值
    /// </summary>
    public static class RuntimeConfigLoader
    {
        private const string CONFIG_FOLDER = "UGC_Configs";
        private const string MANIFEST_FILE = "manifest.json";
        private const string CACHE_FOLDER = "UGC_Cache";
        private const string VERSION_FILE = "version.txt";
        
        private static bool isInitialized = false;
        private static UGCConfigSettings settings = new UGCConfigSettings();
        
        /// <summary>
        /// 当前加载的配置版本
        /// </summary>
        public static string CurrentVersion { get; private set; } = "0.0.0";
        
        /// <summary>
        /// 配置加载完成事件
        /// </summary>
        public static event Action<bool, string> OnConfigLoaded;
        
        /// <summary>
        /// 配置 CDN 设置
        /// 需要在游戏启动前调用（如在 Awake 中）
        /// </summary>
        public static void Configure(UGCConfigSettings newSettings)
        {
            settings = newSettings ?? new UGCConfigSettings();
            Debug.Log($"[UGC] 配置已更新: CDN={settings.useCDN}, URL={settings.cdnBaseUrl}");
        }
        
        /// <summary>
        /// 配置 CDN URL（简化版）
        /// </summary>
        public static void SetCDNUrl(string cdnUrl, bool enableCDN = true)
        {
            settings.cdnBaseUrl = cdnUrl;
            settings.useCDN = enableCDN;
            Debug.Log($"[UGC] CDN 配置: {(enableCDN ? "启用" : "禁用")}, URL={cdnUrl}");
        }
        
        /// <summary>
        /// 游戏启动时自动调用（同步加载本地配置）
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (isInitialized) return;
            isInitialized = true;
            
            Debug.Log("[UGC] RuntimeConfigLoader 初始化...");
            
            // 先同步加载本地配置（确保在场景加载前完成）
            LoadLocalConfigsSync();
        }
        
        /// <summary>
        /// 手动触发 CDN 配置加载（异步）
        /// 通常在游戏启动后、进入主菜单前调用
        /// </summary>
        public static void LoadFromCDN(MonoBehaviour runner, Action<bool, string> callback = null)
        {
            if (!settings.useCDN || string.IsNullOrEmpty(settings.cdnBaseUrl))
            {
                Debug.Log("[UGC] CDN 未配置，跳过远程加载");
                callback?.Invoke(false, "CDN not configured");
                return;
            }
            
            runner.StartCoroutine(LoadFromCDNCoroutine(callback));
        }
        
        /// <summary>
        /// CDN 加载协程
        /// </summary>
        private static IEnumerator LoadFromCDNCoroutine(Action<bool, string> callback)
        {
            Debug.Log($"[UGC] 开始从 CDN 加载配置: {settings.cdnBaseUrl}");
            
            // 1. 下载 manifest.json
            string manifestUrl = CombineUrl(settings.cdnBaseUrl, MANIFEST_FILE);
            UGCManifest manifest = null;
            
            using (var request = UnityWebRequest.Get(manifestUrl))
            {
                request.timeout = settings.cdnTimeout;
                yield return request.SendWebRequest();
                
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"[UGC] 下载清单失败: {request.error}");
                    callback?.Invoke(false, $"Failed to download manifest: {request.error}");
                    yield break;
                }
                
                try
                {
                    manifest = JsonUtility.FromJson<UGCManifest>(request.downloadHandler.text);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[UGC] 解析清单失败: {e.Message}");
                    callback?.Invoke(false, $"Failed to parse manifest: {e.Message}");
                    yield break;
                }
            }
            
            if (manifest == null || manifest.files == null || manifest.files.Count == 0)
            {
                Debug.Log("[UGC] 清单为空，无需更新");
                callback?.Invoke(true, "No updates");
                yield break;
            }
            
            Debug.Log($"[UGC] 清单版本: {manifest.version}, 文件数: {manifest.files.Count}");
            
            // 2. 检查版本是否需要更新
            string cachedVersion = GetCachedVersion();
            if (cachedVersion == manifest.version)
            {
                Debug.Log($"[UGC] 版本相同 ({manifest.version})，从缓存加载");
                LoadCachedConfigs();
                callback?.Invoke(true, "Loaded from cache");
                yield break;
            }
            
            // 3. 下载所有配置文件
            int successCount = 0;
            int totalCount = manifest.files.Count;
            
            foreach (var file in manifest.files)
            {
                string fileUrl = CombineUrl(settings.cdnBaseUrl, file.name);
                
                using (var request = UnityWebRequest.Get(fileUrl))
                {
                    request.timeout = settings.cdnTimeout;
                    yield return request.SendWebRequest();
                    
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogWarning($"[UGC] 下载配置失败: {file.name}, {request.error}");
                        continue;
                    }
                    
                    string jsonContent = request.downloadHandler.text;
                    
                    // 应用配置
                    ApplyConfig(jsonContent, file.name);
                    
                    // 缓存到本地
                    if (settings.cacheToLocal)
                    {
                        CacheConfigFile(file.name, jsonContent);
                    }
                    
                    successCount++;
                }
            }
            
            // 4. 保存版本号
            if (settings.cacheToLocal && successCount > 0)
            {
                SaveCachedVersion(manifest.version);
            }
            
            CurrentVersion = manifest.version;
            
            string message = $"CDN load complete: {successCount}/{totalCount}";
            Debug.Log($"[UGC] {message}");
            
            OnConfigLoaded?.Invoke(successCount == totalCount, message);
            callback?.Invoke(successCount > 0, message);
        }
        
        /// <summary>
        /// 同步加载本地配置文件
        /// </summary>
        private static void LoadLocalConfigsSync()
        {
            // 优先加载缓存的 CDN 配置
            if (settings.cacheToLocal && HasCachedConfigs())
            {
                LoadCachedConfigs();
                return;
            }
            
            // 加载 StreamingAssets 中的配置
            string configPath = Path.Combine(Application.streamingAssetsPath, CONFIG_FOLDER);
            
            if (!Directory.Exists(configPath))
            {
                Debug.Log($"[UGC] 配置目录不存在: {configPath}，跳过配置加载");
                return;
            }
            
            var jsonFiles = Directory.GetFiles(configPath, "*.json");
            Debug.Log($"[UGC] 找到 {jsonFiles.Length} 个本地配置文件");
            
            int successCount = 0;
            foreach (var jsonFile in jsonFiles)
            {
                try
                {
                    var jsonContent = File.ReadAllText(jsonFile, Encoding.UTF8);
                    ApplyConfig(jsonContent, Path.GetFileName(jsonFile));
                    successCount++;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[UGC] 读取配置文件失败: {jsonFile}, 错误: {e.Message}");
                }
            }
            
            Debug.Log($"[UGC] 本地配置加载完成，成功: {successCount}/{jsonFiles.Length}");
        }
        
        /// <summary>
        /// 加载所有配置文件（异步版本，用于 Android）
        /// </summary>
        private static IEnumerator LoadAllConfigs()
        {
            string configPath = Path.Combine(Application.streamingAssetsPath, CONFIG_FOLDER);
            
            #if UNITY_ANDROID && !UNITY_EDITOR
            yield return LoadConfigsFromManifest();
            #else
            if (!Directory.Exists(configPath))
            {
                Debug.Log($"[UGC] 配置目录不存在: {configPath}，跳过配置加载");
                yield break;
            }
            
            var jsonFiles = Directory.GetFiles(configPath, "*.json");
            Debug.Log($"[UGC] 找到 {jsonFiles.Length} 个配置文件");
            
            int successCount = 0;
            foreach (var jsonFile in jsonFiles)
            {
                bool success = false;
                yield return LoadAndApplyConfig(jsonFile, (result) => success = result);
                if (success) successCount++;
            }
            
            Debug.Log($"[UGC] 配置加载完成，成功: {successCount}/{jsonFiles.Length}");
            #endif
        }
        
        /// <summary>
        /// Android 平台从清单文件加载配置
        /// </summary>
        private static IEnumerator LoadConfigsFromManifest()
        {
            string manifestPath = Path.Combine(Application.streamingAssetsPath, CONFIG_FOLDER, "manifest.txt");
            
            using (var request = UnityWebRequest.Get(manifestPath))
            {
                yield return request.SendWebRequest();
                
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("[UGC] 未找到配置清单文件，跳过配置加载");
                    yield break;
                }
                
                var fileList = request.downloadHandler.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                Debug.Log($"[UGC] 从清单找到 {fileList.Length} 个配置文件");
                
                int successCount = 0;
                foreach (var fileName in fileList)
                {
                    string filePath = Path.Combine(Application.streamingAssetsPath, CONFIG_FOLDER, fileName.Trim());
                    bool success = false;
                    yield return LoadAndApplyConfigFromUrl(filePath, (result) => success = result);
                    if (success) successCount++;
                }
                
                Debug.Log($"[UGC] 配置加载完成，成功: {successCount}/{fileList.Length}");
            }
        }
        
        /// <summary>
        /// 加载并应用单个配置文件（本地文件系统）
        /// </summary>
        private static IEnumerator LoadAndApplyConfig(string filePath, Action<bool> callback)
        {
            string jsonContent;
            
            try
            {
                jsonContent = File.ReadAllText(filePath, Encoding.UTF8);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[UGC] 读取配置文件失败: {filePath}, 错误: {e.Message}");
                callback(false);
                yield break;
            }
            
            ApplyConfig(jsonContent, Path.GetFileName(filePath));
            callback(true);
        }
        
        /// <summary>
        /// 加载并应用单个配置文件（URL/Android/CDN）
        /// </summary>
        private static IEnumerator LoadAndApplyConfigFromUrl(string url, Action<bool> callback)
        {
            using (var request = UnityWebRequest.Get(url))
            {
                request.timeout = settings.cdnTimeout;
                yield return request.SendWebRequest();
                
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"[UGC] 加载配置失败: {url}");
                    callback(false);
                    yield break;
                }
                
                ApplyConfig(request.downloadHandler.text, Path.GetFileName(url));
                callback(true);
            }
        }
        
        #region 缓存管理
        
        /// <summary>
        /// 获取缓存目录路径
        /// </summary>
        private static string GetCachePath()
        {
            return Path.Combine(Application.persistentDataPath, CACHE_FOLDER);
        }
        
        /// <summary>
        /// 检查是否有缓存的配置
        /// </summary>
        private static bool HasCachedConfigs()
        {
            string cachePath = GetCachePath();
            if (!Directory.Exists(cachePath)) return false;
            
            var files = Directory.GetFiles(cachePath, "*.json");
            return files.Length > 0;
        }
        
        /// <summary>
        /// 加载缓存的配置
        /// </summary>
        private static void LoadCachedConfigs()
        {
            string cachePath = GetCachePath();
            if (!Directory.Exists(cachePath)) return;
            
            var jsonFiles = Directory.GetFiles(cachePath, "*.json");
            Debug.Log($"[UGC] 从缓存加载 {jsonFiles.Length} 个配置文件");
            
            int successCount = 0;
            foreach (var jsonFile in jsonFiles)
            {
                try
                {
                    var jsonContent = File.ReadAllText(jsonFile, Encoding.UTF8);
                    ApplyConfig(jsonContent, Path.GetFileName(jsonFile));
                    successCount++;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[UGC] 读取缓存配置失败: {jsonFile}, 错误: {e.Message}");
                }
            }
            
            CurrentVersion = GetCachedVersion();
            Debug.Log($"[UGC] 缓存配置加载完成，版本: {CurrentVersion}, 成功: {successCount}/{jsonFiles.Length}");
        }
        
        /// <summary>
        /// 缓存配置文件到本地
        /// </summary>
        private static void CacheConfigFile(string fileName, string content)
        {
            try
            {
                string cachePath = GetCachePath();
                if (!Directory.Exists(cachePath))
                {
                    Directory.CreateDirectory(cachePath);
                }
                
                string filePath = Path.Combine(cachePath, fileName);
                File.WriteAllText(filePath, content, Encoding.UTF8);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[UGC] 缓存配置失败: {fileName}, 错误: {e.Message}");
            }
        }
        
        /// <summary>
        /// 获取缓存的版本号
        /// </summary>
        private static string GetCachedVersion()
        {
            try
            {
                string versionPath = Path.Combine(GetCachePath(), VERSION_FILE);
                if (File.Exists(versionPath))
                {
                    return File.ReadAllText(versionPath, Encoding.UTF8).Trim();
                }
            }
            catch { }
            return "0.0.0";
        }
        
        /// <summary>
        /// 保存版本号到缓存
        /// </summary>
        private static void SaveCachedVersion(string version)
        {
            try
            {
                string cachePath = GetCachePath();
                if (!Directory.Exists(cachePath))
                {
                    Directory.CreateDirectory(cachePath);
                }
                
                string versionPath = Path.Combine(cachePath, VERSION_FILE);
                File.WriteAllText(versionPath, version, Encoding.UTF8);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[UGC] 保存版本号失败: {e.Message}");
            }
        }
        
        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public static void ClearCache()
        {
            try
            {
                string cachePath = GetCachePath();
                if (Directory.Exists(cachePath))
                {
                    Directory.Delete(cachePath, true);
                    Debug.Log("[UGC] 缓存已清除");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[UGC] 清除缓存失败: {e.Message}");
            }
        }
        
        #endregion
        
        #region 工具方法
        
        /// <summary>
        /// 合并 URL 路径
        /// </summary>
        private static string CombineUrl(string baseUrl, string path)
        {
            if (string.IsNullOrEmpty(baseUrl)) return path;
            
            baseUrl = baseUrl.TrimEnd('/');
            path = path.TrimStart('/');
            
            return $"{baseUrl}/{path}";
        }
        
        #endregion
        
        /// <summary>
        /// 应用配置到 ScriptableObject
        /// </summary>
        private static void ApplyConfig(string jsonContent, string fileName)
        {
            try
            {
                var data = SimpleJsonParser.Parse(jsonContent);
                if (data == null)
                {
                    Debug.LogWarning($"[UGC] 解析 JSON 失败: {fileName}");
                    return;
                }
                
                // 获取 assetPath
                if (!data.TryGetValue("assetPath", out var assetPathObj) || assetPathObj == null)
                {
                    Debug.LogWarning($"[UGC] 配置缺少 assetPath: {fileName}");
                    return;
                }
                
                string assetPath = assetPathObj.ToString();
                
                // 加载对应的 ScriptableObject
                ScriptableObject so = null;
                
                #if UNITY_EDITOR
                // 编辑器中使用 AssetDatabase 直接加载（最可靠）
                so = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
                #else
                // 运行时：先尝试 Resources 加载
                so = Resources.Load<ScriptableObject>(ConvertToResourcesPath(assetPath));
                
                // 如果 Resources 加载失败，尝试从已加载的 SO 中查找
                if (so == null)
                {
                    so = FindLoadedScriptableObject(assetPath);
                }
                #endif
                
                if (so == null)
                {
                    Debug.LogWarning($"[UGC] 找不到 ScriptableObject: {assetPath}");
                    return;
                }
                
                // 应用配置
                ApplyDataToObject(so, data);
                Debug.Log($"[UGC] 配置已应用: {fileName} -> {so.name}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[UGC] 应用配置失败: {fileName}, 错误: {e.Message}\n{e.StackTrace}");
            }
        }
        
        /// <summary>
        /// 将 Assets 路径转换为 Resources 路径
        /// </summary>
        private static string ConvertToResourcesPath(string assetPath)
        {
            // Assets/The Archer/Scriptables/xxx.asset -> The Archer/Scriptables/xxx
            if (assetPath.StartsWith("Assets/"))
                assetPath = assetPath.Substring(7);
            
            if (assetPath.EndsWith(".asset"))
                assetPath = assetPath.Substring(0, assetPath.Length - 6);
            
            // 如果路径包含 Resources，提取 Resources 后的部分
            int resourcesIndex = assetPath.IndexOf("Resources/");
            if (resourcesIndex >= 0)
                assetPath = assetPath.Substring(resourcesIndex + 10);
            
            return assetPath;
        }
        
        /// <summary>
        /// 从已加载的对象中查找 ScriptableObject
        /// </summary>
        private static ScriptableObject FindLoadedScriptableObject(string assetPath)
        {
            // 提取文件名（不含扩展名）
            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            
            // 方法1：查找所有已加载的 ScriptableObject（包括未激活的）
            var allSOs = Resources.FindObjectsOfTypeAll<ScriptableObject>();
            foreach (var so in allSOs)
            {
                if (so.name == fileName)
                    return so;
            }
            
            // 方法2：如果上面找不到，可能是因为 SO 还没被任何场景引用
            // 这种情况下，我们需要等待场景加载后再尝试
            // 但由于我们在 BeforeSceneLoad 阶段运行，这里暂时返回 null
            // 这些 SO 会在后续被场景引用时自动加载
            
            return null;
        }
        
        /// <summary>
        /// 存储待应用的配置（用于延迟加载的 SO）
        /// </summary>
        private static Dictionary<string, Dictionary<string, object>> pendingConfigs = new Dictionary<string, Dictionary<string, object>>();
        
        /// <summary>
        /// 尝试应用待处理的配置
        /// 可以在场景加载后调用此方法
        /// </summary>
        public static void TryApplyPendingConfigs()
        {
            if (pendingConfigs.Count == 0) return;
            
            var appliedKeys = new List<string>();
            
            foreach (var kvp in pendingConfigs)
            {
                var so = FindLoadedScriptableObject(kvp.Key);
                if (so != null)
                {
                    ApplyDataToObject(so, kvp.Value);
                    Debug.Log($"[UGC] 延迟配置已应用: {kvp.Key} -> {so.name}");
                    appliedKeys.Add(kvp.Key);
                }
            }
            
            foreach (var key in appliedKeys)
            {
                pendingConfigs.Remove(key);
            }
            
            if (pendingConfigs.Count > 0)
            {
                Debug.Log($"[UGC] 仍有 {pendingConfigs.Count} 个配置待应用");
            }
        }
        
        /// <summary>
        /// 将数据应用到对象
        /// </summary>
        private static void ApplyDataToObject(object target, Dictionary<string, object> data)
        {
            if (target == null || data == null) return;
            
            var type = target.GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            foreach (var field in fields)
            {
                // 跳过 Unity Object 引用类型（Sprite, GameObject, ScriptableObject 等）
                // 这些类型不应该被 JSON 覆盖，因为它们需要通过 Unity 的资源系统加载
                if (typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType))
                {
                    continue;
                }
                
                // 尝试匹配字段名
                string fieldName = field.Name;
                string altName = fieldName.StartsWith("m_") ? fieldName.Substring(2) : fieldName;
                
                object jsonValue = null;
                if (data.TryGetValue(fieldName, out jsonValue) || data.TryGetValue(altName, out jsonValue))
                {
                    // 跳过 null 值，保持原有值不变
                    if (jsonValue == null) continue;
                    
                    // 跳过数组/列表中包含 Unity Object 引用的类型
                    if (IsUnityObjectCollection(field.FieldType))
                    {
                        continue;
                    }
                    
                    try
                    {
                        var convertedValue = ConvertValue(jsonValue, field.FieldType);
                        if (convertedValue != null)
                        {
                            field.SetValue(target, convertedValue);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"[UGC] 设置字段 {fieldName} 失败: {e.Message}");
                    }
                }
            }
        }
        
        /// <summary>
        /// 检查类型是否是包含 Unity Object 的集合
        /// </summary>
        private static bool IsUnityObjectCollection(Type type)
        {
            // 检查数组
            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                return typeof(UnityEngine.Object).IsAssignableFrom(elementType);
            }
            
            // 检查 List<T>
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var elementType = type.GetGenericArguments()[0];
                return typeof(UnityEngine.Object).IsAssignableFrom(elementType);
            }
            
            return false;
        }
        
        /// <summary>
        /// 转换值到目标类型
        /// </summary>
        private static object ConvertValue(object value, Type targetType)
        {
            if (value == null) return null;
            
            // 字符串
            if (targetType == typeof(string))
                return value.ToString();
            
            // 基础数值类型
            if (targetType == typeof(int))
                return Convert.ToInt32(value);
            if (targetType == typeof(float))
                return Convert.ToSingle(value);
            if (targetType == typeof(double))
                return Convert.ToDouble(value);
            if (targetType == typeof(bool))
                return Convert.ToBoolean(value);
            if (targetType == typeof(long))
                return Convert.ToInt64(value);
            
            // 枚举
            if (targetType.IsEnum)
                return Enum.ToObject(targetType, Convert.ToInt32(value));
            
            // 数组
            if (targetType.IsArray && value is List<object> list)
            {
                var elementType = targetType.GetElementType();
                var array = Array.CreateInstance(elementType, list.Count);
                for (int i = 0; i < list.Count; i++)
                {
                    array.SetValue(ConvertValue(list[i], elementType), i);
                }
                return array;
            }
            
            // List<T>
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
            {
                if (value is List<object> sourceList)
                {
                    var elementType = targetType.GetGenericArguments()[0];
                    
                    // 如果元素类型是 Unity Object 或没有无参构造函数的复杂类型，跳过
                    if (typeof(UnityEngine.Object).IsAssignableFrom(elementType))
                    {
                        return null;
                    }
                    
                    var listInstance = Activator.CreateInstance(targetType) as System.Collections.IList;
                    foreach (var item in sourceList)
                    {
                        var convertedItem = ConvertValue(item, elementType);
                        // 跳过 null 值，避免添加失败
                        if (convertedItem != null)
                        {
                            listInstance.Add(convertedItem);
                        }
                    }
                    return listInstance;
                }
            }
            
            // 嵌套对象
            if (value is Dictionary<string, object> dict)
            {
                if (targetType.IsClass || targetType.IsValueType)
                {
                    try
                    {
                        // 尝试创建实例（需要无参构造函数）
                        var instance = Activator.CreateInstance(targetType);
                        ApplyDataToObject(instance, dict);
                        return instance;
                    }
                    catch (MissingMethodException)
                    {
                        // 没有无参构造函数，跳过此字段
                        return null;
                    }
                }
            }
            
            // 尝试直接转换
            try
            {
                return Convert.ChangeType(value, targetType);
            }
            catch
            {
                return null;
            }
        }
    }
    
    /// <summary>
    /// 协程运行器
    /// </summary>
    internal class ConfigLoaderRunner : MonoBehaviour
    {
        private void Awake()
        {
            // 加载完成后销毁自己
            StartCoroutine(DestroyAfterLoad());
        }
        
        private IEnumerator DestroyAfterLoad()
        {
            // 等待一帧确保所有配置加载完成
            yield return null;
            yield return null;
            
            // 可选：加载完成后销毁
            // Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 简单的 JSON 解析器（运行时使用，不依赖 Editor）
    /// </summary>
    internal static class SimpleJsonParser
    {
        public static Dictionary<string, object> Parse(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            
            int index = 0;
            return ParseObject(json, ref index);
        }
        
        private static void SkipWhitespace(string json, ref int index)
        {
            while (index < json.Length && char.IsWhiteSpace(json[index]))
                index++;
        }
        
        private static object ParseValue(string json, ref int index)
        {
            SkipWhitespace(json, ref index);
            if (index >= json.Length) return null;
            
            char c = json[index];
            
            if (c == '"') return ParseString(json, ref index);
            if (c == '{') return ParseObject(json, ref index);
            if (c == '[') return ParseArray(json, ref index);
            if (c == 't' || c == 'f') return ParseBool(json, ref index);
            if (c == 'n') return ParseNull(json, ref index);
            if (char.IsDigit(c) || c == '-') return ParseNumber(json, ref index);
            
            return null;
        }
        
        private static string ParseString(string json, ref int index)
        {
            index++; // Skip opening quote
            var sb = new StringBuilder();
            
            while (index < json.Length)
            {
                char c = json[index];
                if (c == '"')
                {
                    index++;
                    return sb.ToString();
                }
                if (c == '\\' && index + 1 < json.Length)
                {
                    index++;
                    char escaped = json[index];
                    switch (escaped)
                    {
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        case 'u':
                            // Unicode escape
                            if (index + 4 < json.Length)
                            {
                                string hex = json.Substring(index + 1, 4);
                                if (int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out int codePoint))
                                {
                                    sb.Append((char)codePoint);
                                    index += 4;
                                }
                            }
                            break;
                        default: sb.Append(escaped); break;
                    }
                }
                else
                {
                    sb.Append(c);
                }
                index++;
            }
            
            return sb.ToString();
        }
        
        private static Dictionary<string, object> ParseObject(string json, ref int index)
        {
            var dict = new Dictionary<string, object>();
            index++; // Skip '{'
            
            SkipWhitespace(json, ref index);
            if (index < json.Length && json[index] == '}')
            {
                index++;
                return dict;
            }
            
            while (index < json.Length)
            {
                SkipWhitespace(json, ref index);
                if (index >= json.Length || json[index] != '"') break;
                
                var key = ParseString(json, ref index);
                
                SkipWhitespace(json, ref index);
                if (index >= json.Length || json[index] != ':') break;
                index++; // Skip ':'
                
                var value = ParseValue(json, ref index);
                dict[key] = value;
                
                SkipWhitespace(json, ref index);
                if (index >= json.Length) break;
                if (json[index] == '}')
                {
                    index++;
                    return dict;
                }
                if (json[index] == ',') index++;
            }
            
            return dict;
        }
        
        private static List<object> ParseArray(string json, ref int index)
        {
            var list = new List<object>();
            index++; // Skip '['
            
            SkipWhitespace(json, ref index);
            if (index < json.Length && json[index] == ']')
            {
                index++;
                return list;
            }
            
            while (index < json.Length)
            {
                var value = ParseValue(json, ref index);
                list.Add(value);
                
                SkipWhitespace(json, ref index);
                if (index >= json.Length) break;
                if (json[index] == ']')
                {
                    index++;
                    return list;
                }
                if (json[index] == ',') index++;
            }
            
            return list;
        }
        
        private static bool ParseBool(string json, ref int index)
        {
            if (json.Length >= index + 4 && json.Substring(index, 4) == "true")
            {
                index += 4;
                return true;
            }
            if (json.Length >= index + 5 && json.Substring(index, 5) == "false")
            {
                index += 5;
                return false;
            }
            return false;
        }
        
        private static object ParseNull(string json, ref int index)
        {
            if (json.Length >= index + 4 && json.Substring(index, 4) == "null")
            {
                index += 4;
                return null;
            }
            return null;
        }
        
        private static object ParseNumber(string json, ref int index)
        {
            int start = index;
            bool isFloat = false;
            
            if (json[index] == '-') index++;
            
            while (index < json.Length)
            {
                char c = json[index];
                if (char.IsDigit(c))
                {
                    index++;
                }
                else if (c == '.' || c == 'e' || c == 'E' || c == '+' || c == '-')
                {
                    isFloat = true;
                    index++;
                }
                else
                {
                    break;
                }
            }
            
            string numStr = json.Substring(start, index - start);
            
            if (isFloat)
            {
                if (double.TryParse(numStr, System.Globalization.NumberStyles.Float, 
                    System.Globalization.CultureInfo.InvariantCulture, out double d))
                    return d;
            }
            else
            {
                if (long.TryParse(numStr, out long l))
                    return l;
            }
            
            return 0;
        }
    }
}
