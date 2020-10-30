using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
using System.Linq;

namespace Server
{
    public class Player
    {
        public int ID { get; protected set; }
        public string name { get; protected set; }
        public Color color { get; protected set; }

        protected List<Unit> units = new List<Unit>();
        protected List<City> cities = new List<City>();

        public GameState currentState { get; protected set; }

        static int nextID = 0;

        public Player(string name, Color color)
        {
            ID = nextID;
            nextID++;

            this.name = name;
            this.color = color;
        }

        /// <summary>
        /// Initialize current state.
        /// </summary>
        /// <param name="players"></param>
        public void Initialize(PlayerData[] players)
        {
            currentState = new GameState(World.Width, World.Height, players);
        }

        /// <summary>
        /// Called when this players turn starts.
        /// </summary>
        public void StartTurn()
        {
            foreach(City city in cities)
            {
                city.Produce();
            }


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
        /// Update the visible state of a tile.
        /// </summary>
        /// <param name="tile"></param>
        public void UpdateTile(TileData tile)
        {
            currentState.UpdateTile(tile);
            CommunicationController.UpdateTile(this, tile);
        }

        public void UpdatePlayer(PlayerData player)
        {
            currentState.UpdatePlayer(player);
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

        /// <summary>
        /// Add a new city for this player.
        /// </summary>
        /// <param name="city"></param>
        public void AddCity(City city)
        {
            cities.Add(city);
        }

        /// <summary>
        /// Remove a city from this players control.
        /// </summary>
        /// <param name="city"></param>
        public void RemoveCity(City city)
        {
            cities.Remove(city);
        }

        public PlayerData GetData()
        {
            Dictionary<UnitType, int> production = new Dictionary<UnitType, int>();

            foreach (UnitType unit in Unit.units) production.Add(unit, 0);

            foreach (City city in cities)
            {
                if(city.production != null)
                    production[city.production] += 1;
            }

            return new PlayerData()
            {
                ID = ID,
                name = name,
                color = color,
                production = production
            };
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
