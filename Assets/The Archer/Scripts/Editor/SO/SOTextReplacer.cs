using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace OctoberStudio.Localization.Editor
{
    /// <summary>
    /// 从翻译后的 CSV 读取翻译，直接替换 SO 文件中的文本
    /// </summary>
    public class SOTextReplacer : EditorWindow
    {
        [MenuItem("Tools/Localization/Replace SO Texts from CSV")]
        public static void ShowWindow()
        {
            GetWindow<SOTextReplacer>("替换 SO 文本");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("ScriptableObject 文本替换工具", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "此工具会读取翻译后的 CSV 文件，直接替换 .asset 文件中的 string 字段值。\n" +
                "⚠️ 注意：此操作会直接修改 .asset 文件，建议先备份！",
                MessageType.Warning);

            EditorGUILayout.Space();

            if (GUILayout.Button("选择 CSV 文件并替换", GUILayout.Height(30)))
            {
                ReplaceTexts();
            }
        }

        private void ReplaceTexts()
        {
            // 1. 选择 CSV 文件
            string csvPath = EditorUtility.OpenFilePanel("选择翻译后的 CSV 文件", "", "csv");
            if (string.IsNullOrEmpty(csvPath))
                return;

            // 2. 读取 CSV
            var translations = ReadCSV(csvPath);
            if (translations.Count == 0)
            {
                EditorUtility.DisplayDialog("错误", "CSV 文件为空或格式不正确", "确定");
                return;
            }

            // 3. 确认操作
            bool confirmed = EditorUtility.DisplayDialog("确认替换",
                $"即将替换 {translations.Count} 条文本。\n" +
                "此操作会直接修改 .asset 文件，无法撤销！\n\n" +
                "是否继续？",
                "继续", "取消");

            if (!confirmed)
                return;

            // 4. 备份提示
            bool backup = EditorUtility.DisplayDialog("备份",
                "建议先备份项目！\n\n是否继续？",
                "继续（不备份）", "取消");

            if (!backup)
                return;

            // 5. 执行替换
            int successCount = 0;
            int failCount = 0;

            EditorUtility.DisplayProgressBar("替换文本", "正在处理...", 0f);

            foreach (var translation in translations)
            {
                if (string.IsNullOrEmpty(translation.zh))
                {
                    failCount++;
                    continue;
                }

                EditorUtility.DisplayProgressBar("替换文本",
                    $"正在处理: {Path.GetFileName(translation.assetPath)} ({successCount + failCount + 1}/{translations.Count})",
                    (float)(successCount + failCount) / translations.Count);

                if (ReplaceTextInAsset(translation))
                {
                    successCount++;
                }
                else
                {
                    failCount++;
                }
            }

            EditorUtility.ClearProgressBar();

            // 6. 刷新资源
            AssetDatabase.Refresh();

            // 7. 显示结果
            EditorUtility.DisplayDialog("完成",
                $"替换完成！\n" +
                $"成功: {successCount} 条\n" +
                $"失败: {failCount} 条\n" +
                $"（失败可能是因为字段不存在或格式不匹配）",
                "确定");
        }

        private bool ReplaceTextInAsset(TranslationEntry translation)
        {
            if (!File.Exists(translation.assetPath))
            {
                Debug.LogWarning($"文件不存在: {translation.assetPath}");
                return false;
            }

            try
            {
                // 1. 读取文件内容
                // Unity YAML 资源建议使用 UTF-8（无 BOM）
                var utf8NoBom = new UTF8Encoding(false);
                string content = File.ReadAllText(translation.assetPath, utf8NoBom);
                string originalContent = content;

                // 2. 构建替换模式
                // 匹配：fieldName: value 或 fieldName: 'value' 或 fieldName: "value"
                string pattern = $@"^(\s+{Regex.Escape(translation.fieldName)}:\s+)(.+)$";

                // 3. 替换单行字符串
                content = Regex.Replace(content, pattern, match =>
                {
                    string prefix = match.Groups[1].Value;
                    string oldValue = match.Groups[2].Value.Trim();

                    // 处理多行字符串
                    if (oldValue == "|" || oldValue == ">")
                    {
                        // 多行字符串，需要特殊处理
                        return ReplaceMultiLineString(content, translation.fieldName, translation.zh, match.Index);
                    }

                    // 单行字符串：替换值
                    string newValue = EscapeYAMLString(translation.zh);
                    return prefix + newValue;
                }, RegexOptions.Multiline);

                // 4. 处理多行字符串（如果单行替换失败）
                if (content == originalContent)
                {
                    content = ReplaceMultiLineStringInContent(content, translation.fieldName, translation.zh);
                }

                // 5. 如果内容有变化，保存文件
                if (content != originalContent)
                {
                    File.WriteAllText(translation.assetPath, content, utf8NoBom);
                    return true;
                }

                return false;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"替换失败: {translation.assetPath}, 错误: {e.Message}");
                return false;
            }
        }

        private string ReplaceMultiLineStringInContent(string content, string fieldName, string newValue)
        {
            // 匹配多行字符串：fieldName: | 或 fieldName: >
            string pattern = $@"^(\s+{Regex.Escape(fieldName)}:\s+)([|>])\s*$";
            Match match = Regex.Match(content, pattern, RegexOptions.Multiline);

            if (!match.Success)
                return content;

            int startIndex = match.Index;
            int indentLevel = GetIndentLevel(match.Groups[1].Value);
            string indent = new string(' ', indentLevel + 2);

            // 找到多行字符串的结束位置
            string[] lines = content.Split('\n');
            int startLine = GetLineNumber(content, startIndex);
            int endLine = startLine + 1;

            // 查找结束位置（下一个相同或更小缩进的非空行）
            for (int i = startLine + 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line))
                {
                    endLine = i + 1;
                    continue;
                }

                int lineIndent = GetIndentLevel(line);
                if (lineIndent <= indentLevel)
                {
                    endLine = i;
                    break;
                }

                endLine = i + 1;
            }

            // 构建新的多行字符串
            StringBuilder newContent = new StringBuilder();
            string[] newValueLines = newValue.Split('\n');

            // 添加前面的内容
            for (int i = 0; i < startLine; i++)
            {
                newContent.AppendLine(lines[i]);
            }

            // 添加字段名和开始标记
            newContent.Append($"{new string(' ', indentLevel)}{fieldName}: |\n");

            // 添加多行内容
            foreach (string line in newValueLines)
            {
                newContent.AppendLine($"{indent}{line}");
            }

            // 添加后面的内容
            for (int i = endLine; i < lines.Length; i++)
            {
                newContent.AppendLine(lines[i]);
            }

            return newContent.ToString();
        }

        private string ReplaceMultiLineString(string content, string fieldName, string newValue, int startIndex)
        {
            // 简化处理：直接替换整个多行字符串块
            return ReplaceMultiLineStringInContent(content, fieldName, newValue);
        }

        private int GetLineNumber(string content, int index)
        {
            int line = 0;
            for (int i = 0; i < index && i < content.Length; i++)
            {
                if (content[i] == '\n')
                    line++;
            }
            return line;
        }

        private int GetIndentLevel(string line)
        {
            int indent = 0;
            foreach (char c in line)
            {
                if (c == ' ') indent++;
                else if (c == '\t') indent += 4;
                else break;
            }
            return indent;
        }

        private string EscapeYAMLString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "\"\"";

            // 如果包含特殊字符，需要用引号包裹
            if (value.Contains("\n") || value.Contains("\"") || value.Contains("'") || value.Contains(":"))
            {
                // 转义引号
                value = value.Replace("\"", "\\\"");
                return $"\"{value}\"";
            }

            // 简单字符串直接返回
            return value;
        }

        private List<TranslationEntry> ReadCSV(string path)
        {
            var entries = new List<TranslationEntry>();

            // 重要：很多人会用 Excel 保存 CSV（常见是 GBK/ANSI），这里需要自动识别编码
            string csvText = ReadAllTextWithEncodingFallback(path);
            using (StringReader reader = new StringReader(csvText))
            {
                // 跳过标题行
                string header = reader.ReadLine();

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = ParseCSVLine(line);
                    if (parts.Length >= 5)
                    {
                        entries.Add(new TranslationEntry
                        {
                            key = TrimBom(parts[0]),
                            assetPath = parts[1],
                            fieldName = parts[2],
                            en = parts[3],
                            zh = parts[4]
                        });
                    }
                }
            }

            return entries;
        }

        private static string TrimBom(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return s.TrimStart('\uFEFF').Trim();
        }

        private static string ReadAllTextWithEncodingFallback(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            if (bytes.Length == 0) return string.Empty;

            // BOM 检测
            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            {
                return new UTF8Encoding(true).GetString(bytes);
            }
            if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
            {
                return Encoding.Unicode.GetString(bytes);
            }
            if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
            {
                return Encoding.BigEndianUnicode.GetString(bytes);
            }

            // 先尝试严格 UTF-8（遇到非法字节会抛异常）
            try
            {
                return new UTF8Encoding(false, true).GetString(bytes);
            }
            catch
            {
                // 回退：GB18030/GBK（Excel 常见编码）
                TryRegisterCodePagesProviderViaReflection();
                try
                {
                    return Encoding.GetEncoding("GB18030").GetString(bytes);
                }
                catch
                {
                    try
                    {
                        // 进一步尝试 GBK（CP936）
                        return Encoding.GetEncoding(936).GetString(bytes);
                    }
                    catch
                    {
                        // 最后兜底：系统默认
                        return Encoding.Default.GetString(bytes);
                    }
                }
            }
        }

        /// <summary>
        /// Unity 的不同运行时/配置下，code pages provider 可能不可用或不可直接引用。
        /// 这里用反射方式尝试注册，不影响编译与运行。
        /// </summary>
        private static void TryRegisterCodePagesProviderViaReflection()
        {
            try
            {
                // 尝试获取 CodePagesEncodingProvider 类型
                // 常见程序集名：System.Text.Encoding.CodePages
                var providerType =
                    Type.GetType("System.Text.CodePagesEncodingProvider, System.Text.Encoding.CodePages") ??
                    Type.GetType("System.Text.CodePagesEncodingProvider");

                if (providerType == null) return;

                var instanceProp = providerType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                var instance = instanceProp?.GetValue(null, null);
                if (instance == null) return;

                var registerMethod = typeof(Encoding).GetMethod("RegisterProvider", BindingFlags.Public | BindingFlags.Static);
                registerMethod?.Invoke(null, new[] { instance });
            }
            catch
            {
                // ignore
            }
        }

        private string[] ParseCSVLine(string line)
        {
            var parts = new List<string>();
            bool inQuotes = false;
            StringBuilder current = new StringBuilder();

            foreach (char c in line)
            {
                if (c == '"')
                {
                    if (inQuotes && current.Length > 0 && current[current.Length - 1] == '"')
                    {
                        // 转义的引号 ""
                        current.Append('"');
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    parts.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }
            parts.Add(current.ToString());

            return parts.ToArray();
        }

        [System.Serializable]
        public class TranslationEntry
        {
            public string key;
            public string assetPath;
            public string fieldName;
            public string en;
            public string zh;
        }
    }
}
