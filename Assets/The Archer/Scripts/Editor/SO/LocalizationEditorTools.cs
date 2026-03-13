using UnityEditor;

namespace OctoberStudio.Localization.Editor
{
    /// <summary>
    /// 本地化工具菜单入口
    /// </summary>
    public static class LocalizationEditorTools
    {
        [MenuItem("Tools/Localization/1. Extract SO Texts to CSV", priority = 1)]
        public static void ExtractTexts()
        {
            SOTextExtractor.ShowWindow();
        }

        [MenuItem("Tools/Localization/2. Replace SO Texts from CSV", priority = 2)]
        public static void ReplaceTexts()
        {
            SOTextReplacer.ShowWindow();
        }

        [MenuItem("Tools/Localization/3. Complete Workflow (Extract → Translate → Replace)", priority = 3)]
        public static void CompleteWorkflow()
        {
            // 1. 提取文本
            bool extract = EditorUtility.DisplayDialog("步骤 1/3: 提取文本",
                "即将提取所有 SO 文件中的文本到 CSV。\n\n点击确定继续。",
                "提取", "取消");

            if (extract)
            {
                SOTextExtractor.ShowWindow();
            }

            // 2. 提示翻译
            EditorUtility.DisplayDialog("步骤 2/3: 翻译",
                "请使用 ChatGPT 或其他工具翻译 CSV 文件中的文本。\n" +
                "翻译完成后，点击确定继续下一步。",
                "确定");

            // 3. 替换文本
            bool replace = EditorUtility.DisplayDialog("步骤 3/3: 替换文本",
                "即将用翻译后的文本替换 SO 文件。\n\n⚠️ 此操作会直接修改文件，建议先备份！",
                "继续", "取消");

            if (replace)
            {
                SOTextReplacer.ShowWindow();
            }
        }
    }
}
