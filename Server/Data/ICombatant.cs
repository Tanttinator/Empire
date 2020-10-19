using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Server
{
    public interface ICombatant
    {
        Player Owner { get; }
        Tile Tile { get; }
        void Defeated(ICombatant enemy);
        bool TakeDamage();
    }
}
