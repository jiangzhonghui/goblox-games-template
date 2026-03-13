namespace OctoberStudio.Abilities
{
    public class MysticCoinsAbilityBehavior : AbilityBehavior<MysticCoinsAbilityData, MysticCoinsAbilityLevel>
    {
        protected override void SetAbilityLevel(int levelId)
        {
            base.SetAbilityLevel(levelId);
            GameController.TempGold.Deposit(AbilityLevel.AmountOfCoins);
        }
    }
}