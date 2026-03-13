========================================
        UGC 配置热更目录
========================================

使用方法：
1. 在 Unity 编辑器中运行 "Tools > UGC > SO-JSON Converter"
2. 点击 "导出所有 SO 到 JSON"
3. 将导出的 JSON 文件复制到此目录
4. 游戏启动时会自动加载并覆盖 SO 的属性值

JSON 格式要求：
- 必须包含 "assetPath" 字段，指向原始 SO 文件路径
- 只需要包含你想修改的字段

示例 JSON：
{
  "assetPath": "Assets/The Archer/Scriptables/Abilities/xxx.asset",
  "title": "新技能名称",
  "description": "新技能描述",
  "levels": [
    {
      "damage": 100,
      "cooldown": 5.0
    }
  ]
}

Android 平台注意：
- 需要在 manifest.txt 中列出所有 JSON 文件名
- 每行一个文件名

========================================
