namespace OctoberStudio.Abilities
{
    public class CommonCoinsAbilityBehavior : AbilityBehavior<CommonCoinsAbilityData, CommonCoinsAbilityLevel>
    {
        protected override void SetAbilityLevel(int levelId)
        {
            base.SetAbilityLevel(levelId);

            GameController.TempGold.Deposit(AbilityLevel.AmountOfCoins);
        }
    }
}