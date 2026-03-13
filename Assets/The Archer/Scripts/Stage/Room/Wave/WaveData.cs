using OctoberStudio.Audio;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace OctoberStudio
{
    [System.Serializable]
    public class WaveData
    {
        [SerializeField] protected List<EnemySpawnData> enemySpawns = new List<EnemySpawnData>();
        public List<EnemySpawnData> EnemySpawns => enemySpawns;

        [SerializeField] protected List<ChestSpawnData> chestSpawns = new List<ChestSpawnData>();
        public List<ChestSpawnData> ChestSpawns => chestSpawns;

        [SerializeField] protected int groupId = 0;
        public int GroupId => groupId;

        [SerializeField] protected float waveDropExpericence;
        public float WaveDropExperience => waveDropExpericence;

        [SerializeField] protected int waveDropGold;
        public int WaveDropGold => waveDropGold;

        [SerializeField] protected float customWaveEndDelay = -1;
        public float CustomWaveEndDelay => customWaveEndDelay;

        [SerializeField] protected Sprite icon;
        public Sprite Icon => icon;

        [SerializeField] protected AudioData waveMusic;
        public AudioData WaveMusic => waveMusic;

#if UNITY_EDITOR

        public void SaveToSerializedProperty(SerializedProperty property)
        {
            var enemySpawnsProperty = property.FindPropertyRelative("enemySpawns");
            enemySpawnsProperty.arraySize = enemySpawns.Count;
            for (int i = 0; i < enemySpawns.Count; i++)
            {
                enemySpawns[i].SaveToSerializedProperty(enemySpawnsProperty.GetArrayElementAtIndex(i));
            }

            var chestSpawnsProperty = property.FindPropertyRelative("chestSpawns");
            chestSpawnsProperty.arraySize = chestSpawns.Count;
            for (int i = 0; i < chestSpawns.Count; i++)
            {
                chestSpawns[i].SaveToSerializedProperty(chestSpawnsProperty.GetArrayElementAtIndex(i));
            }

            var groupIdProperty = property.FindPropertyRelative("groupId");
            groupIdProperty.intValue = groupId;

            var waveDropExpericenceProperty = property.FindPropertyRelative("waveDropExpericence");
            waveDropExpericenceProperty.floatValue = waveDropExpericence;

            var waveDropGoldProperty = property.FindPropertyRelative("waveDropGold");
            waveDropGoldProperty.intValue = waveDropGold;

            var customWaveEndDelayProperty = property.FindPropertyRelative("customWaveEndDelay");
            customWaveEndDelayProperty.floatValue = customWaveEndDelay;

            var iconProperty = property.FindPropertyRelative("icon");
            iconProperty.objectReferenceValue = icon;

            var waveMusicProperty = property.FindPropertyRelative("waveMusic");
            waveMusicProperty.objectReferenceValue = waveMusic;
        }
#endif
    }
}