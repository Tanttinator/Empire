using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGraphicsController : MonoBehaviour
{
    [SerializeField] UnitSpriteData[] unitSprites = default;

    static UnitGraphicsController instance;

    /// <summary>
    /// Find a sprite matching the given unit type in the registry.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public static Sprite GetUnitSprite(UnitType unit)
    {
        foreach(UnitSpriteData data in instance.unitSprites)
        {
            if (data.unit == unit) return data.sprite;
        }
        return null;
    }

    private void Awake()
    {
        instance = this;
    }
}

[System.Serializable]
public struct UnitSpriteData
{
    public UnitType unit;
    public Sprite sprite;
}
