using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores data for the current world.
/// </summary>
public class World : MonoBehaviour
{
    [Header("World Parameters")]
    [SerializeField] int width = 10;
    [SerializeField] int height = 10;

    public static int Width => instance.width;
    public static int Height => instance.height;

    static World instance;

    private void Start()
    {
        WorldGraphics.RefreshWorld();
    }

    private void Awake()
    {
        instance = this;
    }
}
