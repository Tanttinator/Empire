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
        /// Activate all units within this units cargo.
        /// </summary>
        /// <param name="unit"></param>
        public void UnloadUnit(Unit unit)
        {
            if (unit.Cargo.Length == 0) return;

            foreach(Unit other in unit.Cargo)
            {
                activeUnits.Remove(other);
                activeUnits.Insert(0, other);
            }
            SelectUnit(activeUnits[0]);
        }

        public void SetUnitInactive(Unit unit)
        {
            bool wasSelected = selectedUnit == unit;

            if (wasSelected) DeselectUnit();

            activeUnits.Remove(unit);

            if (wasSelected)
            {
                if (activeUnits.Count == 0) CommunicationController.TurnCompleted(this);
                else SelectUnit(activeUnits[0]);
            }
        }

        public void SelectUnit(Unit unit)
        {
            if (unit.moves == 0) return;
            if (selectedUnit != null) DeselectUnit();
            selectedUnit = unit;
            CommunicationController.SelectUnit(unit);
        }

        void DeselectUnit()
        {
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
            if (selectedUnit.DoTurn()) SetUnitInactive(selectedUnit);
        }
    }
}
