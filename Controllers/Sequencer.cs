using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequencer : MonoBehaviour
{

    static Queue<Sequence> sequenceQueue = new Queue<Sequence>();
    static Sequence currentSequence;

    /// <summary>
    /// Add new sequence to be executed.
    /// </summary>
    /// <param name="sequence"></param>
    public static void AddSequence(Sequence sequence)
    {
        sequenceQueue.Enqueue(sequence);
    }

    private void Update()
    {
        if(currentSequence != null)
        {
            if(currentSequence.Update())
            {
                currentSequence.End();
                currentSequence = null;
            }
        } 
        else if(sequenceQueue.Count > 0)
        {
            currentSequence = sequenceQueue.Dequeue();
            currentSequence.Start();
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
