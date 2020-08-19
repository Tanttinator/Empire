using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureGraphics : MonoBehaviour
{
    [SerializeField] SpriteRenderer background = default;
    [SerializeField] SpriteRenderer icon = default;

    public void SetStructure(StructureData structure)
    {
        if(structure == null)
        {
            background.enabled = false;
            icon.enabled = false;
        }
        else
        {
            icon.sprite = WorldGraphics.GetStructureSprite(structure.structure);
            icon.color = structure.color;
            background.enabled = true;
            icon.enabled = true;
        }
    }
}
