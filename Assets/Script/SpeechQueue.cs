using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechQueue
{

    private Queue<float[]> q;

    public SpeechQueue()
    {
        q = new Queue<float[]>();
    }

    public bool Check()
    {
        if (q.Count > 0)
            return true;
        return false;
    }

    public void Push(float[] _speech)
    {
        q.Enqueue(_speech);
    }
    public float[] Pop()
    {
        return q.Dequeue();
    }
    private void DeleteAll()
    {
        q.Clear();
    }

    ~SpeechQueue()
    {
        DeleteAll();
    }
}
