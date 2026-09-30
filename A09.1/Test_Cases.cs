// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Test_Cases.cs
// Tests the generic queue implementation
// ------------------------------------------------------------------------------------------------
using static System.Console;

#region Class Program -----------------------------------------------------------------------------
/// <summary>
/// Tests the generic queue implementation
/// </summary>
class Program {
   #region Implementation -------------------------------------------
   // Runs all queue test cases
   static void Main () {
      TestEmptyQueue ();
      TestSingleElement ();
      TestFifo ();
      TestTailLessThanHead ();
      TestResize ();
      TestWrappedResize ();
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

   // Tests adding and removing a single element
   static void TestSingleElement () {
      TQueue<int> queue = new ();
      queue.Enqueue (10);
      PrintStatus ("Single element", queue.Dequeue () == 10);
   }

   // Tests if elements are removed in FIFO order
   static void TestFifo () {
      TQueue<int> queue = new ();
      for (int n = 0; n < 4; n++) queue.Enqueue (n * 10);
      bool passed = true;
      for (int n = 0; n < 4; n++) passed &= queue.Dequeue () == n * 10;
      PrintStatus ("FIFO order", passed);
   }

   // Tests the circular buffer when the tail wraps before the head
   static void TestTailLessThanHead () {
      TQueue<int> queue = new ();
      for (int n = 0; n < 4; n++) queue.Enqueue (n * 10);
      queue.Dequeue ();
      queue.Dequeue ();
      queue.Enqueue (40);
      bool passed = true;
      for (int n = 2; n < 5; n++) passed &= queue.Dequeue () == n * 10;
      PrintStatus ("mTail < mHead", passed);
   }

   // Tests that the queue grows when the buffer becomes full
   static void TestResize () {
      TQueue<int> queue = new ();
      for (int n = 0; n < 5; n++) queue.Enqueue (n * 10);
      bool passed = true;
      for (int n = 0; n < 5; n++) passed &= queue.Dequeue () == n * 10;
      PrintStatus ("Resize", passed);
   }

   // Tests resizing when the circular buffer has wrapped around
   static void TestWrappedResize () {
      TQueue<int> queue = new ();
      for (int n = 0; n < 4; n++) queue.Enqueue (n * 10);
      queue.Dequeue ();
      queue.Dequeue ();
      for (int n = 4; n < 7; n++) queue.Enqueue (n * 10);
      bool passed = true;
      for (int n = 2; n < 7; n++) passed &= queue.Dequeue () == n * 10;
      PrintStatus ("Wrapped resize", passed);
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
}
#endregion