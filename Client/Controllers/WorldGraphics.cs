using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Client
{
    /// <summary>
    /// Responsible for the graphical representation of the world.
    /// </summary>
    public class WorldGraphics : MonoBehaviour
    {

        [SerializeField] TileGraphics tileObject = default;
        [SerializeField] GameObject explosion = default;
        static TileGraphics[,] tiles;

        static int width;
        static int height;

        static WorldGraphics instance;

        /// <summary>
        /// Create all tile objects.
        /// </summary>
        public static void InitTiles(int width, int height)
        {
            WorldGraphics.width = width;
            WorldGraphics.height = height;

            tiles = new TileGraphics[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tiles[x, y] = Instantiate(instance.tileObject.gameObject, GetTilePosition(new Coords(x, y)), Quaternion.identity, instance.transform).GetComponent<TileGraphics>();
                }
            }

            ClientController.CameraController.SetConstraints(0f, 0f, width, height);
        }

        /// <summary>
        /// Draw the given state.
        /// </summary>
        /// <param name="state"></param>
        public static void DrawState(GameState state)
        {
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    GetTileGraphics(x, y).Refresh(state.GetTile(new Coords(x, y)));
                }
            }
        }

        public static bool ValidCoords(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        public static bool ValidCoords(Coords coords)
        {
            return ValidCoords(coords.x, coords.y);
        }

        public static void SpawnExplosion(Coords tile)
        {
            Instantiate(instance.explosion, GetTilePosition(tile), Quaternion.identity);
        }

        #region Accessors

        /// <summary>
        /// Returns the graphics at the given coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static TileGraphics GetTileGraphics(int x, int y)
        {
            if (!ValidCoords(x, y)) return null;
            return tiles[x, y];
        }

        /// <summary>
        /// Returns the graphics at the given coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static TileGraphics GetTileGraphics(Coords c)
        {
            if (!ValidCoords(c.x, c.y)) return null;
            return tiles[c.x, c.y];
        }

        /// <summary>
        /// Returns the coords of the tile whose graphics overlap a point in the world.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Coords GetTileAtPoint(Vector2 point)
        {
            return new Vector2(point.x - instance.transform.position.x, point.y - instance.transform.position.y);
        }

        /// <summary>
        /// Returns the coordinates for the position of the given tile in the world graphics.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public static Vector3 GetTilePosition(Coords coords)
        {
            return new Vector3(coords.x, coords.y, 0f);
        }

        #endregion

        private void Awake()
        {
            instance = this;
        }

    }
}
