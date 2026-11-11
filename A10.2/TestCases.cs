// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// TestCases.cs
// Tests the generic double-ended queue implementation
// ------------------------------------------------------------------------------------------------
using static System.Console;

#region Class TestCases ---------------------------------------------------------------------------
/// <summary>Tests the generic double-ended queue implementation</summary>
class TestCases {
   #region Implementation -------------------------------------------
   // Runs all double-ended queue test cases
   static void Main () {
      TestEmptyThrows ();
      TestTailHeadOrder ();
      TestHeadTailOrder ();
      TestMixedOrder ();
      TestTailWrap ();
      TestHeadWrap ();
      TestTailResize ();
      TestHeadResize ();
      TestWrappedTailResize ();
      TestWrappedHeadResize ();
      TestMultipleResize ();
      TestReuse ();
      TestIsEmpty ();
      TestCount ();
      WriteLine ($"\nPassed: {sPass}, Failed: {sFail}");
   }

   // Tests if dequeue operations throw an exception for an empty queue
   static void TestEmptyThrows () {
      TDblQueue<int> queue = new ();
      PrintStatus ("DequeueHead on empty queue", Throws (() => queue.DequeueHead ()));
      PrintStatus ("DequeueTail on empty queue", Throws (() => queue.DequeueTail ()));
      // A queue that was filled and then emptied (its indices have moved)
      queue.EnqueueTail (10);
      queue.DequeueHead ();
      PrintStatus ("DequeueHead on emptied queue", Throws (() => queue.DequeueHead ()));
      PrintStatus ("DequeueTail on emptied queue", Throws (() => queue.DequeueTail ()));
   }

   // Tests insertion at tail and removal from head
   static void TestTailHeadOrder () {
      TDblQueue<int> queue = new ();
      FillTail (queue, 0, sData.Length);       // [0, 10, 20, 30], 40 triggers Resize -> [0 ... 60]
      PrintStatus ("EnqueueTail and DequeueHead order", CheckHead (queue, 0, sData.Length));
   }

   // Tests insertion at head and removal from tail
   static void TestHeadTailOrder () {
      TDblQueue<int> queue = new ();
      FillHead (queue, 0, sData.Length);       // [30, 20, 10, 0], 40 triggers Resize -> [60 ... 0]
      PrintStatus ("EnqueueHead and DequeueTail order", CheckTail (queue, 0, sData.Length));
   }

   // Tests insertion and removal from both ends
   static void TestMixedOrder () {
      TDblQueue<int> queue = new ();
      queue.EnqueueTail (10);                         // [10]
      queue.EnqueueHead (20);                         // [20, 10]
      queue.EnqueueTail (30);                         // [20, 10, 30]
      queue.EnqueueHead (40);                         // [40, 20, 10, 30]
      bool passed = queue.DequeueHead () == 40;       // [20, 10, 30]
      passed &= queue.DequeueTail () == 30;           // [20, 10]
      queue.EnqueueTail (50);                         // [20, 10, 50]
      queue.EnqueueHead (60);                         // [60, 20, 10, 50]
      passed &= queue.DequeueHead () == 60;           // [20, 10, 50]
      passed &= queue.DequeueTail () == 50;           // [20, 10]
      passed &= queue.DequeueHead () == 20;           // [10]
      passed &= queue.DequeueTail () == 10;           // []
      PrintStatus ("Mixed Enqueue and Dequeue order", passed);
   }

   // Tests circular wrapping when inserting at tail (assumes an initial capacity of 4)
   static void TestTailWrap () {
      TDblQueue<int> queue = new ();
      FillTail (queue, 0, 4);             // [0, 10, 20, 30], full, tail wrapped to index 0
      queue.DequeueHead ();               // [10, 20, 30]
      queue.DequeueHead ();               // [20, 30], head at index 2
      FillTail (queue, 4, 6);             // [20, 30, 40, 50], 40 and 50 wrapped to indices 0 and 1
      PrintStatus ("Circular wrap at tail", CheckHead (queue, 2, 6));
   }

