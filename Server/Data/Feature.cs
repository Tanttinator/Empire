using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Server
{
    public class Feature
    {
        public string name { get; protected set; }

        Func<Tile, bool> connectTexture;

        public Feature(string name, Func<Tile, bool> connectTexture)
        {
            this.name = name;
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

        public static Feature forest = new Feature("Forest", (tile) => tile.feature == forest);
        public static Feature mountains = new Feature("Mountains", (tile) => tile.feature == mountains);
        public static Feature river = new Feature("River", (tile) => tile.feature == river || tile.feature == mountains || !tile.land);
    }
}
