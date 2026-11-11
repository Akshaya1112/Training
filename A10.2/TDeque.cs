// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// TDeque.cs
// Generic double-ended queue implemented as a resizable circular buffer
// ------------------------------------------------------------------------------------------------

#region Class TDeque ------------------------------------------------------------------------------
/// <summary>Generic double-ended queue implemented as a resizable circular buffer</summary>
class TDeque<T> {
   #region Properties -----------------------------------------------
   /// <summary>Gets a value indicating whether the deque contains no elements</summary>
   public bool IsEmpty => mUsed == 0;

   /// <summary>Gets the number of elements currently in the deque</summary>
   public int Count => mUsed;
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Adds an element at the tail of the deque, grows the buffer if it is full</summary>
   public void EnqueueTail (T a) {
      if (mUsed == mData.Length) Resize ();
      mData[mTail] = a;
      mTail = (mTail + 1) % mData.Length;
      mUsed++;
   }

   /// <summary>Adds an element at the head of the deque, grows the buffer if it is full</summary>
   public void EnqueueHead (T a) {
      if (mUsed == mData.Length) Resize ();
      mHead = (mHead - 1 + mData.Length) % mData.Length;
      mData[mHead] = a;
      mUsed++;
   }

   /// <summary>Removes and returns the element at the head of the deque</summary>
   public T DequeueHead () {
      if (mUsed == 0) throw new InvalidOperationException ("Deque is Empty");
      T a = mData[mHead];
      mData[mHead] = default!;
      mHead = (mHead + 1) % mData.Length;
      mUsed--;
      return a;
   }

   /// <summary>Removes and returns the element at the tail of the deque</summary>
   public T DequeueTail () {
      if (mUsed == 0) throw new InvalidOperationException ("Deque is Empty");
      mTail = (mTail - 1 + mData.Length) % mData.Length;
      T a = mData[mTail];
      mData[mTail] = default!;
      mUsed--;
      return a;
   }
   #endregion

   #region Implementation -------------------------------------------
   // Grows the internal buffer and copies all elements in order, starting at index 0
   void Resize () {
      T[] newData = new T[mData.Length * RESIZEFACTOR];
      for (int i = 0; i < mUsed; i++) newData[i] = mData[(mHead + i) % mData.Length];
      mData = newData;
      mHead = 0;
      mTail = mUsed;
   }
   #endregion

   #region Private data ---------------------------------------------
   T[] mData = new T[INITCAPACITY];       // Circular buffer holding the elements
   int mHead, mTail, mUsed;               // Index of first element, next free slot, element count
   const int INITCAPACITY = 4;            // Initial size of the buffer
   const int RESIZEFACTOR = 2;            // Factor by which the buffer grows when full
   #endregion
}
#endregion