   // Tests circular wrapping when inserting at head (assumes an initial capacity of 4)
   static void TestHeadWrap () {
      TDblQueue<int> queue = new ();
      FillHead (queue, 0, 4);        // [30, 20, 10, 0], full
      queue.DequeueTail ();          // [30, 20, 10]
      queue.DequeueTail ();          // [30, 20]
      FillHead (queue, 4, 6);        // [50, 40, 30, 20], 40 and 50 wrapped back to indices 3 and 2
      PrintStatus ("Circular wrap at head", CheckTail (queue, 2, 6));
   }

   // Tests resizing when elements are inserted at tail
   static void TestTailResize () {
      TDblQueue<int> queue = new ();
      FillTail (queue, 0, sData.Length);              // [0, 10, 20, 30], 40 triggers Resize -> [0 ... 60]
      bool passed = queue.Count == sData.Length;
      passed &= CheckHead (queue, 0, sData.Length);   // 0 ... 60 in the order added
      PrintStatus ("Resize using EnqueueTail", passed);
   }

   // Tests resizing when elements are inserted at head
   static void TestHeadResize () {
      TDblQueue<int> queue = new ();
      FillHead (queue, 0, sData.Length);       // [30, 20, 10, 0], 40 triggers Resize -> [60 ... 0]
      bool passed = queue.Count == sData.Length;
      // Head inserts come back out of the head in reverse order
      for (int i = sData.Length - 1; i >= 0; i--) passed &= queue.DequeueHead () == sData[i];
      PrintStatus ("Resize using EnqueueHead", passed);
   }

   // Tests resizing (triggered by EnqueueTail) when elements are stored across the circular boundary
   // (assumes an initial capacity of 4)
   static void TestWrappedTailResize () {
      TDblQueue<int> queue = new ();
      FillTail (queue, 0, 4);              // [0, 10, 20, 30], full
      queue.DequeueHead ();                // [10, 20, 30]
      queue.DequeueHead ();                // [20, 30]
      FillTail (queue, 4, sData.Length);   // [20, 30, 40, 50] full and wrapped, 60 triggers Resize -> [20 ... 60]
      bool passed = queue.Count == sData.Length - 2;
      passed &= CheckHead (queue, 2, sData.Length);   // 20 ... 60, order kept by Resize
      PrintStatus ("Resize at tail after wrapping", passed);
   }

   // Tests resizing (triggered by EnqueueHead) when elements are stored across the circular boundary
   // (assumes an initial capacity of 4)
   static void TestWrappedHeadResize () {
      TDblQueue<int> queue = new ();
      FillTail (queue, 0, 4);                         // [0, 10, 20, 30], full
      queue.DequeueHead ();                           // [10, 20, 30]
      queue.DequeueHead ();                           // [20, 30]
      FillTail (queue, 4, 6);                         // [20, 30, 40, 50], full and wrapped
      queue.EnqueueHead (-1);                         // triggers Resize -> [-1, 20, 30, 40, 50]
      bool passed = queue.Count == 5;
      passed &= queue.DequeueHead () == -1;           // [20, 30, 40, 50]
      passed &= CheckHead (queue, 2, 6);              // 20 ... 50, order kept by Resize
      PrintStatus ("Resize at head after wrapping", passed);
   }

   // Tests growing more than once (capacity 4 -> 8 -> 16 -> 32)
   static void TestMultipleResize () {
      const int count = 20;
      TDblQueue<int> queue = new ();
      for (int i = 0; i < count; i++) queue.EnqueueTail (i);   // [0 ... 19], Resize at the 5th, 9th and 17th item
      bool passed = queue.Count == count;
      for (int i = 0; i < count; i++) passed &= queue.DequeueHead () == i;
      PrintStatus ("Multiple resizes", passed);
   }

