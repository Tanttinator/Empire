using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;

namespace Server
{
    public class Structure : Observer
    {
        public int ID { get; protected set; }
        public string type { get; protected set; }

        static int nextID = 0;

        public Structure(string type)
        {
            this.type = type;
            ID = nextID++;
            owner = GameController.neutral;
        }

        protected override void OnOwnerChanged(Player owner, Player oldOwner)
        {
            tile?.UpdateState();
            tile?.UpdateState(oldOwner);
        }

        public virtual bool Interact(Unit unit)
        {
            return false;
        }

        public virtual StructureData GetData()
        {
            return new StructureData()
            {
                ID = ID,
                structure = type,
                owner = owner.ID
            };
        }

        /// <summary>
        /// Create a new structure.
        /// </summary>
        /// <param name="structure"></param>
        /// <param name="tile"></param>
        /// <param name="owner"></param>
        public static void CreateStructure(Structure structure, Tile tile, Player owner)
        {
            structure.SetTile(tile);
            structure.SetOwner(owner);
            tile.SetStructure(structure);
        }
    }
}
