namespace OctoberStudio.Abilities
{
    public class LegendaryCoinsAbilityBehavior : AbilityBehavior<LegendaryCoinsAbilityData, LegendaryCoinsAbilityLevel>
    {
        protected override void SetAbilityLevel(int levelId)
        {
            base.SetAbilityLevel(levelId);
            GameController.TempGold.Deposit(AbilityLevel.AmountOfCoins);
        }
    }
}