using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class UnitGraphics : MonoBehaviour
    {
        [SerializeField] SpriteRenderer unitSprite = default;
        [SerializeField] SpriteRenderer unitBackground = default;

        bool shown = false;

        bool idle = false;
        float idleTimer = 0f;
        [SerializeField] float idleFrequency = 1f;

        Color playerColor = Color.white;

        /// <summary>
        /// Set the type of unit to be shown.
        /// </summary>
        /// <param name="unit"></param>
        public void SetUnit(UnitData unit)
        {
            if (unit == null) Hide();
            else
            {
                unitSprite.sprite = SpriteRegistry.GetSprite(unit.unit).GetSprite(false, false, false, false).sprite;
                playerColor = unit.color;
                unitSprite.color = playerColor;
                unitBackground.color = playerColor;
                Show();
            }
        }

        /// <summary>
        /// Start / Stop unit idle animation.
        /// </summary>
        /// <param name="idle"></param>
        public void SetIdle(bool idle)
        {
            this.idle = idle;
            idleTimer = idleFrequency / 2f;
            unitSprite.enabled = shown;
            unitBackground.enabled = shown;
        }

        /// <summary>
        /// Make this graphic visible.
        /// </summary>
        public void Show()
        {
            unitSprite.enabled = true;
            unitBackground.enabled = true;
            shown = true;
        }

        /// <summary>
        /// Make this graphic invisible.
        /// </summary>
        public void Hide()
        {
            unitSprite.enabled = false;
            unitBackground.enabled = false;
            shown = false;
            SetIdle(false);
        }

        private void Awake()
        {
            Hide();
        }

        private void Update()
        {
            if (idle && shown)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer >= idleFrequency)
                {
                    unitSprite.enabled = !unitSprite.enabled;
                    unitBackground.enabled = !unitBackground.enabled;
                    idleTimer = 0f;
                }
            }
        }
    }
}