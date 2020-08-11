using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sequencer : MonoBehaviour
{

    static Queue<Sequence> sequenceQueue = new Queue<Sequence>();
    static Sequence currentSequence;

    public static bool idle { get; private set; }

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
