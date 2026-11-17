// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// TestCases.cs
// Tests the generic double-ended queue implementation
// ------------------------------------------------------------------------------------------------
using static ESide;
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
      PrintStatus ("DequeueHead on empty queue", Throws (() => sQueue.DequeueHead ()));
      PrintStatus ("DequeueTail on empty queue", Throws (() => sQueue.DequeueTail ()));
      // A queue that was filled and then emptied (its indices have moved)
      sQueue.EnqueueTail (10);
      sQueue.DequeueHead ();
      PrintStatus ("DequeueHead on emptied queue", Throws (() => sQueue.DequeueHead ()));
      PrintStatus ("DequeueTail on emptied queue", Throws (() => sQueue.DequeueTail ()));
      sQueue.Clear ();
   }

   // Tests insertion at tail and removal from head
   static void TestTailHeadOrder () {
      Fill (0, sDataCount, Tail);       // [0, 10, 20, 30], 40 triggers Resize -> [0 ... 60]
      PrintStatus ("EnqueueTail and DequeueHead order", Check (0, sDataCount, Head));
      sQueue.Clear ();
   }

   // Tests insertion at head and removal from tail
   static void TestHeadTailOrder () {
      Fill (0, sDataCount, Head);       // [30, 20, 10, 0], 40 triggers Resize -> [60 ... 0]
      PrintStatus ("EnqueueHead and DequeueTail order", Check (0, sDataCount, Tail));
      sQueue.Clear ();
   }

   // Tests insertion and removal from both ends
   static void TestMixedOrder () {
      sQueue.EnqueueTail (10);                         // [10]
      sQueue.EnqueueHead (20);                         // [20, 10]
      sQueue.EnqueueTail (30);                         // [20, 10, 30]
      sQueue.EnqueueHead (40);                         // [40, 20, 10, 30]
      bool passed = sQueue.DequeueHead () == 40;       // [20, 10, 30]
      passed &= sQueue.DequeueTail () == 30;           // [20, 10]
      sQueue.EnqueueTail (50);                         // [20, 10, 50]
      sQueue.EnqueueHead (60);                         // [60, 20, 10, 50]
      passed &= sQueue.DequeueHead () == 60;           // [20, 10, 50]
      passed &= sQueue.DequeueTail () == 50;           // [20, 10]
      passed &= sQueue.DequeueHead () == 20;           // [10]
      passed &= sQueue.DequeueTail () == 10;           // []
      PrintStatus ("Mixed Enqueue and Dequeue order", passed);
      sQueue.Clear ();
   }

   // Tests circular wrapping when inserting at tail (assumes an initial capacity of 4)
   static void TestTailWrap () {
      Fill (0, 4, Tail);                  // [0, 10, 20, 30], full, tail wrapped to index 0
      sQueue.DequeueHead ();              // [10, 20, 30]
      sQueue.DequeueHead ();              // [20, 30], head at index 2
      Fill (4, 6, Tail);                  // [20, 30, 40, 50], 40 and 50 wrapped to indices 0 and 1
      PrintStatus ("Circular wrap at tail", Check (2, 6, Head));
      sQueue.Clear ();
   }

   // Tests circular wrapping when inserting at head (assumes an initial capacity of 4)
   static void TestHeadWrap () {
      Fill (0, 4, Head);             // [30, 20, 10, 0], full
      sQueue.DequeueTail ();         // [30, 20, 10]
      sQueue.DequeueTail ();         // [30, 20]
      Fill (4, 6, Head);             // [50, 40, 30, 20], 40 and 50 wrapped back to indices 3 and 2
      PrintStatus ("Circular wrap at head", Check (2, 6, Tail));
      sQueue.Clear ();
   }

   // Tests resizing when elements are inserted at tail
   static void TestTailResize () {
      Fill (0, sDataCount, Tail);                       // [0, 10, 20, 30], 40 triggers Resize -> [0 ... 60]
      bool passed = sQueue.Count == sDataCount;
      passed &= Check (0, sDataCount, Head);            // 0 ... 60 in the order added
      PrintStatus ("Resize using EnqueueTail", passed);
      sQueue.Clear ();
   }

   // Tests resizing when elements are inserted at head
   static void TestHeadResize () {
      Fill (0, sDataCount, Head);       // [30, 20, 10, 0], 40 triggers Resize -> [60 ... 0]
      bool passed = sQueue.Count == sDataCount;
      // Head inserts come back out of the head in reverse order
      for (int i = sDataCount - 1; i >= 0; i--) passed &= sQueue.DequeueHead () == sData[i];
      PrintStatus ("Resize using EnqueueHead", passed);
      sQueue.Clear ();
   }

   // Tests resizing (triggered by EnqueueTail) when elements are stored across the circular boundary
   // (assumes an initial capacity of 4)
   static void TestWrappedTailResize () {
      Fill (0, 4, Tail);                   // [0, 10, 20, 30], full
      sQueue.DequeueHead ();               // [10, 20, 30]
      sQueue.DequeueHead ();               // [20, 30]
      Fill (4, sDataCount, Tail);          // [20, 30, 40, 50] full and wrapped, 60 triggers Resize -> [20 ... 60]
      bool passed = sQueue.Count == sDataCount - 2;
      passed &= Check (2, sDataCount, Head);   // 20 ... 60, order kept by Resize
      PrintStatus ("Resize at tail after wrapping", passed);
      sQueue.Clear ();
   }

   // Tests resizing (triggered by EnqueueHead) when elements are stored across the circular boundary
   // (assumes an initial capacity of 4)
   static void TestWrappedHeadResize () {
      Fill (0, 4, Tail);                              // [0, 10, 20, 30], full
      sQueue.DequeueHead ();                          // [10, 20, 30]
      sQueue.DequeueHead ();                          // [20, 30]
      Fill (4, 6, Tail);                              // [20, 30, 40, 50], full and wrapped
      sQueue.EnqueueHead (-1);                        // triggers Resize -> [-1, 20, 30, 40, 50]
      bool passed = sQueue.Count == 5;
      passed &= sQueue.DequeueHead () == -1;          // [20, 30, 40, 50]
      passed &= Check (2, 6, Head);                   // 20 ... 50, order kept by Resize
      PrintStatus ("Resize at head after wrapping", passed);
      sQueue.Clear ();
   }

   // Tests growing more than once (capacity 4 -> 8 -> 16 -> 32)
   static void TestMultipleResize () {
      const int COUNT = 20;
      for (int i = 0; i < COUNT; i++) sQueue.EnqueueTail (i);   // [0 ... 19], Resize at the 5th, 9th and 17th item
      bool passed = sQueue.Count == COUNT;
      for (int i = 0; i < COUNT; i++) passed &= sQueue.DequeueHead () == i;
      PrintStatus ("Multiple resizes", passed);
      sQueue.Clear ();
   }

   // Tests that a queue can be filled again after it has been emptied
   static void TestReuse () {
      Fill (0, 3, Tail);                                     // [0, 10, 20]
      for (int i = 0; i < 3; i++) sQueue.DequeueHead ();     // [], head and tail now at index 3
      Fill (0, sDataCount, Tail);                            // [0, 10, 20, 30] wraps, 40 triggers Resize -> [0 ... 60]
      bool passed = sQueue.Count == sDataCount;
      passed &= Check (0, sDataCount, Head);                 // 0 ... 60 in the order added
      PrintStatus ("Reuse after emptied", passed);
      sQueue.Clear ();
   }

   // Tests whether the queue is empty
   static void TestIsEmpty () {
      PrintStatus ("Initially empty", sQueue.IsEmpty);
      sQueue.EnqueueTail (10);                                      // [10]
      PrintStatus ("Not empty after EnqueueTail", !sQueue.IsEmpty);
      sQueue.DequeueHead ();                                        // []
      PrintStatus ("Empty after DequeueHead", sQueue.IsEmpty);
      sQueue.EnqueueHead (20);                                      // [20]
      PrintStatus ("Not empty after EnqueueHead", !sQueue.IsEmpty);
      sQueue.DequeueTail ();                                        // []
      PrintStatus ("Empty after DequeueTail", sQueue.IsEmpty);
      sQueue.Clear ();
   }

   // Tests whether Count is updated correctly
   static void TestCount () {
      bool passed = sQueue.Count == 0;                 // []
      sQueue.EnqueueTail (10);                         // [10]
      sQueue.EnqueueHead (20);                         // [20, 10]
      sQueue.EnqueueTail (30);                         // [20, 10, 30]
      passed &= sQueue.Count == 3;
      sQueue.DequeueHead ();                           // [10, 30]
      passed &= sQueue.Count == 2;
      sQueue.DequeueTail ();                           // [10]
      passed &= sQueue.Count == 1;
      sQueue.DequeueHead ();                           // []
      passed &= sQueue.Count == 0;
      PrintStatus ("Count updates correctly", passed);
      sQueue.Clear ();
   }
   #endregion

   #region Helpers --------------------------------------------------
   // Enqueues test values from index 'from' until 'to' at the given side of the queue
   static void Fill (int from, int to, ESide side) {
      for (int i = from; i < to; i++) {
         if (side == Tail) sQueue.EnqueueTail (sData[i]);
         else sQueue.EnqueueHead (sData[i]);
      }
   }

   // Dequeues from the given side and compares with test values from 'from' until 'to'
   static bool Check (int from, int to, ESide side) {
      bool passed = true;
      for (int i = from; i < to; i++) {
         int a = side == Head ? sQueue.DequeueHead () : sQueue.DequeueTail ();
         passed &= a == sData[i];
      }
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
   static readonly int[] sData = [0, 10, 20, 30, 40, 50, 60];    // Test Values
   static TDblQueue<int> sQueue = new ();                          // Queue shared by all tests, cleared after each
   static int sPass, sFail, sDataCount = sData.Length;             // Pass and fail counts and  Number of test values
   #endregion
}
#endregion