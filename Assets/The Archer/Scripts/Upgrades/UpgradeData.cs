using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OctoberStudio.Upgrades
{
    [CreateAssetMenu(fileName = "Upgrades", menuName = "October/Upgrades/Upgrade")]
    public class UpgradeData : ScriptableObject
    {
        [SerializeField] protected UpgradeType upgradeType;
        [SerializeField] protected Sprite icon;
        [SerializeField] protected string title;
        [SerializeField, TextArea] protected string description;
        [SerializeField] protected string statsText;
        [SerializeField, Tooltip("What to show in the description as the value when upgraid isn't bought yet")] protected float initialValue = 0f;
        [SerializeField] protected GameObject prefab;
        [SerializeField] protected int devStartLevel = 0;

        public UpgradeType UpgradeType => upgradeType;
        public Sprite Icon => icon;
        public string Title => title;
        public string Description => description;
        public string StatsText => statsText;
        public float InitialValue => initialValue;
        public GameObject Prefab => prefab;

        public int DevStartLevel => devStartLevel;

        [SerializeField] protected List<UpgradeLevel> levels;

        public int LevelsCount => levels.Count;

        public UpgradeLevel GetLevel(int id)
        {
            return levels[id];
        }

        public event UnityAction<int> onUpgradeLevelChanged;

        public virtual void SetLevel(int level)
        {
            onUpgradeLevelChanged?.Invoke(level);
        }
    }

    [System.Serializable]
    public class UpgradeLevel
    {
        [SerializeField] int cost;
        [SerializeField] float value;

        public int Cost => cost;
        public float Value => value;
    }
}