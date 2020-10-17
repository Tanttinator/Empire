using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Server
{
    public abstract class Observer
    {
        public Player owner { get; protected set; }
        public Tile tile { get; protected set; }
        Tile[] visibleTiles;

        public void SetTile(Tile tile)
        {
            Tile oldTile = this.tile;
            this.tile = tile;
            OnTileChanged(tile, oldTile);
            RefreshVision();
        }

        protected virtual void OnTileChanged(Tile tile, Tile oldTile)
        {

        }

        public void SetOwner(Player owner)
        {
            RemoveObserver();
            Player oldOwner = this.owner;
            this.owner = owner;
            OnOwnerChanged(owner, oldOwner);
            AddObserver();
        }

        protected virtual void OnOwnerChanged(Player owner, Player oldOwner)
        {

        }

        void RefreshVision()
        {
            RemoveObserver();
            AddObserver();
        }

        protected void RemoveObserver()
        {
            if (tile == null) return;
            if (visibleTiles != null)
            {
                foreach (Tile tile in visibleTiles) tile.RemoveObserver(this);
            }
        }

        void AddObserver()
        {
            if (tile == null) return;
            visibleTiles = GetTilesInVision();
            foreach (Tile tile in visibleTiles) tile.AddObserver(this);
        }

        /// <summary>
        /// Returns all tiles within this units visibility.
        /// </summary>
        /// <returns></returns>
        Tile[] GetTilesInVision()
        {
            List<Tile> tiles = new List<Tile>();

            tiles.Add(tile);
            tiles.AddRange(World.GetNeighbors(tile));

            return tiles.ToArray();
        }

    }
}
