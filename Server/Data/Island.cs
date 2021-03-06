﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Server
{
    public class Island
    {
        List<Tile> tiles = new List<Tile>();
        public int Area => tiles.Count;

        /// <summary>
        /// Assign a new tile to this island.
        /// </summary>
        /// <param name="tile"></param>
        public void AddTile(Tile tile)
        {
            tile.island = this;
            tiles.Add(tile);
        }

        /// <summary>
        /// Try to return a random tile from this island that is coastal and doesn't have a feature.
        /// </summary>
        /// <returns></returns>
        public Tile GetCoastalCitySpot()
        {
            if (Area == 0) return null;

            for (int i = 0; i < 1000; i++)
            {
                Tile tile = tiles[Random.Range(0, Area)];
                if (tile.IsCoastal && tile.feature == null) return tile;
            }

            return null;
        }

        /// <summary>
        /// Returns the best location for a city.
        /// </summary>
        /// <returns></returns>
        public Tile GetOptimalCitySpot()
        {
            float maxScore = Mathf.Infinity;
            Tile bestTile = null;

            foreach (Tile tile in tiles)
            {
                if (tile.structure != null || tile.feature != null) continue;

                float score = CityScore(tile);
                if (score > maxScore || bestTile == null)
                {
                    maxScore = score;
                    bestTile = tile;
                }
            }

            return bestTile;
        }

        /// <summary>
        /// Calculate the city score of a tile.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        float CityScore(Tile tile)
        {
            float totalScore = 0f;

            foreach (Tile other in tiles)
            {
                if (other == tile) continue;

                if (other.structure is City)
                {
                    totalScore += Mathf.Pow(Vector2.Distance(tile.coords, other.coords), 0.1f);
                }
            }

            return totalScore;
        }
    }
}
