using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OctoberStudio.UI
{
    public class CostButtonBehavior : MonoBehaviour
    {
        [SerializeField] protected Button button;
        [SerializeField] protected Sprite activeButtonSprite;
        [SerializeField] protected Sprite disabledButtonSprite;

        [Space]
        [SerializeField] protected ScalingLabelBehavior costLabel;

        public RectTransform RectTransform { get; protected set; }
        public Selectable Selectable => button;

        public bool IsActive { get; protected set; }
        public bool ButtonEnabled { get => button.interactable; set => button.interactable = value; }

        public bool HasEnoughMoney => GameController.Gold.Amount >= costLabel.Amount;

        protected virtual void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }

        protected virtual void OnEnable()
        {
            GameController.Gold.OnAmountChanged += OnCurrencyAmountChanged;
        }

        protected virtual void OnDisable()
        {
            GameController.Gold.OnAmountChanged -= OnCurrencyAmountChanged;
        }

        public virtual bool WithdrawCost()
        {
            if(GameController.Gold.Amount >= costLabel.Amount)
            {
                GameController.Gold.Withdraw(costLabel.Amount);
                return true;
            }

            return false;
        }

        public virtual void SetOnClick(UnityAction onClick)
        {
            button.onClick.AddListener(onClick);
        }

        public virtual void SetCost(int cost)
        {
            costLabel.SetAmount(cost);
            RecalculateVisuals();
        }

        protected virtual void OnCurrencyAmountChanged(int amount)
        {
            RecalculateVisuals();
        }

        protected virtual void RecalculateVisuals()
        {
            button.image.sprite = GameController.Gold.Amount >= costLabel.Amount ? activeButtonSprite : disabledButtonSprite;
        }

        
    }
}