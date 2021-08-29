using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Server
{
    public class Feature
    {
        public string name { get; protected set; }
        public int movementCost { get; protected set; }

        Func<Tile, bool> connectTexture;

        public Feature(string name, int movementCost, Func<Tile, bool> connectTexture)
        {
            this.name = name;
            this.movementCost = movementCost;
            this.connectTexture = connectTexture;
        }

        /// <summary>
        /// Should this feature connect texture to the given tile?
        /// </summary>
        /// <param name="tile"></param>
        public bool ConnectTexture(Tile tile)
        {
            if (tile == null) return true;

            return connectTexture(tile);
        }

        public static Feature forest = new Feature("Forest", 1, (tile) => tile.feature == forest);
        public static Feature mountains = new Feature("Mountains", 1, (tile) => tile.feature == mountains);
        public static Feature river = new Feature("River", 1, (tile) => tile.feature == river || tile.feature == mountains || !tile.land);
    }
}
