using UnityEngine;

namespace OctoberStudio.Currency
{
    public class CurrenciesManager : MonoBehaviour, ICurrenciesManager
    {
        [SerializeField] CurrenciesDatabase database;

        private void Awake()
        {
            if (!GameController.RegisterCurrenciesManager(this))
            {
                Destroy(gameObject);
            }
        }

        public Sprite GetIcon(string currencyId)
        {
            var data = database.GetCurrency(currencyId);

            if(data == null) return null;

            return data.Icon;
        }

        public string GetName(string currencyId)
        {
            var data = database.GetCurrency(currencyId);

            if (data == null) return null;

            return data.Name;
        }
    }
}