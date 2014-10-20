using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class KZCircularBuffer<T> : IEnumerable<T> {

    private T[] values;
    private int start;
    private int size;

    public KZCircularBuffer(int capacity)
    {
        values = new T[capacity];
    }

    public void Add(T t)
    {
        int next = (start + size) % values.Length;
        values[next] = t;
        if (size == values.Length)
        {
            start = (start + 1) % values.Length;
        }
        else
        {
            size++;
        }
    }

    public int Count { get { return size; } }

    public T GetAt(int index)
    {
        return values[index];
    }

    public T GetLast()
    {
        if (size <= 0) return default(T); //>>>
        else return values[(start + size - 1) % values.Length];
    }


    public T this[int index]
    {
        get { return values[(start + index) % values.Length];  }
        set { values[index] = value;  }
    }

    public void Resize(int newCapacity)
    {
        if (newCapacity == values.Length)
        {
            return;
        }
        else
        {
            T[] temp = values;
            values = new T[newCapacity];

            if (newCapacity > values.Length)
            {    
                for (int i = 0; i < size; i++)
                {
                    values[i] = temp[(start + i) % temp.Length];
                }
                start = 0;
            }
            else
            {
                if (newCapacity < size)
                {
                    start = (start + (size - newCapacity)) % temp.Length;
                    size = newCapacity;
                }
                for (int i = 0; i < size; i++)
                {
                    values[i] = temp[(start + i) % temp.Length];
                }
                start = 0;
            }
        } 
        
    }

    public void Clear()
    {
        size = 0;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for(int i=0; i<size; i++)
        {
            yield return values[(start + i) % values.Length];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void DebugPrint()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < values.Length; i++)
        {
            if (i == start) sb.Append(">");
            if (i == start + size) sb.Append("|");
            sb.Append(values[i]);
            sb.Append(" ");
        }
        sb.Append("start = ").Append(start);
        sb.Append(", size = ").Append(size);
        Debug.Log(sb);
    }
}
