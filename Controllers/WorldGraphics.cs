using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for the graphical representation of the world.
/// </summary>
public class WorldGraphics : MonoBehaviour
{

    [SerializeField] GameObject tileObject = default;
    static GameObject[,] tiles;

    static WorldGraphics instance;

    /// <summary>
    /// Refresh the world graphics according to the current state of the game.
    /// </summary>
    public static void RefreshWorld()
    {
        int width = World.Width;
        int height = World.Height;

        tiles = new GameObject[width, height];

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if(tiles[x, y] == null)
                    tiles[x, y] = Instantiate(instance.tileObject, new Vector3(x, y, 0), Quaternion.identity, instance.transform);
            }
        }
    }

    private void Awake()
    {
        instance = this;
    }

}
