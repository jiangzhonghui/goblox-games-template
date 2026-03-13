using OctoberStudio.Audio;
using System.Collections.Generic;
using UnityEngine;

namespace OctoberStudio
{
    [CreateAssetMenu(menuName = "October/Stage/Stage Data", fileName = "Stage Data 001")]
    public class StageData : ScriptableObject
    {
        [SerializeField] protected Sprite stageImage;
        public Sprite StageImage => stageImage;

        [SerializeField] protected string stageName;
        public string StageName => stageName;

        [SerializeField] protected bool showStageObjective = true;
        public bool ShowStageObjective => showStageObjective;

        [SerializeField, TextArea] protected string stageObjective;
        public string StageObjective => stageObjective;

        [SerializeField] protected AudioData stageMusic;
        public AudioData StageMusic => stageMusic;

        [SerializeField] protected bool showAbilitySelector;
        public bool ShowAbilitySelector => showAbilitySelector;

        [Space]
        [SerializeField] protected float enemyDamageMulitplier = 1f;
        [SerializeField] protected float enemyDamageMultiplierRoomStep = 0.1f;
        [SerializeField] protected float enemyDamageMultiplierWaveStep = 0.02f;

        public float EnemyDamageMultiplier => enemyDamageMulitplier;
        public float EnemyDamageMultiplierRoomStep => enemyDamageMultiplierRoomStep;
        public float EnemyDamageMultiplierWaveStep => enemyDamageMultiplierWaveStep;

        [Space]
        [SerializeField] protected float enemyHPMulitplier = 1f;
        [SerializeField] protected float enemyHPMultiplierRoomStep = 0.1f;
        [SerializeField] protected float enemyHPMultiplierWaveStep = 0.02f;

        public float EnemyHPMultiplier => enemyHPMulitplier;
        public float EnemyHPMultiplierRoomStep => enemyHPMultiplierRoomStep;
        public float EnemyHPMultiplierWaveStep => enemyHPMultiplierWaveStep;

        [Header("Rooms")]
        [SerializeField] protected List<RoomData> rooms = new List<RoomData>();
        public List<RoomData> Rooms => rooms;

        public int RoomsCount => rooms.Count;

        public RoomData GetRoom(int index)
        {
            return rooms[index];
        }
    }
}