using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace OctoberStudio.Localization.Editor
{
    /// <summary>
    /// 提取所有 ScriptableObject 中的 string 字段，生成 CSV 供翻译
    /// </summary>
    public class SOTextExtractor : EditorWindow
    {
        [MenuItem("Tools/Localization/Extract SO Texts to CSV")]
        public static void ShowWindow()
        {
            GetWindow<SOTextExtractor>("提取 SO 文本");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("ScriptableObject 文本提取工具", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "此工具会扫描 Scriptables 文件夹下的配表文件，提取其中的 string 字段，生成 CSV 文件供翻译。\n" +
                "自动排除：Audio、Effects、Rim 等非配表文件夹。",
                MessageType.Info);

            EditorGUILayout.Space();

            if (GUILayout.Button("开始提取", GUILayout.Height(30)))
            {
                ExtractAllTexts();
            }
        }

        private void ExtractAllTexts()
        {
            var entries = new List<TranslationEntry>();
            var processedFiles = new HashSet<string>();

            // 1. 只查找 Scriptables 文件夹下的 ScriptableObject（配表文件）
            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { "Assets/The Archer/Scriptables" });
            int totalFiles = guids.Length;
            int processedCount = 0;

            EditorUtility.DisplayProgressBar("提取文本", "正在扫描配表文件...", 0f);

            foreach (string guid in guids)
            {
                processedCount++;
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                // 跳过 meta 文件
                if (assetPath.EndsWith(".meta")) continue;

                // 排除非配表文件夹（Audio、Effects 等）
                if (ShouldSkipAsset(assetPath))
                {
                    continue;
                }

                EditorUtility.DisplayProgressBar("提取文本",
                    $"正在处理: {Path.GetFileName(assetPath)} ({processedCount}/{totalFiles})",
                    (float)processedCount / totalFiles);

                // 2. 读取 .asset 文件内容
                string content = File.ReadAllText(assetPath, Encoding.UTF8);

                // 3. 解析 YAML，提取 string 字段
                ExtractStringsFromAsset(content, assetPath, entries, processedFiles);
            }

            EditorUtility.ClearProgressBar();

            // 4. 导出 CSV
            string csvPath = EditorUtility.SaveFilePanel("保存 CSV", "", "so_texts_export", "csv");
            if (!string.IsNullOrEmpty(csvPath))
            {
                ExportToCSV(entries, csvPath);
                EditorUtility.DisplayDialog("完成",
                    $"提取完成！\n共提取 {entries.Count} 条文本\n已保存到: {csvPath}", "确定");

                // 打开 CSV 文件
                System.Diagnostics.Process.Start(csvPath);
            }
        }

        private void ExtractStringsFromAsset(string content, string assetPath,
            List<TranslationEntry> entries, HashSet<string> processedFiles)
        {
            // 匹配 YAML 中的 string 字段
            // 格式：fieldName: value 或 fieldName: 'value' 或 fieldName: "value"
            // 多行字符串格式：fieldName: | 或 fieldName: >

            // 匹配单行字符串：fieldName: value
            var singleLinePattern = @"^(\s+)(\w+):\s+(.+)$";
            // 匹配多行字符串开始：fieldName: | 或 fieldName: >
            var multiLineStartPattern = @"^(\s+)(\w+):\s+[|>]";

            string[] lines = content.Split('\n');
            string currentField = null;
            int currentIndent = 0;
            StringBuilder multiLineValue = new StringBuilder();
            bool inMultiLine = false;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                string trimmedLine = line.Trim();

                // 跳过空行和注释
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("%"))
                    continue;

                // 跳过 Unity 内部字段（m_ 开头的字段）
                if (trimmedLine.StartsWith("m_"))
                    continue;

                // 检查是否是多行字符串的开始
                Match multiLineMatch = Regex.Match(line, multiLineStartPattern);
                if (multiLineMatch.Success)
                {
                    currentField = multiLineMatch.Groups[2].Value;
                    currentIndent = multiLineMatch.Groups[1].Length;
                    inMultiLine = true;
                    multiLineValue.Clear();
                    continue;
                }

                // 处理多行字符串内容
                if (inMultiLine)
                {
                    // 检查是否结束（遇到相同或更小的缩进，且不是字符串内容）
                    int lineIndent = GetIndentLevel(line);
                    if (lineIndent <= currentIndent && !string.IsNullOrWhiteSpace(line))
                    {
                        // 多行字符串结束
                        string value = multiLineValue.ToString().Trim();
                        if (!string.IsNullOrEmpty(value) && IsTextField(currentField))
                        {
                            AddEntry(entries, assetPath, currentField, value);
                        }
                        inMultiLine = false;
                        currentField = null;
                    }
                    else
                    {
                        // 继续收集多行内容
                        multiLineValue.AppendLine(line.Substring(currentIndent + 2));
                    }
                    continue;
                }

                // 匹配单行字符串
                Match match = Regex.Match(line, singleLinePattern);
                if (match.Success)
                {
                    string fieldName = match.Groups[2].Value;
                    string value = match.Groups[3].Value.Trim();

                    // 特殊处理 description 字段的多行字符串（单引号包裹）
                    if (fieldName == "description" && value.StartsWith("'") && !value.EndsWith("'"))
                    {
                        // description 多行字符串开始，收集后续行直到找到结束引号
                        string multiLineDesc = ExtractMultiLineDescription(lines, i, match.Groups[1].Length);
                        if (!string.IsNullOrEmpty(multiLineDesc))
                        {
                            AddEntry(entries, assetPath, fieldName, multiLineDesc);
                        }
                        // 跳过已处理的行
                        i = SkipMultiLineDescription(lines, i);
                        continue;
                    }

                    // 移除引号
                    if (value.StartsWith("'") && value.EndsWith("'"))
                        value = value.Substring(1, value.Length - 2);
                    else if (value.StartsWith("\"") && value.EndsWith("\""))
                        value = value.Substring(1, value.Length - 2);

                    // 处理转义字符
                    value = value.Replace("\\n", "\n").Replace("\\t", "\t");

                    // 检查是否是文本字段
                    if (IsTextField(fieldName) && !string.IsNullOrEmpty(value))
                    {
                        AddEntry(entries, assetPath, fieldName, value);
                    }
                }
            }

            // 处理最后的多行字符串
            if (inMultiLine && multiLineValue.Length > 0)
            {
                string value = multiLineValue.ToString().Trim();
                if (!string.IsNullOrEmpty(value) && IsTextField(currentField))
                {
                    AddEntry(entries, assetPath, currentField, value);
                }
            }
        }

        private string ExtractMultiLineDescription(string[] lines, int startLine, int baseIndent)
        {
            if (startLine >= lines.Length) return "";

            // 提取第一行的内容（去掉开头的单引号）
            string firstLine = lines[startLine];
            int colonIndex = firstLine.IndexOf(':');
            if (colonIndex < 0 || colonIndex >= firstLine.Length - 1) return "";

            string firstValue = firstLine.Substring(colonIndex + 1).Trim();
            if (!firstValue.StartsWith("'") || firstValue.Length < 2) return "";

            StringBuilder result = new StringBuilder();
            // 去掉开头的单引号（安全处理）
            if (firstValue.Length > 1)
            {
                result.Append(firstValue.Substring(1));
            }

            // 继续读取后续行，直到找到结束的单引号
            for (int i = startLine + 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrEmpty(line)) continue;

                int lineIndent = GetIndentLevel(line);

                // 如果缩进小于等于基础缩进，说明已经超出 description 字段范围
                if (lineIndent <= baseIndent && !string.IsNullOrWhiteSpace(line))
                {
                    break;
                }

                // 查找结束引号
                int quoteIndex = line.IndexOf("'");
                if (quoteIndex >= 0)
                {
                    // 找到结束引号，提取到引号前的内容
                    if (quoteIndex > 0)
                    {
                        string endValue = line.Substring(0, quoteIndex).Trim();
                        if (!string.IsNullOrEmpty(endValue))
                        {
                            result.Append("\n").Append(endValue);
                        }
                    }
                    break;
                }
                else
                {
                    // 继续收集内容（去掉缩进，添加边界检查）
                    int contentStart = baseIndent + 2;
                    if (contentStart < line.Length)
                    {
                        string content = line.Substring(contentStart).Trim();
                        if (!string.IsNullOrEmpty(content))
                        {
                            if (result.Length > 0)
                                result.Append("\n");
                            result.Append(content);
                        }
                    }
                    else if (line.Trim().Length == 0)
                    {
                        // 空行，保留换行
                        result.Append("\n");
                    }
                }
            }

            return result.ToString().Trim();
        }

        private int SkipMultiLineDescription(string[] lines, int startLine)
        {
            string firstLine = lines[startLine];
            int baseIndent = GetIndentLevel(firstLine);

            // 跳过后续行，直到找到结束引号或遇到下一个字段
            for (int i = startLine + 1; i < lines.Length; i++)
            {
                string line = lines[i];
                int lineIndent = GetIndentLevel(line);

                // 如果缩进小于等于基础缩进，说明已经超出 description 字段范围
                if (lineIndent <= baseIndent && !string.IsNullOrWhiteSpace(line))
                {
                    return i - 1; // 返回上一行，让外层循环继续处理
                }

                // 找到结束引号
                if (line.Contains("'"))
                {
                    return i; // 返回当前行，让外层循环继续处理下一行
                }
            }

            return lines.Length - 1;
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

        private bool ShouldSkipAsset(string assetPath)
        {
            // 排除非配表文件夹
            string lowerPath = assetPath.ToLower();

            // 排除 Audio 文件夹（音频数据不是配表）
            if (lowerPath.Contains("/audio/"))
                return true;

            // 排除 Effects 文件夹（特效数据不是配表）
            if (lowerPath.Contains("/effects/"))
                return true;

            // 排除 Rim 文件夹（边缘效果数据不是配表）
            if (lowerPath.Contains("/rim/"))
                return true;

            // 可以添加更多排除规则
            // if (lowerPath.Contains("/other/"))
            //     return true;

            return false;
        }

        private bool IsTextField(string fieldName)
        {
            // 排除 Unity 内部字段（以 m_ 开头的字段）
            if (fieldName.StartsWith("m_"))
                return false;

            // 排除明确的非文本字段（数值、引用等）
            string lowerName = fieldName.ToLower();
            if (lowerName == "guid" ||
                lowerName == "fileid" ||
                lowerName == "type" ||
                lowerName == "cost" ||
                lowerName == "value" ||
                lowerName == "level" ||
                lowerName == "enabled" ||
                lowerName == "script" ||
                lowerName == "icon" ||
                lowerName == "prefab" ||
                lowerName == "sprite" ||
                lowerName == "audio" ||
                lowerName == "multiplier" ||
                lowerName == "step" ||
                lowerName == "initialvalue" ||
                lowerName == "devstartlevel" ||
                lowerName == "upgradetype" ||
                lowerName == "abilitytype" ||
                lowerName == "rarity" ||
                lowerName == "enabledineditor" ||
                lowerName == "enabledinbuild")
                return false;

            // 判断是否是文本字段（可以根据需要扩展）
            return lowerName.Contains("title") ||
                   (lowerName.Contains("name") && !lowerName.StartsWith("m_")) || // name 字段但要排除 m_name
                   lowerName.Contains("description") ||
                   lowerName.Contains("desc") ||
                   lowerName.Contains("text") ||
                   lowerName.Contains("objective") ||
                   lowerName.Contains("message") ||
                   lowerName.Contains("label") ||
                   lowerName.Contains("itemname") ||
                   lowerName.Contains("stagename") ||
                   lowerName.Contains("heroname") ||
                   lowerName.Contains("weaponname");
        }

        private void AddEntry(List<TranslationEntry> entries, string assetPath,
            string fieldName, string value)
        {
            // 生成唯一 key：文件名_字段名
            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            string key = $"{fileName}_{fieldName}";

            // 检查是否已存在（避免重复）
            if (entries.Exists(e => e.key == key))
                return;

            entries.Add(new TranslationEntry
            {
                key = key,
                assetPath = assetPath,
                fieldName = fieldName,
                en = value, // 原始英文
                zh = "" // 待翻译
            });
        }

        private void ExportToCSV(List<TranslationEntry> entries, string path)
        {
            using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                // 写入 CSV 头（BOM 确保 Excel 正确显示中文）
                writer.Write('\uFEFF'); // UTF-8 BOM
                writer.WriteLine("key,assetPath,fieldName,en,zh");

                // 写入数据
                foreach (var entry in entries)
                {
                    // 转义 CSV 特殊字符
                    string key = EscapeCSV(entry.key);
                    string assetPath = EscapeCSV(entry.assetPath);
                    string fieldName = EscapeCSV(entry.fieldName);
                    string en = EscapeCSV(entry.en);
                    string zh = EscapeCSV(entry.zh);

                    writer.WriteLine($"{key},{assetPath},{fieldName},{en},{zh}");
                }
            }
        }

        private string EscapeCSV(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            // 如果包含逗号、引号或换行符，需要用引号包裹
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                // 转义引号：将 " 替换为 ""
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }

            return value;
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
