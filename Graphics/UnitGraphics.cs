using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGraphics : MonoBehaviour
{
    [SerializeField] SpriteRenderer unitSprite = default;

    /// <summary>
    /// Set the type of unit to be shown.
    /// </summary>
    /// <param name="unit"></param>
    public void SetUnit(Unit unit)
    {
        if (unit == null) Hide();
        else
        {
            unitSprite.sprite = UnitGraphicsController.GetUnitSprite(unit.type);
            Show();
        }
    }

    /// <summary>
    /// Make this graphic visible.
    /// </summary>
    public void Show()
    {
        unitSprite.enabled = true;
    }

    /// <summary>
    /// Make this graphic invisible.
    /// </summary>
    public void Hide()
    {
        unitSprite.enabled = false;
    }

    private void Awake()
    {
        Hide();
    }
}
