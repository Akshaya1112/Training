// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// TDblQueue.cs
// Generic double-ended queue implemented as a resizable circular buffer
// ------------------------------------------------------------------------------------------------
using static ESide;

#region Class TDblQueue -------------------------------------------------------------------
/// <summary>Generic double-ended queue implemented as a resizable circular buffer</summary>
class TDblQueue<T> {
   #region Properties -----------------------------------------------
   /// <summary>Gets a value indicating whether the queue contains no elements</summary>
   public bool IsEmpty => mUsed == 0;

   /// <summary>Number of elements in the queue</summary>
   public int Count => mUsed;

   // Capacity of the internal circular buffer
   int Capacity => mData.Length;
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Adds an element at the tail of the queue, grows the buffer if it is full</summary>
   public void EnqueueTail (T a) => Enqueue (a, Tail);

   /// <summary>Adds an element at the head of the queue, grows the buffer if it is full</summary>
   public void EnqueueHead (T a) => Enqueue (a, Head);

   /// <summary>Removes and returns the element at the head of the queue</summary>
   public T DequeueHead () => Dequeue (Head);

   /// <summary>Removes and returns the element at the tail of the queue</summary>
   public T DequeueTail () => Dequeue (Tail);
   #endregion

   #region Implementation -------------------------------------------
   // Enqueues an element to the specified side of the queue
   void Enqueue (T a, ESide side) {
      if (mUsed == Capacity) Resize ();
      if (side == Tail) {
         mData[mTail] = a;
         mTail = (mTail + 1) % Capacity;
      } else {
         mHead = (mHead - 1 + Capacity) % Capacity;
         mData[mHead] = a;
      }
      mUsed++;
   }

   // Removes and returns an element from the specified side of the queue
   T Dequeue (ESide side) {
      if (mUsed == 0) throw new InvalidOperationException ("Queue is Empty");
      int index;
      if (side == Head) {
         index = mHead;
         mHead = (mHead + 1) % Capacity;
      } else {
         mTail = (mTail - 1 + Capacity) % Capacity;
         index = mTail;
      }
      T a = mData[index];
      mData[index] = default!;
      mUsed--;
      return a;
   }

   // Grows the internal buffer and copies all elements in order, starting at index 0
   void Resize () {
      T[] temp = new T[Capacity * RESIZEFACTOR];
      for (int i = 0; i < mUsed; i++) temp[i] = mData[(mHead + i) % Capacity];
      mData = temp;
      (mHead, mTail) = (0, mUsed);
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

#region Enum ESide --------------------------------------------------------------------------------
/// <summary>Specifies which end of the double-ended queue to operate on</summary>
enum ESide {
   Head,   // Specifies the head (front) of the queue
   Tail    // Specifies the tail (back) of the queue
}
#endregion