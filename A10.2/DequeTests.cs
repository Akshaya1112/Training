// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// DequeTests.cs
// Tests the generic deque implementation
// ------------------------------------------------------------------------------------------------
using static System.Console;

#region Class DequeTests -----------------------------------------------------------------------------
/// <summary>Tests the generic deque implementation</summary>
class DequeTests {
   #region Implementation -------------------------------------------
   // Runs all deque test cases
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

   // Tests if dequeue operations throw an exception for an empty deque
   static void TestEmptyThrows () {
      TDeque<int> deque = new ();
      PrintStatus ("DequeueHead on empty deque", Throws (() => deque.DequeueHead ()));
      PrintStatus ("DequeueTail on empty deque", Throws (() => deque.DequeueTail ()));
      // A deque that was filled and then emptied (its indices have moved)
      deque.EnqueueTail (10);
      deque.DequeueHead ();
      PrintStatus ("DequeueHead on emptied deque", Throws (() => deque.DequeueHead ()));
      PrintStatus ("DequeueTail on emptied deque", Throws (() => deque.DequeueTail ()));
   }

   // Tests insertion at tail and removal from head
   static void TestTailHeadOrder () {
      TDeque<int> deque = new ();
      FillTail (deque, 0, sData.Length);
      PrintStatus ("EnqueueTail and DequeueHead order", CheckHead (deque, 0, sData.Length));
   }

   // Tests insertion at head and removal from tail
   static void TestHeadTailOrder () {
      TDeque<int> deque = new ();
      FillHead (deque, 0, sData.Length);
      PrintStatus ("EnqueueHead and DequeueTail order", CheckTail (deque, 0, sData.Length));
   }

   // Tests insertion and removal from both ends
   static void TestMixedOrder () {
      TDeque<int> deque = new ();
      deque.EnqueueTail (10);
      deque.EnqueueHead (20);
      deque.EnqueueTail (30);
      deque.EnqueueHead (40);
      bool passed = deque.DequeueHead () == 40;
      passed &= deque.DequeueTail () == 30;
      deque.EnqueueTail (50);
      deque.EnqueueHead (60);
      passed &= deque.DequeueHead () == 60;
      passed &= deque.DequeueTail () == 50;
      passed &= deque.DequeueHead () == 20;
      passed &= deque.DequeueTail () == 10;
      PrintStatus ("Mixed Enqueue and Dequeue order", passed);
   }

   // Tests circular wrapping when inserting at tail (assumes an initial capacity of 4)
   static void TestTailWrap () {
      TDeque<int> deque = new ();
      FillTail (deque, 0, 4);
      deque.DequeueHead ();
      deque.DequeueHead ();
      FillTail (deque, 4, 6);
      PrintStatus ("Circular wrap at tail", CheckHead (deque, 2, 6));
   }

   // Tests circular wrapping when inserting at head (assumes an initial capacity of 4)
   static void TestHeadWrap () {
      TDeque<int> deque = new ();
      FillHead (deque, 0, 4);
      deque.DequeueTail ();
      deque.DequeueTail ();
      FillHead (deque, 4, 6);
      PrintStatus ("Circular wrap at head", CheckTail (deque, 2, 6));
   }

   // Tests resizing when elements are inserted at tail
   static void TestTailResize () {
      TDeque<int> deque = new ();
      FillTail (deque, 0, sData.Length);
      bool passed = deque.Count == sData.Length;
      passed &= CheckHead (deque, 0, sData.Length);
      PrintStatus ("Resize using EnqueueTail", passed);
   }

   // Tests resizing when elements are inserted at head
   static void TestHeadResize () {
      TDeque<int> deque = new ();
      FillHead (deque, 0, sData.Length);
      bool passed = deque.Count == sData.Length;
      // Head inserts come back out of the head in reverse order
      for (int i = sData.Length - 1; i >= 0; i--) passed &= deque.DequeueHead () == sData[i];
      PrintStatus ("Resize using EnqueueHead", passed);
   }

