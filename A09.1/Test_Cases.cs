// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Test_Cases.cs
// Tests the generic queue implementation
// ------------------------------------------------------------------------------------------------
using static System.Console;

#region Class Program -----------------------------------------------------------------------------
/// <summary>Tests the generic queue implementation</summary>
class Program {
   #region Implementation -------------------------------------------
   // Runs all queue test cases
   static void Main () {
      TestEmptyQueue ();
      TestOrder ("Single element", 1);
      TestOrder ("FIFO order", 4);
      TestOrder ("mTail < mHead", 5, skip: 2, split: 4);
      TestOrder ("Resize", 5);
      TestOrder ("Wrapped resize", 7, skip: 2, split: 4);
      TestIsEmpty ();
   }

   // Tests if dequeue throws an exception for an empty queue
   static void TestEmptyQueue () {
      TQueue<int> queue = new ();
      bool passed = false;
      try {
         queue.Dequeue ();
      } catch (InvalidOperationException) {
         passed = true;
      }
      PrintStatus ("Dequeue empty queue", passed);
   }

   // Enqueues sData[0..count) and checks they dequeue in the same order
   // skip: elements dequeued midway; split: elements enqueued before that (default: all)
   static void TestOrder (string name, int count, int skip = 0, int split = -1) {
      if (split < 0) split = count;
      TQueue<int> queue = new ();
      for (int i = 0; i < split; i++) queue.Enqueue (sData[i]);
      for (int i = 0; i < skip; i++) queue.Dequeue ();
      for (int i = split; i < count; i++) queue.Enqueue (sData[i]);
      bool passed = true;
      for (int i = skip; i < count; i++) passed &= queue.Dequeue () == sData[i];
      PrintStatus (name, passed);
   }

   // Tests whether the queue is empty
   static void TestIsEmpty () {
      TQueue<int> queue = new ();
      queue.Enqueue (10);
      PrintStatus ("Not empty after Enqueue", !queue.IsEmpty);
      queue.Dequeue ();
      PrintStatus ("Empty after Dequeue", queue.IsEmpty);
   }

   // Displays the test status
   static void PrintStatus (string name, bool passed) {
      Write ("[");
      ForegroundColor = passed ? ConsoleColor.Green : ConsoleColor.Red;
      Write (passed ? "PASS" : "FAIL");
      ResetColor ();
      WriteLine ($"] {name}");
   }
   #endregion

   #region Private data ---------------------------------------------
   static readonly int[] sData = { 0, 10, 20, 30, 40, 50, 60 };    // Test Values
   #endregion
}
#endregion