using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Common;
using Tanttinator.ModularUI;
using UnityEngine.UI;

namespace Client
{
    public class CityProductionUI : MonoBehaviour
    {

        [SerializeField] TMP_Text title = default;

        [SerializeField] Toggle noProduction = default;

        [SerializeField] Container units = default;
        [SerializeField] Container turns = default;
        [SerializeField] Container counts = default;

        [SerializeField] GameObject productionToggle = default;
        [SerializeField] GameObject tableCell = default;

        [SerializeField] Hidable hidable = default;

        static CityData city;

        static UnitType selectedUnit;
        static Dictionary<UnitType, int> productionCounts;

        static Dictionary<UnitType, ProductionSelectionToggleUI> toggles = new Dictionary<UnitType, ProductionSelectionToggleUI>();
        static Dictionary<UnitType, TMP_Text> countTexts = new Dictionary<UnitType, TMP_Text>();

        static CityProductionUI instance;

        public static void Show(CityData city)
        {
            CityProductionUI.city = city;

            instance.units.Clear();
            instance.turns.Clear();
            instance.counts.Clear();

            instance.title.text = city.name + "\nProduction";

            selectedUnit = city.production;
            productionCounts = ClientController.gameState.GetPlayer(city.owner).production;

            toggles.Clear();
            countTexts.Clear();

            foreach (UnitType unit in ClientController.unitTypes) AddUnit(city, unit, ClientController.gameState.GetPlayer(city.owner).color);

            instance.noProduction.onValueChanged.RemoveAllListeners();
            instance.noProduction.interactable = instance.noProduction.isOn = selectedUnit == null;
            instance.noProduction.onValueChanged.AddListener((value) => { if (value) { ToggleUnit(null, value); }; instance.noProduction.interactable = !value; });

            instance.hidable.Show();
        }

        static void AddUnit(CityData city, UnitType unit, Color color)
        {
            ProductionSelectionToggleUI toggle = instance.units.Add(unit, instance.productionToggle).GetComponent<ProductionSelectionToggleUI>();
            toggle.Setup(unit, color, selectedUnit == unit, (b) => ToggleUnit(unit, b));

            instance.turns.Add(unit, instance.tableCell).GetComponent<TMP_Text>().text = Mathf.CeilToInt(unit.productionCost * 1f / city.efficiency).ToString();

            TMP_Text count = instance.counts.Add(unit, instance.tableCell).GetComponent<TMP_Text>();
            count.text = productionCounts[unit].ToString();

            toggles[unit] = toggle;
            countTexts[unit] = count;
        }

        static void ToggleUnit(UnitType unit, bool value)
        {
            if (value)
            {
                if (selectedUnit == null) instance.noProduction.isOn = false;
                else
                {
                    toggles[selectedUnit].SetValue(false);
                    countTexts[selectedUnit].text = (--productionCounts[selectedUnit]).ToString();
                }
                if (unit != null) countTexts[unit].text = (++productionCounts[unit]).ToString();
                selectedUnit = unit;
            }
            else if (unit != null) instance.noProduction.isOn = true;
        }

        public void Confirm()
        {
            if (city == null) return;
            city.production = selectedUnit;
            CommunicationController.SetProduction(city.ID, selectedUnit);
        }

        private void Awake()
        {
            instance = this;
        }
    }
}