   // Tests resizing (triggered by EnqueueTail) when elements are stored across the circular boundary
   // (assumes an initial capacity of 4)
   static void TestWrappedTailResize () {
      TDeque<int> deque = new ();
      FillTail (deque, 0, 4);
      deque.DequeueHead ();
      deque.DequeueHead ();
      FillTail (deque, 4, sData.Length);
      bool passed = deque.Count == sData.Length - 2;
      passed &= CheckHead (deque, 2, sData.Length);
      PrintStatus ("Resize at tail after wrapping", passed);
   }

   // Tests resizing (triggered by EnqueueHead) when elements are stored across the circular boundary
   // (assumes an initial capacity of 4)
   static void TestWrappedHeadResize () {
      TDeque<int> deque = new ();
      FillTail (deque, 0, 4);
      deque.DequeueHead ();
      deque.DequeueHead ();
      FillTail (deque, 4, 6);
      deque.EnqueueHead (-1);
      bool passed = deque.Count == 5;
      passed &= deque.DequeueHead () == -1;
      passed &= CheckHead (deque, 2, 6);
      PrintStatus ("Resize at head after wrapping", passed);
   }

   // Tests growing more than once (capacity 4 -> 8 -> 16 -> 32)
   static void TestMultipleResize () {
      const int count = 20;
      TDeque<int> deque = new ();
      for (int i = 0; i < count; i++) deque.EnqueueTail (i);
      bool passed = deque.Count == count;
      for (int i = 0; i < count; i++) passed &= deque.DequeueHead () == i;
      PrintStatus ("Multiple resizes", passed);
   }

   // Tests that a deque can be filled again after it has been emptied
   static void TestReuse () {
      TDeque<int> deque = new ();
      FillTail (deque, 0, 3);
      for (int i = 0; i < 3; i++) deque.DequeueHead ();
      FillTail (deque, 0, sData.Length);
      bool passed = deque.Count == sData.Length;
      passed &= CheckHead (deque, 0, sData.Length);
      PrintStatus ("Reuse after emptied", passed);
   }

   // Tests whether the deque is empty
   static void TestIsEmpty () {
      TDeque<int> deque = new ();
      PrintStatus ("Initially empty", deque.IsEmpty);
      deque.EnqueueTail (10);
      PrintStatus ("Not empty after EnqueueTail", !deque.IsEmpty);
      deque.DequeueHead ();
      PrintStatus ("Empty after DequeueHead", deque.IsEmpty);
      deque.EnqueueHead (20);
      PrintStatus ("Not empty after EnqueueHead", !deque.IsEmpty);
      deque.DequeueTail ();
      PrintStatus ("Empty after DequeueTail", deque.IsEmpty);
   }

   // Tests whether Count is updated correctly
   static void TestCount () {
      TDeque<int> deque = new ();
      bool passed = deque.Count == 0;
      deque.EnqueueTail (10);
      deque.EnqueueHead (20);
      deque.EnqueueTail (30);
      passed &= deque.Count == 3;
      deque.DequeueHead ();
      passed &= deque.Count == 2;
      deque.DequeueTail ();
      passed &= deque.Count == 1;
      deque.DequeueHead ();
      passed &= deque.Count == 0;
      PrintStatus ("Count updates correctly", passed);
   }
   #endregion

   #region Helpers --------------------------------------------------
   // Enqueues sData[from..to) at the tail
   static void FillTail (TDeque<int> deque, int from, int to) {
      for (int i = from; i < to; i++) deque.EnqueueTail (sData[i]);
   }

   // Enqueues sData[from..to) at the head
   static void FillHead (TDeque<int> deque, int from, int to) {
      for (int i = from; i < to; i++) deque.EnqueueHead (sData[i]);
   }

   // Dequeues from the head and checks the values against sData[from..to)
   static bool CheckHead (TDeque<int> deque, int from, int to) {
      bool passed = true;
      for (int i = from; i < to; i++) passed &= deque.DequeueHead () == sData[i];
      return passed;
   }

   // Dequeues from the tail and checks the values against sData[from..to)
   static bool CheckTail (TDeque<int> deque, int from, int to) {
      bool passed = true;
      for (int i = from; i < to; i++) passed &= deque.DequeueTail () == sData[i];
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