using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGraphics : MonoBehaviour
{
    [SerializeField] SpriteRenderer unitSprite = default;

    bool shown = false;

    bool idle = false;
    float idleTimer = 0f;
    [SerializeField] float idleFrequency = 1f;

    /// <summary>
    /// Set the type of unit to be shown.
    /// </summary>
    /// <param name="unit"></param>
    public void SetUnit(UnitType unit, Color unitColor)
    {
        if (unit == null) Hide();
        else
        {
            unitSprite.sprite = UnitGraphicsController.GetUnitSprite(unit);
            unitSprite.color = unitColor;
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
    }

    /// <summary>
    /// Make this graphic visible.
    /// </summary>
    public void Show()
    {
        unitSprite.enabled = true;
        shown = true;
    }

    /// <summary>
    /// Make this graphic invisible.
    /// </summary>
    public void Hide()
    {
        unitSprite.enabled = false;
        shown = false;
        SetIdle(false);
    }

    private void Awake()
    {
        Hide();
    }

    private void Update()
    {
        if(idle && shown)
        {
            idleTimer += Time.deltaTime;
            if(idleTimer >= idleFrequency)
            {
                unitSprite.enabled = !unitSprite.enabled;
                idleTimer = 0f;
            }
        }
    }
}
