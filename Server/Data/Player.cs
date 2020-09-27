using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;

namespace Server
{
    public class Player
    {
        public int ID { get; protected set; }
        public string name { get; protected set; }
        public Color color { get; protected set; }

        protected List<Unit> units = new List<Unit>();

        //TODO: cache seen tiles when they are updated.
        public TileData[] SeenTiles
        {
            get
            {
                TileData[] tiles = new TileData[World.Width * World.Height];

                for (int x = 0; x < World.Width; x++)
                {
                    for (int y = 0; y < World.Height; y++)
                    {
                        tiles[x + y * World.Width] = World.GetTile(x, y).VisibleState(this);
                    }
                }

                return tiles;
            }
        }

        public event Action onTurnStarted;

        static int nextID = 0;

        public Player(string name, Color color)
        {
            ID = nextID;
            nextID++;

            this.name = name;
            this.color = color;
        }

        /// <summary>
        /// Called when this players turn starts.
        /// </summary>
        public void StartTurn()
        {
            onTurnStarted?.Invoke();
            foreach (Unit unit in units)
            {
                unit.Refresh();
            }
            OnTurnStarted();
        }

        /// <summary>
        /// Tells GameController to cycle to the next player.
        /// </summary>
        public void EndTurn()
        {
            OnTurnEnded();
            GameController.NextPlayer();
        }

        /// <summary>
        /// Set all tiles visible to this player.
        /// </summary>
        public void RevealMap()
        {
            for (int x = 0; x < World.Width; x++)
            {
                for (int y = 0; y < World.Height; y++)
                {
                    World.GetTile(x, y).Reveal(this);
                }
            }
        }

        /// <summary>
        /// Add a unit for this player.
        /// </summary>
        /// <param name="unit"></param>
        public void AddUnit(Unit unit)
        {
            units.Add(unit);
        }

        /// <summary>
        /// Remove a unit from this player.
        /// </summary>
        /// <param name="unit"></param>
        public void RemoveUnit(Unit unit)
        {
            units.Remove(unit);
        }

        public PlayerData GetData()
        {
            return new PlayerData()
            {
                name = name,
                color = color
            };
        }

        public virtual void AddSequence(Sequence sequence)
        {

        }

        /// <summary>
        /// Callback for when this players turn has ended.
        /// </summary>
        protected virtual void OnTurnEnded()
        {

        }

        /// <summary>
        /// Callback for when this players turn has started.
        /// </summary>
        protected virtual void OnTurnStarted()
        {

        }

        /// <summary>
        /// Called every frame while this player is active.
        /// </summary>
        public virtual void DoTurn()
        {

        }

        public override string ToString()
        {
            return name;
        }
    }
}
