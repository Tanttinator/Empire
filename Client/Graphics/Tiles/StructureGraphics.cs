using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Client {
    public class StructureGraphics : MonoBehaviour
    {
        [SerializeField] SpriteRenderer background = default;
        [SerializeField] SpriteRenderer icon = default;

        public void SetStructure(StructureData structure)
        {
            if (structure == null)
            {
                background.enabled = false;
                icon.enabled = false;
            }
            else
            {
                icon.sprite = SpriteRegistry.GetSprite(structure.structure).GetSprite(false, false, false, false).sprite;
                icon.color = ClientController.CurrentState.GetPlayer(structure.owner).color;
                background.enabled = true;
                icon.enabled = true;
            }
        }
    }
}
