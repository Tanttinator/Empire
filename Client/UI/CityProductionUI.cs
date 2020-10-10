﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Common;
using Tanttinator.ModularUI;

namespace Client
{
    public class CityProductionUI : MonoBehaviour
    {

        [SerializeField] TMP_Text title = default;

        [SerializeField] Container units = default;
        [SerializeField] Container turns = default;
        [SerializeField] Container counts = default;

        [SerializeField] GameObject productionToggle = default;
        [SerializeField] GameObject tableCell = default;

        [SerializeField] Hidable hidable = default;

        static CityProductionUI instance;

        public static void Show(CityData city)
        {
            instance.units.Clear();
            instance.turns.Clear();
            instance.counts.Clear();

            instance.title.text = city.name + "\nProduction";

            foreach (string unit in ClientController.units) AddUnit(unit, city.owner.color);

            instance.hidable.Show();
        }

        static void AddUnit(string unit, Color color)
        {
            instance.units.Add(unit, instance.productionToggle).GetComponent<ProductionSelectionToggleUI>().Setup(unit, color, (b) => ToggleUnit(unit, b));
            instance.turns.Add(unit, instance.tableCell);
            instance.counts.Add(unit, instance.tableCell);
        }

        static void ToggleUnit(string unit, bool value)
        {
            Debug.Log("Toggle production of " + unit + ": " + value);
        }

        public void Confirm()
        {

        }

        private void Awake()
        {
            instance = this;
        }
    }
}
