using UnityEngine;
using UnityEditor;

namespace OctoberStudio.Save
{
    public static class SaveActionsMenu
    {
        [MenuItem("Tools/October/Delete Save File", priority = 3)]
        private static void DeleteSaveFile()
        {
            PlayerPrefs.DeleteAll();
            SaveManager.DeleteSaveFile();
        }

        [MenuItem("Tools/October/Delete Save File", true)]
        private static bool DeleteSaveFileValidation()
        {
            return !Application.isPlaying;
        }

        [MenuItem("Tools/October/Open All Stages", priority = 2)]
        private static void OpenAllStages()
        {
            var stageSave = GameController.SaveManager.GetSave<StageSave>("Stage");

            string[] guiID = AssetDatabase.FindAssets("t:StageDatabase");

            if (guiID != null)
            {
                var database = AssetDatabase.LoadAssetAtPath<StageDatabase>(AssetDatabase.GUIDToAssetPath(guiID[0]));

                if(database != null)
                {
                    stageSave.SetMaxReachedStageId(database.StagesAmount - 1);

                    EditorApplication.isPlaying = false;
                }
            }
        }

        [MenuItem("Tools/October/Open All Stages", true)]
        private static bool OpenAllStagesValidation()
        {
            return Application.isPlaying;
        }

        [MenuItem("Tools/October/Get 1K Gold", priority = 1)]
        private static void GetGold()
        {
            var gold = GameController.SaveManager.GetSave<CurrencySave>("gold");

            gold.Deposit(1000);
        }

        [MenuItem("Tools/October/Get 1K Gold", true)]
        private static bool GetGoldValidation()
        {
            return Application.isPlaying;
        }

        [MenuItem("Tools/October/Get 10K Gold", priority = 1)]
        private static void GetGoldBig()
        {
            var gold = GameController.SaveManager.GetSave<CurrencySave>("gold");

            gold.Deposit(10000);
        }

        [MenuItem("Tools/October/Get 10K Gold", true)]
        private static bool GetGoldBigValidation()
        {
            return Application.isPlaying;
        }
    }
}