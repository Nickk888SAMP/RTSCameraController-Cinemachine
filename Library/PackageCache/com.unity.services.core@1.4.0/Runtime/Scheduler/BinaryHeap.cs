using System;
using UnityEngine;

namespace Unity.Services.Core.Scheduler.Internal
{
    class MinimumBinaryHeap<T> where T : IComparable<T>
    {
        const float k_IncreaseFactor = 1.5f;
        const float k_DecreaseFactor = 2.0f;

        readonly int m_MinimumCapacity;
        T[] m_HeapArray;
        int m_Count;

        public int Count => m_Count;
        public T Min => m_HeapArray[0];
        public MinimumBinaryHeap(int capacity = 10)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("capacity must be more than 0");
            }
            m_MinimumCapacity = capacity;
            m_HeapArray = new T[capacity];
            m_Count = 0;
        }

        public void Insert(T data)
        {
            IncreaseHeapCapacityWhenFull();
            var dataPos = m_Count;
            m_HeapArray[m_Count] = data;
            m_Count++;
            while (dataPos != 0 && m_HeapArray[dataPos].CompareTo(m_HeapArray[Parent(dataPos)]) < 0)
            {
                Swap(ref m_HeapArray[dataPos], ref m_HeapArray[Parent(dataPos)]);
                dataPos = Parent(dataPos);
            }
        }

        void IncreaseHeapCapacityWhenFull()
        {
            if (m_Count == m_HeapArray.Length)
            {
                int newCapacity = (int)Math.Ceiling(Count * k_IncreaseFactor);
                T[] newHeapArray = new T[newCapacity];
                Array.Copy(m_HeapArray, newHeapArray, m_Count);
                m_HeapArray = newHeapArray;
            }
        }

        public void Remove(T data)
        {
            var key = GetKey(data);
            while (key != 0)
            {
                Swap(ref m_HeapArray[key],
                    ref m_HeapArray[Parent(key)]);
                key = Parent(key);
            }
            ExtractMin();
        }

        public T ExtractMin()
        {
            if (m_Count <= 0)
            {
                throw new InvalidOperationException("Can not ExtractMin: BinaryHeap is empty.");
            }
            var data = m_HeapArray[0];

            if (m_Count == 1)
            {
                m_Count--;
                m_HeapArray[0] = default;
                return data;
            }

            m_Count--;
            m_HeapArray[0] = m_HeapArray[m_Count];
            m_HeapArray[m_Count] = default;
            MinHeapify(0);
            DecreaseHeapCapacityWhenSpare();
            return data;
        }

        void DecreaseHeapCapacityWhenSpare()
        {
            if (m_Count > m_MinimumCapacity && m_Count < m_HeapArray.Length / k_DecreaseFactor)
            {
                T[] newHeapArray = new T[m_Count];
                Array.Copy(m_HeapArray, newHeapArray, m_Count);
                m_HeapArray = newHeapArray;
            }
        }

        int GetKey(T data)
        {
            var key = -1;
            for (var i = 0; i < m_Count; i++)
            {
                if (m_HeapArray[i].Equals(data))
                {
                    key = i;
                    break;
                }
            }

            return key;
        }

        void MinHeapify(int key)
        {
            int leftKey = LeftChild(key);
            int rightKey = RightChild(key);

            int smallest = key;

            if (leftKey < m_Count &&
                m_HeapArray[leftKey].CompareTo(m_HeapArray[smallest]) < 0)
            {
                smallest = leftKey;
            }

            if (rightKey < m_Count &&
                m_HeapArray[rightKey].CompareTo(m_HeapArray[smallest]) < 0)
            {
                smallest = rightKey;
            }

            if (smallest != key)
            {
                Swap(ref m_HeapArray[key],
                    ref m_HeapArray[smallest]);
                MinHeapify(smallest);
            }
        }

        static void Swap(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        static int Parent(int key)
        {
            return (key - 1) / 2;
        }

        static int LeftChild(int key)
        {
            return 2 * key + 1;
        }

        static int RightChild(int key)
        {
            return 2 * key + 2;
        }
    }
}