   // Tests that a queue can be filled again after it has been emptied
   static void TestReuse () {
      TDblQueue<int> queue = new ();
      FillTail (queue, 0, 3);                            // [0, 10, 20]
      for (int i = 0; i < 3; i++) queue.DequeueHead ();  // [], head and tail now at index 3
      FillTail (queue, 0, sData.Length);                 // [0, 10, 20, 30] wraps, 40 triggers Resize -> [0 ... 60]
      bool passed = queue.Count == sData.Length;
      passed &= CheckHead (queue, 0, sData.Length);      // 0 ... 60 in the order added
      PrintStatus ("Reuse after emptied", passed);
   }

   // Tests whether the queue is empty
   static void TestIsEmpty () {
      TDblQueue<int> queue = new ();
      PrintStatus ("Initially empty", queue.IsEmpty);
      queue.EnqueueTail (10);                                      // [10]
      PrintStatus ("Not empty after EnqueueTail", !queue.IsEmpty);
      queue.DequeueHead ();                                        // []
      PrintStatus ("Empty after DequeueHead", queue.IsEmpty);
      queue.EnqueueHead (20);                                      // [20]
      PrintStatus ("Not empty after EnqueueHead", !queue.IsEmpty);
      queue.DequeueTail ();                                        // []
      PrintStatus ("Empty after DequeueTail", queue.IsEmpty);
   }

   // Tests whether Count is updated correctly
   static void TestCount () {
      TDblQueue<int> queue = new ();
      bool passed = queue.Count == 0;                 // []
      queue.EnqueueTail (10);                         // [10]
      queue.EnqueueHead (20);                         // [20, 10]
      queue.EnqueueTail (30);                         // [20, 10, 30]
      passed &= queue.Count == 3;
      queue.DequeueHead ();                           // [10, 30]
      passed &= queue.Count == 2;
      queue.DequeueTail ();                           // [10]
      passed &= queue.Count == 1;
      queue.DequeueHead ();                           // []
      passed &= queue.Count == 0;
      PrintStatus ("Count updates correctly", passed);
   }
   #endregion

   #region Helpers --------------------------------------------------
   // Enqueues test values from index 'from' until 'to' at the queue's tail
   static void FillTail (TDblQueue<int> queue, int from, int to) {
      for (int i = from; i < to; i++) queue.EnqueueTail (sData[i]);
   }

   // Enqueues test values from index 'from' until 'to' at the queue's head
   static void FillHead (TDblQueue<int> queue, int from, int to) {
      for (int i = from; i < to; i++) queue.EnqueueHead (sData[i]);
   }

   // Dequeues from the head and compares with test values from 'from' until 'to'
   static bool CheckHead (TDblQueue<int> queue, int from, int to) {
      bool passed = true;
      for (int i = from; i < to; i++) passed &= queue.DequeueHead () == sData[i];
      return passed;
   }

   // Dequeues from the tail and compares with test values from 'from' until 'to'
   static bool CheckTail (TDblQueue<int> queue, int from, int to) {
      bool passed = true;
      for (int i = from; i < to; i++) passed &= queue.DequeueTail () == sData[i];
      return passed;
   }

   // Returns true if the given action throws an InvalidOperationException
   static bool Throws (Action action) {
      try { action (); } catch (InvalidOperationException) { return true; }
      return false;
   }

   // Displays the test status and updates the pass/fail counts
   static void PrintStatus (string name, bool passed) {
      if (passed) sPass++; else sFail++;
      Write ("[");
      ForegroundColor = passed ? ConsoleColor.Green : ConsoleColor.Red;
      Write (passed ? "PASS" : "FAIL");
      ResetColor ();
      WriteLine ($"] {name}");
   }
   #endregion

   #region Private data ---------------------------------------------
   static readonly int[] sData = { 0, 10, 20, 30, 40, 50, 60 };      // Test Values
   static int sPass, sFail;                                          // Pass and fail counts
   #endregion
}
#endregion