// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Queue.cs
// Implements a generic queue using a circular buffer
// ------------------------------------------------------------------------------------------------

class TQueue<T> {
   // Adds an element to the end of the queue
   public void Enqueue (T a) {
      if (mUsed == mData.Length) Resize ();
      mData[mTail] = a;
      mTail = (mTail + 1) % mData.Length;
      mUsed++;
   }

   // Removes and returns the first element in the queue
   public T Dequeue () {
      if (mUsed == 0) throw new InvalidOperationException ("Queue is empty");
      T a = mData[mHead];
      mData[mHead] = default!;
      mHead = (mHead + 1) % mData.Length;
      mUsed--;
      return a;
   }

   // Checks whether the queue contains no elements
   public bool IsEmpty => mUsed == 0;

   // Doubles the buffer size and preserves the queue order
   void Resize () {
      T[] newData = new T[mData.Length * 2];
      for (int i = 0; i < mUsed; i++) newData[i] = mData[(mHead + i) % mData.Length];
      mData = newData;
      mHead = 0;
      mTail = mUsed;
   }

   T[] mData = new T[4];
   int mHead;
   int mTail;
   int mUsed;
}