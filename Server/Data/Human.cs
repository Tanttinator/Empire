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
        /// Issue a command to the active unit.
        /// </summary>
        /// <param name="command"></param>
        public void ExecuteCommand(UnitCommand command)
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
            CommunicationController.DeselectUnit();

            if (activeUnits.Count == 0) CommunicationController.TurnCompleted(this);
            else SelectUnit(ActiveUnit);
        }

        /// <summary>
        /// Set a unit as active.
        /// </summary>
        /// <param name="unit"></param>
        void SelectUnit(Unit unit)
        {
            if (unit == null) return;
            CommunicationController.SelectUnit(unit);
        }

        protected override void OnTurnStarted()
        {
            foreach (Unit unit in units)
            {
                if (!unit.sleeping) activeUnits.Add(unit);
            }
            if (activeUnits.Count == 0)
            {
                EndTurn();
                return;
            }

            CommunicationController.StartTurn(this);

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
