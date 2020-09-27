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
            ShowTileInfo(data);
        }

        static void ShowTileInfo(TileData data)
        {
            instance.locationText.text = "Location: " + data.coords.x + ", " + data.coords.y;
            instance.infoText.text = data.ToString();

            instance.actionButtonContainer.Clear();

            if(data.structure is CityData)
                AddAction(new ButtonAction("City", () => ShowCityInfo(data)));
            if(data.unit != null)
                AddAction(new ButtonAction("Unit", () => ShowUnitInfo(data)));

            instance.hidable.Show();
        }

        static void ShowUnitInfo(TileData data)
        {
            instance.locationText.text = "Location: " + data.coords.x + ", " + data.coords.y;
            instance.infoText.text = "Unit: " + data.unit.unit + "\nOwner: " + data.unit.owner.name;

            instance.actionButtonContainer.Clear();

            if (data.structure is CityData)
                AddAction(new ButtonAction("City", () => ShowCityInfo(data)));
            AddAction(new ButtonAction("Tile", () => ShowTileInfo(data)));
        }

        static void ShowCityInfo(TileData data)
        {
            instance.locationText.text = "Location: " + data.coords.x + ", " + data.coords.y;
            instance.infoText.text = "City: " + (data.structure as CityData).name + "\nOwner: " + data.structure.owner.name;

            instance.actionButtonContainer.Clear();

            if (data.unit != null)
                AddAction(new ButtonAction("Unit", () => ShowUnitInfo(data)));
            AddAction(new ButtonAction("Tile", () => ShowTileInfo(data)));
            AddAction(new ButtonAction("Production", () => Debug.Log("Choose Production")));
        }

        public static void Hide()
        {
            instance.hidable.Hide();
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
