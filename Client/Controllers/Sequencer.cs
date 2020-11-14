using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Client
{
    public static class Sequencer
    {
        static Queue<Sequence> sequenceQueue = new Queue<Sequence>();
        static Sequence currentSequence;

        /// <summary>
        /// Add new sequence to be executed.
        /// </summary>
        /// <param name="sequence"></param>
        public static void AddSequence(Sequence sequence)
        {
            //Debug.Log("Added sequence: " + sequence);
            sequenceQueue.Enqueue(sequence);
        }

        public static void Update()
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
                //Debug.Log("Started sequence: " + currentSequence);
                currentSequence.Start();
            }

            InputController.Update();
        }
    }
}
