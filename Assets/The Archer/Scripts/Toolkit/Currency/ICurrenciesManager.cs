using UnityEngine;

namespace OctoberStudio.Currency
{
    public interface ICurrenciesManager
    {
        Sprite GetIcon(string currencyId);
        string GetName(string currencyId);
    }
}