using UnityEngine;
using UnityEngine.Events;

namespace OctoberStudio.Drop
{
    public class HealDropBehavior : DropBehavior
    {
        [SerializeField] protected Float hpProportion = (0.3f, 0.8f);

        public static event UnityAction<HealDropBehavior> onHealPickedUp;

        public override void OnPickedUp()
        {
            base.OnPickedUp();

            StageController.Player.HealProportion(hpProportion);

            gameObject.SetActive(false);

            onHealPickedUp?.Invoke(this);
        }
    }
}