using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Server
{
    public class Human : Player
    {
        Unit selectedUnit = null;
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
            command.Execute(this, selectedUnit);
        }

        /// <summary>
        /// Removes current unit from the list of active units.
        /// </summary>
        public void NextUnit()
        {
            DeselectUnit();
            if (activeUnits.Count == 0) CommunicationController.TurnCompleted(this);
            else SelectUnit(activeUnits[0]);
        }

        /// <summary>
        /// Set a unit as active.
        /// </summary>
        /// <param name="unit"></param>
        public void SelectUnit(Unit unit)
        {
            if (unit == null) return;
            DeselectUnit();
            activeUnits.Remove(unit);
            selectedUnit = unit;
            CommunicationController.SelectUnit(unit);
        }

        void DeselectUnit()
        {
            if (selectedUnit == null) return;
            selectedUnit = null;
            CommunicationController.DeselectUnit();
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

            CommunicationController.StartTurn(this, activeUnits[0]);

            SelectUnit(activeUnits[0]);
        }

        protected override void OnTurnEnded()
        {
            activeUnits.Clear();
        }

        public override void DoTurn()
        {
            if (selectedUnit == null) return;
            if (selectedUnit.DoTurn()) NextUnit();
        }
    }
}
