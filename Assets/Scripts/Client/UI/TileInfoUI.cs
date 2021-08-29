using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Tanttinator.ModularUI;
using Common;
using System;

namespace Client
{
    public class TileInfoUI : MonoBehaviour
    {
        [SerializeField] TMP_Text locationText = default;
        [SerializeField] TMP_Text infoText = default;
        [SerializeField] Hidable hidable = default;
        [SerializeField] GameObject actionButton = default;
        [SerializeField] Container actionButtonContainer = default;

        static TileInfoUI instance;

        public static void Show(TileData data)
        {
            ShowTileInfo(data, data.structure);
        }

        static void ShowTileInfo(TileData data, StructureData structure)
        {
            instance.locationText.text = "Location: " + data.coords.x + ", " + data.coords.y;
            instance.infoText.text = data.ToString();

            instance.actionButtonContainer.Clear();

            if(structure is CityData)
                AddAction(new ButtonAction("City", () => ShowCityInfo(data, structure)));
            /*if(unit != null)
                AddAction(new ButtonAction("Unit", () => ShowUnitInfo(data, unit, structure)));*/

            instance.hidable.Show();
        }

        static void ShowUnitInfo(TileData data, UnitData unit, StructureData structure)
        {
            instance.locationText.text = "Location: " + data.coords.x + ", " + data.coords.y;
            instance.infoText.text = "Unit: " + unit.unitType + "\nOwner: " + ClientController.gameState.GetPlayer(unit.owner).name;

            instance.actionButtonContainer.Clear();

            if (structure is CityData)
                AddAction(new ButtonAction("City", () => ShowCityInfo(data, structure)));
            AddAction(new ButtonAction("Tile", () => ShowTileInfo(data, structure)));
        }

        static void ShowCityInfo(TileData data, StructureData structure)
        {
            CityData city = (CityData)structure;
            instance.locationText.text = "Location: " + data.coords.x + ", " + data.coords.y;
            instance.infoText.text = "City: " + city.name + 
                "\nOwner: " + ClientController.gameState.GetPlayer(city.owner).name + 
                "\nProduction: " + (
                    city.production == null? "No Production" : 
                    city.production.name + " (" + Mathf.CeilToInt((city.production.productionCost - city.progress[city.production]) * 1f / city.efficiency) + ")") + 
                "\nEfficiency: " + city.efficiency + "%";

            instance.actionButtonContainer.Clear();

            /*if (unit != null)
                AddAction(new ButtonAction("Unit", () => ShowUnitInfo(data, unit, structure)));*/
            AddAction(new ButtonAction("Tile", () => ShowTileInfo(data, structure)));
            AddAction(new ButtonAction("Production", () => { CityProductionUI.Show(city); instance.hidable.Hide(); }));
        }

        static void AddAction(ButtonAction action)
        {
            ActionButtonUI newButton = instance.actionButtonContainer.Add(action, instance.actionButton).GetComponent<ActionButtonUI>();
            newButton.Setup(action);
        }

        private void Awake()
        {
            instance = this;
        }
    }
}
