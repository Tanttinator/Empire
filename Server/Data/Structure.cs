using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;

namespace Server
{
    public class Structure
    {
        public int ID { get; protected set; }
        public string type { get; protected set; }
        public Player owner { get; protected set; } = GameController.neutral;
        public Tile tile { get; protected set; }

        static int nextID = 0;

        public Structure(string type)
        {
            this.type = type;
            ID = nextID++;
            //SetOwner(GameController.neutral);
        }

        public void SetTile(Tile tile)
        {
            this.tile = tile;
        }

        public void SetOwner(Player owner)
        {
            Player oldOwner = this.owner;
            this.owner = owner;
            OnOwnerChanged(oldOwner);
            tile?.UpdateState();
            tile?.UpdateState(oldOwner);
        }

        protected virtual void OnOwnerChanged(Player oldOwner)
        {

        }

        public virtual void Interact(Unit unit)
        {

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
