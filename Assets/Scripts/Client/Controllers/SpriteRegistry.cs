using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class SpriteRegistry : MonoBehaviour
    {
        [SerializeField] List<RegistryItem> registry = default;

        static SpriteRegistry instance;

        public static SpriteProvider GetSprite(string key)
        {
            foreach (RegistryItem item in instance.registry)
            {
                if (item.key == key) return item.sprite;
            }

            Debug.LogError("No sprite registered for " + key);
            return null;
        }

        private void Awake()
        {
            instance = this;
        }
    }

    [System.Serializable]
    public struct RegistryItem
    {
        public string key;
        public SpriteProvider sprite;
    }
}