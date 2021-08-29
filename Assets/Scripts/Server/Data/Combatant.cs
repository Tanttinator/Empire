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

        public virtual void SetTile(Tile tile)
        {
            RemoveObserver();
            this.tile = tile;
            AddObserver();
        }

        public virtual void SetOwner(Player owner)
        {
            RemoveObserver();
            this.owner = owner;
            AddObserver();
        }

        #region Vision

        protected void RemoveObserver()
        {
            if (tile == null) return;
            if (visibleTiles != null)
            {
                foreach (Tile tile in visibleTiles) tile.RemoveObserver(this);
            }
        }

        protected void AddObserver()
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

            int attackerStrength = attacker.CalculateStrength(defender);
            int defenderStrength = defender.CalculateStrength(attacker);

            CommunicationController.SpawnExplosion(defender.tile, attacker.tile);
            CommunicationController.SpawnExplosion(attacker.tile, defender.tile);

            while (!attackerDead && !defenderDead)
            {
                if (Random.Range(0f, 1f) < attackerStrength * 1f / (attackerStrength + defenderStrength))
                {
                    CommunicationController.SpawnExplosion(defender.tile, attacker.tile);
                    defenderDead = defender.TakeDamage();
                }
                else
                {
                    CommunicationController.SpawnExplosion(attacker.tile, defender.tile);
                    attackerDead = attacker.TakeDamage();
                }
            }

            if (attackerDead)
            {
                attacker.OnDefeat(defender);
            }

            if (defenderDead)
            {
                defender.OnDefeat(attacker);
                attacker.OnVictory(defender);
            }

            CommunicationController.UpdateState(0.3f);
        }

        protected virtual int CalculateStrength(Combatant enemy)
        {
            return 1;
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
