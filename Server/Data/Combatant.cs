using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Server
{
    public abstract class Combatant
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

        #region Vision

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

        #endregion

        #region Combat

        protected static void Battle(Combatant attacker, Combatant defender)
        {
            bool attackerDead = false;
            bool defenderDead = false;

            CommunicationController.SpawnExplosion(defender.tile, attacker.tile);
            CommunicationController.SpawnExplosion(attacker.tile, defender.tile);

            while (!attackerDead && !defenderDead)
            {
                if (Random.Range(0, 2) == 0)
                {
                    CommunicationController.SpawnExplosion(attacker.tile, defender.tile);
                    attackerDead = attacker.TakeDamage();
                }
                else
                {
                    CommunicationController.SpawnExplosion(defender.tile, attacker.tile);
                    defenderDead = defender.TakeDamage();
                }

                if (attackerDead)
                {
                    attacker.OnDefeat(defender);
                }

                if (defenderDead)
                {
                    attacker.OnVictory(defender);
                    defender.OnDefeat(attacker);
                }
            }

            CommunicationController.UpdateState(0.3f);
        }

        protected virtual bool TakeDamage()
        {
            return true;
        }

        protected virtual void OnVictory(Combatant enemy)
        {

        }

        protected virtual void OnDefeat(Combatant enemy)
        {

        }

        #endregion

    }
}
