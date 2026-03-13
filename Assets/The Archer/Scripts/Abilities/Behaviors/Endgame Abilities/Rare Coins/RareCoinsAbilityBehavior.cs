namespace OctoberStudio.Abilities
{
    public class RareCoinsAbilityBehavior : AbilityBehavior<RareCoinsAbilityData, RareCoinsAbilityLevel>
    {
        protected override void SetAbilityLevel(int levelId)
        {
            base.SetAbilityLevel(levelId);
            GameController.TempGold.Deposit(AbilityLevel.AmountOfCoins);
        }
    }
}