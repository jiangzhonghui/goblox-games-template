using OctoberStudio.Save;
using System;
using UnityEngine;

namespace OctoberStudio
{
    public class CurrencySave: ISave
    {
        [SerializeField] protected int amount;
        public int Amount => amount;

        public static implicit operator int (CurrencySave currencySave) => currencySave.amount;

        public event Action<int> OnAmountChanged;

        public virtual void Deposit(int depositedAmount)
        {
            amount += depositedAmount;

            OnAmountChanged?.Invoke(amount);
        }

        public virtual void Withdraw(int withdrawnAmount)
        {
            amount -= withdrawnAmount;
            if (amount < 0) amount = 0;

            OnAmountChanged?.Invoke(amount);
        }

        public virtual bool TryWithdraw(int withdrawnAmount)
        {
            var canAfford = CanAfford(withdrawnAmount);

            if(canAfford) 
            {
                amount -= withdrawnAmount;

                OnAmountChanged?.Invoke(amount);
            }

            return canAfford;
        }

        public virtual bool CanAfford(int requiredAmount)
        {
            return amount >= requiredAmount;
        }

        public virtual void Clear()
        {
            amount = 0;
        }

        public virtual void Flush()
        {

        }
    }
}