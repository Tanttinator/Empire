using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Client
{
    public class UnitGraphics : MonoBehaviour
    {
        [SerializeField] SpriteRenderer unitSprite = default;
        [SerializeField] SpriteRenderer unitBackground = default;

        bool idle = false;
        bool idleOn = true;
        float idleTimer = 0f;
        [SerializeField] float idleFrequency = 1f;

        Color playerColor = Color.white;

        UnitData unit;
        UnitData activeUnit;

        /// <summary>
        /// Set the type of unit to be shown.
        /// </summary>
        /// <param name="unit"></param>
        public void SetUnit(int unit)
        {
            this.unit = ClientController.gameState.GetUnit(unit);
            SetActiveUnit(this.unit);
        }

        public void SetActiveUnit(UnitData unit)
        {
            activeUnit = unit;
            ShowGraphics(unit);
        }

        void ShowGraphics(UnitData unit)
        {
            if (unit == null)
            {
                Hide();
                return;
            }

            unitSprite.sprite = SpriteRegistry.GetSprite(unit.unitType).GetSprite(false, false, false, false).sprite;
            playerColor = ClientController.gameState.GetPlayer(unit.owner).color;
            unitSprite.color = playerColor;
            Color backgroundColor = playerColor;
            if (unit.sleeping) backgroundColor = Color.white;
            unitBackground.color = backgroundColor;

            unitSprite.enabled = true;
            unitBackground.enabled = true;
        }

        void Hide()
        {
            unitSprite.enabled = false;
            unitBackground.enabled = false;
        }

        public void StartIdle()
        {
            idle = true;
            idleTimer = idleFrequency / 1.1f;
            IdleOn();
        }

        public void StopIdle()
        {
            idle = false;
            ShowGraphics(activeUnit);
        }

        void ToggleIdle()
        {
            if (idleOn) IdleOff();
            else IdleOn();
        }

        void IdleOn()
        {
            ShowGraphics(activeUnit);
            idleOn = true;
        }

        void IdleOff()
        {
            if (activeUnit == unit) Hide();
            else ShowGraphics(unit);
            idleOn = false;
        }

        private void Awake()
        {
            Hide();
        }

        private void Update()
        {
            if (idle)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer >= idleFrequency)
                {
                    ToggleIdle();
                    idleTimer = 0f;
                }
            }
        }
    }
}