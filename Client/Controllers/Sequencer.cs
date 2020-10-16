using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;

namespace Client
{
    public class Sequencer : MonoBehaviour
    {

        static Queue<Sequence> sequenceQueue = new Queue<Sequence>();
        static Sequence currentSequence;

        public static bool idle { get; private set; } = true;

        float cooldown = 0f;

        public static event Action onIdleStart;
        public static event Action onIdleEnd;

        /// <summary>
        /// Add new sequence to be executed.
        /// </summary>
        /// <param name="sequence"></param>
        public static void AddSequence(Sequence sequence)
        {
            //Debug.Log("Added sequence: " + sequence);
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
                    cooldown = 0f;
                }
            }
            else if (sequenceQueue.Count > 0)
            {
                if (cooldown == 0f)
                {
                    currentSequence = sequenceQueue.Dequeue();
                    //Debug.Log("Started sequence: " + currentSequence);
                    currentSequence.Start();
                }
            }
            else if (!idle)
            {
                idle = true;
                onIdleStart?.Invoke();
            }

            cooldown = Mathf.Max(0f, cooldown - Time.deltaTime);
        }
    }
}
