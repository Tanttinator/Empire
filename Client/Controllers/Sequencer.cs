using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sequencer : MonoBehaviour
{

    static Queue<Sequence> sequenceQueue = new Queue<Sequence>();
    static Sequence currentSequence;

    public static bool idle { get; private set; } = true;

    public static event Action onIdleStart;
    public static event Action onIdleEnd;

    /// <summary>
    /// Add new sequence to be executed.
    /// </summary>
    /// <param name="sequence"></param>
    public static void AddSequence(Sequence sequence)
    {
        sequenceQueue.Enqueue(sequence);

        if (idle)
        {
            idle = false;
            onIdleEnd?.Invoke();
        }
    }

    private void Update()
    {
        if (currentSequence != null)
        {
            if (currentSequence.Update())
            {
                currentSequence.End();
                currentSequence = null;
            }
        }
        else if (sequenceQueue.Count > 0)
        {
            currentSequence = sequenceQueue.Dequeue();
            currentSequence.Start();
            //Debug.Log("Started sequence: " + currentSequence);
        } 
        else if(!idle)
        {
            idle = true;
            onIdleStart?.Invoke();
        }
    }
}

public class Sequence
{
    public virtual void Start()
    {

    }

    public virtual bool Update()
    {
        return true;
    }

    public virtual void End()
    {

    }
}

public class StartTurnSequence : Sequence
{
    HumanPlayer player;
    TileData[,] seenTiles;
    Coords focusTile;

    public StartTurnSequence(HumanPlayer player, TileData[,] seenTiles, Coords focusTile)
    {
        this.player = player;
        this.seenTiles = seenTiles;
        this.focusTile = focusTile;
    }

    public override void Start()
    {
        ClientController.SetActivePlayer(player, focusTile);
        WorldGraphics.UpdateTiles(seenTiles);
    }
}

public class EndTurnSequence : Sequence
{
    public override void Start()
    {
        ClientController.activePlayer.EndTurn();
    }
}

public class SelectUnitSequence : Sequence
{
    Coords unit;

    public SelectUnitSequence(Coords unit)
    {
        this.unit = unit;
    }

    public override void Start()
    {
        ClientController.SelectUnit(unit);
        ClientController.Camera.MoveTowards(WorldGraphics.GetTilePosition(unit));
    }

    public override bool Update()
    {
        return !ClientController.Camera.isMovingToTarget;
    }
}

public class DeselectUnitSequence : Sequence
{
    public override void Start()
    {
        ClientController.DeselectUnit();
    }
}
