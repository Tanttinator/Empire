using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client {
    public class UnitMovementIndicator : MonoBehaviour
    {
        new LineRenderer renderer;

        static UnitMovementIndicator instance;

        public static void Show(Coords start, Coords end)
        {
            instance.renderer.SetPositions(new Vector3[] { (Vector2)start, (Vector2)end });
            instance.renderer.enabled = true;
        }

        public static void Hide()
        {
            instance.renderer.enabled = false;
        }
    
    private void Awake()
        {
            renderer = GetComponent<LineRenderer>();
            renderer.enabled = false;

            instance = this;
        }
    }
}
