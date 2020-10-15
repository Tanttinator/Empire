using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Server
{
    public class Human : Player
    {
        public Unit ActiveUnit => (activeUnits.Count > 0 ? activeUnits[0] : null);
        List<Unit> activeUnits = new List<Unit>();

        public Human(string name, Color color) : base(name, color)
        {

        }

        /// <summary>
        /// Try to execute a command given by the player.
        /// </summary>
        /// <param name="command"></param>
        public void ExecuteCommand(PlayerCommand command)
        {
            command.Execute(this, ActiveUnit);
        }

        /// <summary>
        /// Removes current unit from the list of active units.
        /// </summary>
        public void NextUnit()
        {
            if (activeUnits.Count == 0) return;

            activeUnits.RemoveAt(0);
            CommunicationController.AddSequence(new DeselectUnitSequence());

            if (activeUnits.Count == 0) CommunicationController.AddSequence(new EndTurnSequence());
            else SelectUnit(ActiveUnit);
        }

        /// <summary>
        /// Set a unit as active.
        /// </summary>
        /// <param name="unit"></param>
        void SelectUnit(Unit unit)
        {
            if (unit == null) return;
            CommunicationController.AddSequence(new SelectUnitSequence(unit.ID));
        }

        protected override void OnTurnStarted()
        {
            foreach (Unit unit in units)
            {
                if (!unit.sleeping) activeUnits.Add(unit);
            }

            CommunicationController.AddSequence(new StartTurnSequence(ID, seenTiles, SeenStructures, ActiveUnit.tile.coords));

            SelectUnit(ActiveUnit);
        }

        protected override void OnTurnEnded()
        {
            activeUnits.Clear();
        }

        public override void DoTurn()
        {
            if (ActiveUnit == null) return;
            if (ActiveUnit.DoTurn()) NextUnit();
        }
    }
}
