// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Queue.cs
// Implements a generic queue using a circular buffer
// ------------------------------------------------------------------------------------------------

#region Class TQueue ------------------------------------------------------------------------------
/// <summary>Implements a generic queue using a circular buffer</summary>
/// <typeparam name="T"></typeparam>
class TQueue<T> {
   #region Properties -----------------------------------------------
   /// <summary>Checks whether the queue contains no elements</summary>
   public bool IsEmpty => mUsed == 0;
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Adds an element to the end of the queue</summary>
   /// <param name="a"></param>
   public void Enqueue (T a) {
      if (mUsed == mData.Length) Resize ();
      mData[mTail] = a;
      mTail = (mTail + 1) % mData.Length;
      mUsed++;
   }

   /// <summary>Removes and returns the first element in the queue</summary>
   /// <exception cref="InvalidOperationException"></exception>
   public T Dequeue () {
      if (mUsed == 0) throw new InvalidOperationException ("Queue is empty");
      T a = mData[mHead];
      mData[mHead] = default!;
      mHead = (mHead + 1) % mData.Length;
      mUsed--;
      return a;
   }
   #endregion

   #region Implementation -------------------------------------------
   // Doubles the buffer size while preserving the queue order
   void Resize () {
      T[] newData = new T[mData.Length * 2];
      for (int i = 0; i < mUsed; i++) newData[i] = mData[(mHead + i) % mData.Length];
      mData = newData;
      mHead = 0;
      mTail = mUsed;
   }
   #endregion

   #region Private data ---------------------------------------------
   T[] mData = new T[4];    // Stores the queue elements
   int mHead;               // Index of the next element to dequeue
   int mTail;               // Index where the next element is enqueued
   int mUsed;               // Number of elements currently in the queue
   #endregion
}
#endregion