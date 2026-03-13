using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace TheArcher.Editor.UGC
{
    /// <summary>
    /// ScriptableObject 与 JSON 双向转换工具
    /// 用于 UGC 换皮场景，让 AI 可以读取和修改配置
    /// 资源引用只保留路径，不使用 GUID（对 AI 更友好）
    /// </summary>
    public class SOJsonConverter : EditorWindow
    {
        private const string JSON_EXPORT_FOLDER = "UGC_Configs";

        private Vector2 scrollPosition;
        private List<string> logMessages = new List<string>();

        [MenuItem("Tools/UGC/SO-JSON Converter")]
        public static void ShowWindow()
        {
            var window = GetWindow<SOJsonConverter>("SO-JSON Converter");
            window.minSize = new Vector2(400, 300);
        }

        private void OnGUI()
        {
            GUILayout.Label("ScriptableObject ↔ JSON 转换工具", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "此工具用于将 ScriptableObject 导出为 JSON 格式，方便 AI 读取和修改。\n" +
                "资源引用会转换为路径格式，修改后可以映射回 ScriptableObject。",
                MessageType.Info);

            GUILayout.Space(10);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("导出所有 SO 到 JSON", GUILayout.Height(40)))
                {
                    ExportAllSOToJson();
                }

                if (GUILayout.Button("从 JSON 导入回 SO", GUILayout.Height(40)))
                {
                    ImportAllJsonToSO();
                }
            }

            GUILayout.Space(10);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("导出选中的 SO"))
                {
                    ExportSelectedSO();
                }

                if (GUILayout.Button("打开导出目录"))
                {
                    OpenExportFolder();
                }
            }

            GUILayout.Space(10);
            GUILayout.Label("日志:", EditorStyles.boldLabel);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(150));
            foreach (var msg in logMessages)
            {
                EditorGUILayout.LabelField(msg, EditorStyles.wordWrappedLabel);
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("清除日志"))
            {
                logMessages.Clear();
            }
        }

        private void Log(string message)
        {
            logMessages.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            Debug.Log($"[SOJsonConverter] {message}");
            Repaint();
        }

        private string GetExportPath()
        {
            return Path.Combine(Application.dataPath, JSON_EXPORT_FOLDER);
        }

        private void OpenExportFolder()
        {
            var path = GetExportPath();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            EditorUtility.RevealInFinder(path);
        }

        #region Export SO to JSON

        private void ExportAllSOToJson()
        {
            var exportPath = GetExportPath();
            if (!Directory.Exists(exportPath))
            {
                Directory.CreateDirectory(exportPath);
            }

            // 查找 The Archer/Scriptables 目录下的所有 SO
            var scriptablesPath = "Assets/The Archer/Scriptables";
            var guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { scriptablesPath });

            Log($"找到 {guids.Length} 个 ScriptableObject");

            int successCount = 0;

            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);

                if (so != null)
                {
                    try
                    {
                        var jsonData = ConvertSOToJson(so, assetPath);
                        var fileName = Path.GetFileNameWithoutExtension(assetPath) + ".json";
                        var jsonPath = Path.Combine(exportPath, fileName);

                        File.WriteAllText(jsonPath, jsonData, Encoding.UTF8);
                        Log($"导出成功: {fileName}");
                        successCount++;
                    }
                    catch (Exception e)
                    {
                        Log($"导出失败 {assetPath}: {e.Message}");
                    }
                }
            }

            Log($"导出完成! 成功: {successCount}/{guids.Length}");
            AssetDatabase.Refresh();
        }

        private void ExportSelectedSO()
        {
            var selected = Selection.activeObject as ScriptableObject;
            if (selected == null)
            {
                Log("请先选择一个 ScriptableObject");
                return;
            }

            var exportPath = GetExportPath();
            if (!Directory.Exists(exportPath))
            {
                Directory.CreateDirectory(exportPath);
            }

            var assetPath = AssetDatabase.GetAssetPath(selected);

            try
            {
                var jsonData = ConvertSOToJson(selected, assetPath);
                var fileName = Path.GetFileNameWithoutExtension(assetPath) + ".json";
                var jsonPath = Path.Combine(exportPath, fileName);

                File.WriteAllText(jsonPath, jsonData, Encoding.UTF8);

                Log($"导出成功: {fileName}");
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Log($"导出失败: {e.Message}");
            }
        }

        private string ConvertSOToJson(ScriptableObject so, string assetPath)
        {
            // 直接解析 .asset 文件获取完整数据（包括代码中未定义但序列化存在的字段）
            var dataDict = new Dictionary<string, object>();
            dataDict["assetPath"] = assetPath;

            // 读取 .asset 文件内容
            var fullPath = Path.Combine(Application.dataPath.Replace("/Assets", ""), assetPath);
            if (File.Exists(fullPath))
            {
                var yamlContent = File.ReadAllText(fullPath);
                var yamlData = ParseUnityYamlComplete(yamlContent);

                // 合并解析结果到 dataDict
                foreach (var kvp in yamlData)
                {
                    dataDict[kvp.Key] = kvp.Value;
                }
            }
            else
            {
                // 如果无法读取文件，回退到反射方式
                SerializeObject(so, dataDict);
            }

            return JsonHelper.ToJson(dataDict, true);
        }

        /// <summary>
        /// 完整解析 Unity YAML 格式的 .asset 文件
        /// 使用递归下降解析，正确处理所有嵌套结构
        /// </summary>
        private Dictionary<string, object> ParseUnityYamlComplete(string yamlContent)
        {
            var result = new Dictionary<string, object>();
            var lines = yamlContent.Split(new[] { '\n' }, StringSplitOptions.None);

            // 找到 MonoBehaviour 部分的起始行
            int startLine = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim() == "MonoBehaviour:")
                {
                    startLine = i + 1;
                    break;
                }
            }

            if (startLine < 0 || startLine >= lines.Length)
                return result;

            // 解析 MonoBehaviour 下的所有内容
            int currentLine = startLine;
            int baseIndent = GetIndent(lines[startLine]);

            ParseYamlBlock(lines, ref currentLine, baseIndent, result);

            return result;
        }

        /// <summary>
        /// 递归解析 YAML 块
        /// </summary>
        private void ParseYamlBlock(string[] lines, ref int lineIndex, int expectedIndent, Dictionary<string, object> target)
        {
            while (lineIndex < lines.Length)
            {
                var line = lines[lineIndex];
                var trimmedLine = line.TrimEnd('\r');

                // 跳过空行
                if (string.IsNullOrWhiteSpace(trimmedLine))
                {
                    lineIndex++;
                    continue;
                }

                int currentIndent = GetIndent(trimmedLine);

                // 如果缩进小于预期，说明当前块结束
                if (currentIndent < expectedIndent)
                    return;

                // 如果缩进大于预期，跳过（不应该发生）
                if (currentIndent > expectedIndent)
                {
                    lineIndex++;
                    continue;
                }

                var content = trimmedLine.Trim();

                // 如果是数组项，说明当前块结束（数组应该由调用者处理）
                if (content.StartsWith("- "))
                    return;

                // 解析键值对
                int colonIndex = content.IndexOf(':');
                if (colonIndex > 0)
                {
                    var key = content.Substring(0, colonIndex).Trim();
                    var valueStr = content.Substring(colonIndex + 1).Trim();

                    // 跳过 Unity 内部字段
                    if (IsUnityInternalField(key))
                    {
                        lineIndex++;
                        continue;
                    }

                    // 清理字段名（移除 m_ 前缀）
                    var cleanKey = key.StartsWith("m_") ? key.Substring(2) : key;

                    if (string.IsNullOrEmpty(valueStr))
                    {
                        // 值为空，检查下一行是数组还是对象
                        lineIndex++;

                        if (lineIndex < lines.Length)
                        {
                            var nextLine = lines[lineIndex].TrimEnd('\r');

                            // 跳过空行
                            while (lineIndex < lines.Length && string.IsNullOrWhiteSpace(nextLine))
                            {
                                lineIndex++;
                                if (lineIndex < lines.Length)
                                    nextLine = lines[lineIndex].TrimEnd('\r');
                            }

                            if (lineIndex >= lines.Length)
                            {
                                // 文件结束，这是一个空值/空数组
                                target[cleanKey] = new List<object>();
                                continue;
                            }

                            int nextIndent = GetIndent(nextLine);
                            var nextContent = nextLine.Trim();

                            // Unity YAML 中，数组项的缩进可能与父字段相同
                            // 例如：
                            // levels:
                            // - abilityDuration: 5
                            // 这里 "- " 的缩进与 "levels:" 相同
                            if (nextContent.StartsWith("- "))
                            {
                                // 这是一个数组
                                var list = new List<object>();
                                ParseYamlArray(lines, ref lineIndex, nextIndent, list);
                                target[cleanKey] = list;
                            }
                            else if (nextIndent > currentIndent)
                            {
                                // 这是一个嵌套对象
                                var nestedDict = new Dictionary<string, object>();
                                ParseYamlBlock(lines, ref lineIndex, nextIndent, nestedDict);
                                target[cleanKey] = nestedDict;
                            }
                            else
                            {
                                // 下一行缩进不大于当前行且不是数组项，说明这个字段是空数组/空值
                                // 在 Unity 中，空数组通常表示为 "fieldName: " 后面没有内容
                                target[cleanKey] = new List<object>();
                            }
                        }
                        else
                        {
                            // 没有下一行，这是一个空值/空数组
                            target[cleanKey] = new List<object>();
                        }
                    }
                    else
                    {
                        // 有值，直接解析
                        target[cleanKey] = ParseYamlValue(valueStr);
                        lineIndex++;
                    }
                }
                else
                {
                    lineIndex++;
                }
            }
        }

        /// <summary>
        /// 解析 YAML 数组
        /// </summary>
        private void ParseYamlArray(string[] lines, ref int lineIndex, int expectedIndent, List<object> target)
        {
            while (lineIndex < lines.Length)
            {
                var line = lines[lineIndex].TrimEnd('\r');

                // 跳过空行
                if (string.IsNullOrWhiteSpace(line))
                {
                    lineIndex++;
                    continue;
                }

                int currentIndent = GetIndent(line);

                // 如果缩进小于预期，说明数组结束
                if (currentIndent < expectedIndent)
                    return;

                var content = line.Trim();

                // 必须是数组项
                if (!content.StartsWith("- "))
                {
                    // 不是数组项，可能是数组结束
                    return;
                }

                var itemContent = content.Substring(2).Trim();

                // 检查是否是 Unity 资源引用格式 {fileID: xxx, guid: xxx, type: x}
                // 这种情况应该作为简单值处理，而不是对象
                if (itemContent.StartsWith("{") && itemContent.EndsWith("}"))
                {
                    // 这是一个 Unity 资源引用，作为简单值处理
                    target.Add(ParseYamlValue(itemContent));
                    lineIndex++;
                    continue;
                }

                // 检查是否是 "- key: value" 格式（对象数组项）
                int colonIndex = itemContent.IndexOf(':');
                if (colonIndex > 0)
                {
                    // 这是一个对象数组项
                    var itemDict = new Dictionary<string, object>();

                    var key = itemContent.Substring(0, colonIndex).Trim();
                    var valueStr = itemContent.Substring(colonIndex + 1).Trim();

                    if (string.IsNullOrEmpty(valueStr))
                    {
                        // 值为空，检查下一行
                        lineIndex++;

                        if (lineIndex < lines.Length)
                        {
                            var nextLine = lines[lineIndex].TrimEnd('\r');
                            int nextIndent = GetIndent(nextLine);
                            int itemIndent = currentIndent + 2; // 数组项内容的缩进

                            if (nextIndent >= itemIndent)
                            {
                                var nextContent = nextLine.Trim();

                                if (nextContent.StartsWith("- "))
                                {
                                    // 嵌套数组
                                    var nestedList = new List<object>();
                                    ParseYamlArray(lines, ref lineIndex, nextIndent, nestedList);
                                    itemDict[key] = nestedList;
                                }
                                else
                                {
                                    // 嵌套对象
                                    var nestedDict = new Dictionary<string, object>();
                                    ParseYamlBlock(lines, ref lineIndex, nextIndent, nestedDict);
                                    itemDict[key] = nestedDict;
                                }
                            }
                        }
                    }
                    else
                    {
                        itemDict[key] = ParseYamlValue(valueStr);
                        lineIndex++;
                    }

                    // 继续解析同一对象的其他字段（缩进与 "- " 后的内容对齐）
                    // 在 YAML 中，数组项对象的后续字段与第一个字段对齐
                    // 例如：
                    // - abilityDuration: 5
                    //   attackDamageMultiplier: 1.1  <- 这里的缩进是 currentIndent + 2
                    int itemFieldIndent = currentIndent + 2;

                    while (lineIndex < lines.Length)
                    {
                        var nextLine = lines[lineIndex].TrimEnd('\r');
                        if (string.IsNullOrWhiteSpace(nextLine))
                        {
                            lineIndex++;
                            continue;
                        }

                        int nextIndent = GetIndent(nextLine);
                        var nextContent = nextLine.Trim();

                        // 如果缩进回到数组项级别或更低，说明当前对象结束
                        if (nextIndent < itemFieldIndent)
                            break;

                        // 如果是新的数组项（以 "- " 开头且缩进等于数组缩进），说明当前对象结束
                        if (nextContent.StartsWith("- ") && nextIndent == currentIndent)
                            break;

                        // 解析对象的其他字段
                        colonIndex = nextContent.IndexOf(':');
                        if (colonIndex > 0)
                        {
                            var fieldKey = nextContent.Substring(0, colonIndex).Trim();
                            var fieldValue = nextContent.Substring(colonIndex + 1).Trim();

                            if (string.IsNullOrEmpty(fieldValue))
                            {
                                lineIndex++;

                                if (lineIndex < lines.Length)
                                {
                                    var subLine = lines[lineIndex].TrimEnd('\r');
                                    int subIndent = GetIndent(subLine);

                                    if (subIndent > nextIndent)
                                    {
                                        var subContent = subLine.Trim();

                                        if (subContent.StartsWith("- "))
                                        {
                                            var nestedList = new List<object>();
                                            ParseYamlArray(lines, ref lineIndex, subIndent, nestedList);
                                            itemDict[fieldKey] = nestedList;
                                        }
                                        else
                                        {
                                            var nestedDict = new Dictionary<string, object>();
                                            ParseYamlBlock(lines, ref lineIndex, subIndent, nestedDict);
                                            itemDict[fieldKey] = nestedDict;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                itemDict[fieldKey] = ParseYamlValue(fieldValue);
                                lineIndex++;
                            }
                        }
                        else
                        {
                            lineIndex++;
                        }
                    }

                    target.Add(itemDict);
                }
                else if (!string.IsNullOrEmpty(itemContent))
                {
                    // 简单值数组项
                    target.Add(ParseYamlValue(itemContent));
                    lineIndex++;
                }
                else
                {
                    lineIndex++;
                }
            }
        }

        /// <summary>
        /// 获取行的缩进级别
        /// </summary>
        private int GetIndent(string line)
        {
            int indent = 0;
            while (indent < line.Length && line[indent] == ' ')
                indent++;
            return indent;
        }

        /// <summary>
        /// 检查是否是 Unity 内部字段（应该跳过）
        /// </summary>
        private bool IsUnityInternalField(string fieldName)
        {
            var internalFields = new HashSet<string>
            {
                "m_ObjectHideFlags",
                "m_CorrespondingSourceObject",
                "m_PrefabInstance",
                "m_PrefabAsset",
                "m_GameObject",
                "m_Enabled",
                "m_EditorHideFlags",
                "m_Script",
                "m_Name",
                "m_EditorClassIdentifier"
            };
            return internalFields.Contains(fieldName);
        }

        /// <summary>
        /// 解析 YAML 值
        /// </summary>
        private object ParseYamlValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            // 处理 YAML 中带引号的字符串（如 "xxx" 或 'xxx'）
            if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                (value.StartsWith("'") && value.EndsWith("'")))
            {
                // 移除首尾引号
                value = value.Substring(1, value.Length - 2);

                // 处理 Unicode 转义序列（如 \u8FC5 -> 迅）
                if (value.Contains("\\u"))
                {
                    value = DecodeUnicodeEscapes(value);
                }

                // 处理其他转义字符
                value = value.Replace("\\n", "\n")
                             .Replace("\\r", "\r")
                             .Replace("\\t", "\t")
                             .Replace("\\\"", "\"")
                             .Replace("\\'", "'")
                             .Replace("\\\\", "\\");

                return value;
            }

            // Unity 资源引用格式: {fileID: xxx, guid: xxx, type: x}
            // 注意：可能有多种格式，如：
            // {fileID: 11400000, guid: 2db7e2a892080ac47b50cd2cf1e498c6, type: 2}
            // {fileID: 21300000, guid: 40110b04b63ce7743a7cb26090657c03, type: 3}
            if (value.StartsWith("{") && value.EndsWith("}"))
            {
                // 如果是 fileID: 0，返回 null（空引用）
                if (value.Contains("fileID: 0"))
                    return null;

                // 提取 GUID 并转换为资源路径
                var guidMatch = System.Text.RegularExpressions.Regex.Match(value, @"guid:\s*([a-fA-F0-9]+)");
                if (guidMatch.Success)
                {
                    var guid = guidMatch.Groups[1].Value;
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    if (!string.IsNullOrEmpty(path))
                        return path;
                    else
                    {
                        // GUID 存在但找不到路径，可能是资源被删除或未导入
                        // 返回一个标记，方便调试
                        return $"[MissingAsset:guid={guid}]";
                    }
                }

                // 没有 GUID 但有 fileID，可能是内部引用
                // 返回 null 而不是原始值
                return null;
            }

            // 检查是否是纯 GUID 字符串（32位十六进制）
            // 例如：e1093f8aabc234e43ab790916cbb9b83
            if (value.Length == 32 && System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-fA-F0-9]{32}$"))
            {
                // 尝试将 GUID 转换为资源路径
                var path = AssetDatabase.GUIDToAssetPath(value);
                if (!string.IsNullOrEmpty(path))
                    return path;
                // 如果找不到对应资源，保留原始 GUID（可能是自定义的唯一标识符）
                // 但为了 UGC 友好，我们标记它
                return $"[GUID:{value}]";
            }

            // 检查是否是十六进制的内部引用（Unity 序列化的整数数组或 fileID 引用）
            // Unity 内部引用格式：
            // - 16位十六进制（单个 fileID）：如 100000002b000000
            // - 8的倍数位十六进制（多个 fileID 数组）：如 31000000320000003300000034000000...
            // 这些都是 Unity 内部的序列化引用，无法转换为有意义的路径
            // 特征：长度 >= 16，长度是 8 的倍数，只包含十六进制字符（包括纯数字的情况）
            // 注意：纯数字如 "31000000320000003300000034000000" 也是有效的十六进制
            if (value.Length >= 16 && value.Length % 8 == 0 &&
                System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-fA-F0-9]+$"))
            {
                // 这是一个内部引用或引用数组，无法转换为路径
                // 返回 null
                return null;
            }

            // 数字（优先检查，因为 0 和 1 也可能是数字而非布尔值）
            if (int.TryParse(value, out int intVal))
                return intVal;
            if (float.TryParse(value, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out float floatVal))
                return floatVal;

            // 处理 Unicode 转义序列（如 \u8FC5 -> 迅）
            if (value.Contains("\\u"))
            {
                value = DecodeUnicodeEscapes(value);
            }

            return value;
        }

        /// <summary>
        /// 解码 Unicode 转义序列
        /// </summary>
        private string DecodeUnicodeEscapes(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return System.Text.RegularExpressions.Regex.Replace(
                input,
                @"\\u([0-9a-fA-F]{4})",
                match => {
                    var hex = match.Groups[1].Value;
                    var codePoint = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
                    return char.ConvertFromUtf32(codePoint);
                }
            );
        }

        private void SerializeObject(object obj, Dictionary<string, object> dict)
        {
            if (obj == null) return;

            var type = obj.GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                // 跳过 Unity 内部字段
                if (field.Name.StartsWith("m_") && field.DeclaringType == typeof(ScriptableObject))
                    continue;
                if (field.Name.StartsWith("m_") && field.DeclaringType == typeof(UnityEngine.Object))
                    continue;

                var value = field.GetValue(obj);
                var serializedValue = SerializeValue(value, field.FieldType);

                // 使用更友好的字段名
                var fieldName = field.Name;
                if (fieldName.StartsWith("m_"))
                    fieldName = fieldName.Substring(2);

                dict[fieldName] = serializedValue;
            }
        }

        private object SerializeValue(object value, Type type)
        {
            if (value == null) return null;

            // Unity Object 引用 -> 只保留路径字符串
            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                var unityObj = value as UnityEngine.Object;
                if (unityObj == null) return null;

                var path = AssetDatabase.GetAssetPath(unityObj);
                // 如果是子资源（如 Sprite），路径可能为空，使用名称作为标识
                if (string.IsNullOrEmpty(path))
                {
                    path = $"[SubAsset]{unityObj.name}";
                }
                return path;  // 直接返回路径字符串
            }

            // 枚举 -> 直接返回数值
            if (type.IsEnum)
            {
                return (int)value;
            }

            // 基础类型直接返回
            if (type.IsPrimitive || type == typeof(string))
            {
                return value;
            }

            // 数组
            if (type.IsArray)
            {
                var array = value as Array;
                var list = new List<object>();
                var elementType = type.GetElementType();

                foreach (var item in array)
                {
                    list.Add(SerializeValue(item, elementType));
                }
                return list;
            }

            // List
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var list = value as System.Collections.IList;
                var result = new List<object>();
                var elementType = type.GetGenericArguments()[0];

                foreach (var item in list)
                {
                    result.Add(SerializeValue(item, elementType));
                }
                return result;
            }

            // 自定义类/结构体
            if (type.IsClass || type.IsValueType)
            {
                var dict = new Dictionary<string, object>();
                SerializeObject(value, dict);
                return dict;
            }

            return value?.ToString();
        }

        #endregion

        #region Import JSON to SO

        private void ImportAllJsonToSO()
        {
            var exportPath = GetExportPath();
            if (!Directory.Exists(exportPath))
            {
                Log("导出目录不存在");
                return;
            }

            var jsonFiles = Directory.GetFiles(exportPath, "*.json").ToArray();

            Log($"找到 {jsonFiles.Length} 个 JSON 文件");

            int successCount = 0;
            foreach (var jsonFile in jsonFiles)
            {
                try
                {
                    var jsonContent = File.ReadAllText(jsonFile);
                    ImportJsonToSO(jsonContent);
                    Log($"导入成功: {Path.GetFileName(jsonFile)}");
                    successCount++;
                }
                catch (Exception e)
                {
                    Log($"导入失败 {Path.GetFileName(jsonFile)}: {e.Message}");
                }
            }

            Log($"导入完成! 成功: {successCount}/{jsonFiles.Length}");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void ImportJsonToSO(string jsonContent)
        {
            var data = JsonHelper.FromJson<Dictionary<string, object>>(jsonContent);
            if (data == null || !data.ContainsKey("assetPath"))
            {
                throw new Exception("无效的 JSON 格式，缺少 assetPath 字段");
            }

            var soPath = data["assetPath"]?.ToString();

            // 加载原始 SO
            var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(soPath);
            if (so == null)
            {
                throw new Exception($"找不到原始 SO: {soPath}");
            }

            // 应用数据（排除 _soPath 字段）
            Undo.RecordObject(so, "Import JSON to SO");
            DeserializeObject(so, data);
            EditorUtility.SetDirty(so);
        }

        private void DeserializeObject(object target, Dictionary<string, object> data)
        {
            if (target == null || data == null) return;

            var type = target.GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                var fieldName = field.Name;
                var altName = fieldName.StartsWith("m_") ? fieldName.Substring(2) : fieldName;

                object jsonValue = null;
                if (data.ContainsKey(fieldName))
                    jsonValue = data[fieldName];
                else if (data.ContainsKey(altName))
                    jsonValue = data[altName];
                else
                    continue;

                try
                {
                    var deserializedValue = DeserializeValue(jsonValue, field.FieldType);
                    field.SetValue(target, deserializedValue);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"反序列化字段 {fieldName} 失败: {e.Message}");
                }
            }
        }

        private object DeserializeValue(object jsonValue, Type targetType)
        {
            if (jsonValue == null) return null;

            // Unity Object 引用 - 通过路径字符串加载
            if (typeof(UnityEngine.Object).IsAssignableFrom(targetType))
            {
                var path = jsonValue?.ToString();
                if (string.IsNullOrEmpty(path)) return null;

                // 子资源标记，暂不支持修改
                if (path.StartsWith("[SubAsset]")) return null;

                return AssetDatabase.LoadAssetAtPath(path, targetType);
            }

            // 枚举 - 直接从数值转换
            if (targetType.IsEnum)
            {
                return Enum.ToObject(targetType, Convert.ToInt32(jsonValue));
            }

            // 基础类型
            if (targetType == typeof(int)) return Convert.ToInt32(jsonValue);
            if (targetType == typeof(float)) return Convert.ToSingle(jsonValue);
            if (targetType == typeof(double)) return Convert.ToDouble(jsonValue);
            if (targetType == typeof(bool)) return Convert.ToBoolean(jsonValue);
            if (targetType == typeof(string)) return jsonValue?.ToString();
            if (targetType == typeof(long)) return Convert.ToInt64(jsonValue);

            // 数组
            if (targetType.IsArray)
            {
                var elementType = targetType.GetElementType();
                if (jsonValue is List<object> list)
                {
                    var array = Array.CreateInstance(elementType, list.Count);
                    for (int i = 0; i < list.Count; i++)
                    {
                        array.SetValue(DeserializeValue(list[i], elementType), i);
                    }
                    return array;
                }
            }

            // List
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var elementType = targetType.GetGenericArguments()[0];
                var listInstance = Activator.CreateInstance(targetType) as System.Collections.IList;

                if (jsonValue is List<object> list)
                {
                    foreach (var item in list)
                    {
                        listInstance.Add(DeserializeValue(item, elementType));
                    }
                }
                return listInstance;
            }

            // 自定义类/结构体
            if ((targetType.IsClass || targetType.IsValueType) && !targetType.IsPrimitive)
            {
                if (jsonValue is Dictionary<string, object> dict)
                {
                    var instance = Activator.CreateInstance(targetType);
                    DeserializeObject(instance, dict);
                    return instance;
                }
            }

            return jsonValue;
        }

        #endregion
    }

    #region Data Classes

    #endregion

    #region JSON Helper

    /// <summary>
    /// 简单的 JSON 序列化帮助类，支持 Dictionary
    /// </summary>
    public static class JsonHelper
    {
        public static string ToJson(object obj, bool prettyPrint = false)
        {
            var sb = new StringBuilder();
            SerializeValue(obj, sb, 0, prettyPrint);
            return sb.ToString();
        }

        public static T FromJson<T>(string json)
        {
            var index = 0;
            var value = ParseValue(json, ref index);
            return ConvertTo<T>(value);
        }

        private static void SerializeValue(object value, StringBuilder sb, int indent, bool prettyPrint)
        {
            if (value == null)
            {
                sb.Append("null");
                return;
            }

            var type = value.GetType();

            if (value is string str)
            {
                sb.Append('"');
                sb.Append(EscapeString(str));
                sb.Append('"');
            }
            else if (value is bool b)
            {
                sb.Append(b ? "true" : "false");
            }
            else if (type.IsPrimitive || value is decimal)
            {
                sb.Append(Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture));
            }
            else if (value is Dictionary<string, object> dict)
            {
                SerializeDictionary(dict, sb, indent, prettyPrint);
            }
            else if (value is System.Collections.IList list)
            {
                SerializeList(list, sb, indent, prettyPrint);
            }
            else if (type.IsClass || type.IsValueType)
            {
                // 序列化对象为字典
                var objDict = new Dictionary<string, object>();
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    objDict[field.Name] = field.GetValue(value);
                }
                SerializeDictionary(objDict, sb, indent, prettyPrint);
            }
        }

        private static void SerializeDictionary(Dictionary<string, object> dict, StringBuilder sb, int indent, bool prettyPrint)
        {
            sb.Append('{');
            if (prettyPrint) sb.AppendLine();

            var first = true;
            foreach (var kvp in dict)
            {
                if (!first)
                {
                    sb.Append(',');
                    if (prettyPrint) sb.AppendLine();
                }
                first = false;

                if (prettyPrint) sb.Append(new string(' ', (indent + 1) * 2));
                sb.Append('"');
                sb.Append(EscapeString(kvp.Key));
                sb.Append("\": ");
                SerializeValue(kvp.Value, sb, indent + 1, prettyPrint);
            }

            if (prettyPrint)
            {
                sb.AppendLine();
                sb.Append(new string(' ', indent * 2));
            }
            sb.Append('}');
        }

        private static void SerializeList(System.Collections.IList list, StringBuilder sb, int indent, bool prettyPrint)
        {
            sb.Append('[');
            if (prettyPrint && list.Count > 0) sb.AppendLine();

            for (int i = 0; i < list.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(',');
                    if (prettyPrint) sb.AppendLine();
                }

                if (prettyPrint) sb.Append(new string(' ', (indent + 1) * 2));
                SerializeValue(list[i], sb, indent + 1, prettyPrint);
            }

            if (prettyPrint && list.Count > 0)
            {
                sb.AppendLine();
                sb.Append(new string(' ', indent * 2));
            }
            sb.Append(']');
        }

        private static string EscapeString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            var sb = new StringBuilder();
            foreach (char c in str)
            {
                switch (c)
                {
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '"':
                        sb.Append("\\\"");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        // 保留所有字符（包括中文等 Unicode 字符），不进行转义
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
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

            throw new Exception($"Unexpected character at position {index}: {c}");
        }

        private static void SkipWhitespace(string json, ref int index)
        {
            while (index < json.Length && char.IsWhiteSpace(json[index]))
                index++;
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
                if (c == '\\')
                {
                    index++;
                    if (index < json.Length)
                    {
                        char escaped = json[index];
                        switch (escaped)
                        {
                            case 'n': sb.Append('\n'); break;
                            case 'r': sb.Append('\r'); break;
                            case 't': sb.Append('\t'); break;
                            case '"': sb.Append('"'); break;
                            case '\\': sb.Append('\\'); break;
                            default: sb.Append(escaped); break;
                        }
                    }
                }
                else
                {
                    sb.Append(c);
                }
                index++;
            }

            throw new Exception("Unterminated string");
        }

        private static Dictionary<string, object> ParseObject(string json, ref int index)
        {
            var dict = new Dictionary<string, object>();
            index++; // Skip opening brace

            SkipWhitespace(json, ref index);
            if (json[index] == '}')
            {
                index++;
                return dict;
            }

            while (true)
            {
                SkipWhitespace(json, ref index);

                // Parse key
                if (json[index] != '"')
                    throw new Exception($"Expected string key at position {index}");
                var key = ParseString(json, ref index);

                SkipWhitespace(json, ref index);
                if (json[index] != ':')
                    throw new Exception($"Expected ':' at position {index}");
                index++;

                // Parse value
                var value = ParseValue(json, ref index);
                dict[key] = value;

                SkipWhitespace(json, ref index);
                if (json[index] == '}')
                {
                    index++;
                    return dict;
                }
                if (json[index] != ',')
                    throw new Exception($"Expected ',' or '}}' at position {index}");
                index++;
            }
        }

        private static List<object> ParseArray(string json, ref int index)
        {
            var list = new List<object>();
            index++; // Skip opening bracket

            SkipWhitespace(json, ref index);
            if (json[index] == ']')
            {
                index++;
                return list;
            }

            while (true)
            {
                var value = ParseValue(json, ref index);
                list.Add(value);

                SkipWhitespace(json, ref index);
                if (json[index] == ']')
                {
                    index++;
                    return list;
                }
                if (json[index] != ',')
                    throw new Exception($"Expected ',' or ']' at position {index}");
                index++;
            }
        }

        private static bool ParseBool(string json, ref int index)
        {
            if (json.Substring(index, 4) == "true")
            {
                index += 4;
                return true;
            }
            if (json.Substring(index, 5) == "false")
            {
                index += 5;
                return false;
            }
            throw new Exception($"Invalid boolean at position {index}");
        }

        private static object ParseNull(string json, ref int index)
        {
            if (json.Substring(index, 4) == "null")
            {
                index += 4;
                return null;
            }
            throw new Exception($"Invalid null at position {index}");
        }

        private static object ParseNumber(string json, ref int index)
        {
            var start = index;
            if (json[index] == '-') index++;

            while (index < json.Length && (char.IsDigit(json[index]) || json[index] == '.' || json[index] == 'e' || json[index] == 'E' || json[index] == '+' || json[index] == '-'))
            {
                if (json[index] == '.' || json[index] == 'e' || json[index] == 'E')
                {
                    index++;
                    while (index < json.Length && (char.IsDigit(json[index]) || json[index] == '+' || json[index] == '-'))
                        index++;
                    return double.Parse(json.Substring(start, index - start), System.Globalization.CultureInfo.InvariantCulture);
                }
                index++;
            }

            var numStr = json.Substring(start, index - start);
            if (long.TryParse(numStr, out long longVal))
                return longVal;
            return double.Parse(numStr, System.Globalization.CultureInfo.InvariantCulture);
        }

        private static T ConvertTo<T>(object value)
        {
            if (value == null) return default;
            if (value is T t) return t;

            var targetType = typeof(T);

            if (value is Dictionary<string, object> dict)
            {
                var instance = Activator.CreateInstance<T>();
                var fields = targetType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    if (dict.ContainsKey(field.Name))
                    {
                        var fieldValue = dict[field.Name];
                        if (fieldValue is Dictionary<string, object> subDict)
                        {
                            var method = typeof(JsonHelper).GetMethod("ConvertTo", BindingFlags.NonPublic | BindingFlags.Static);
                            var generic = method.MakeGenericMethod(field.FieldType);
                            field.SetValue(instance, generic.Invoke(null, new[] { subDict }));
                        }
                        else
                        {
                            field.SetValue(instance, fieldValue);
                        }
                    }
                }
                return instance;
            }

            return (T)Convert.ChangeType(value, targetType);
        }
    }

    #endregion
}
