// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Test_Cases.cs
// Tests the generic queue implementation
// ------------------------------------------------------------------------------------------------
using static System.Console;

class Program {
   static void Main () {
      TestEmptyQueue ();
      TestSingleElement ();
      TestFifo ();
      TestHeadLessThanTail ();
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
      queue.Enqueue (10);
      queue.Enqueue (20);
      queue.Enqueue (30);
      bool passed = queue.Dequeue () == 10
         && queue.Dequeue () == 20
         && queue.Dequeue () == 30;
      PrintStatus ("FIFO order", passed);
   }

   // Tests the queue when the head index is before the tail index
   static void TestHeadLessThanTail () {
      TQueue<int> queue = new ();
      queue.Enqueue (10);
      queue.Enqueue (20);
      queue.Enqueue (30);
      bool passed = queue.Dequeue () == 10
         && queue.Dequeue () == 20
         && queue.Dequeue () == 30;
      PrintStatus ("mHead < mTail", passed);
   }

   // Tests the circular buffer when the tail wraps before the head
   static void TestTailLessThanHead () {
      TQueue<int> queue = new ();
      queue.Enqueue (10);
      queue.Enqueue (20);
      queue.Enqueue (30);
      queue.Enqueue (40);
      queue.Dequeue ();
      queue.Dequeue ();
      queue.Enqueue (50);
      queue.Enqueue (60);
      bool passed = queue.Dequeue () == 30
         && queue.Dequeue () == 40
         && queue.Dequeue () == 50
         && queue.Dequeue () == 60;
      PrintStatus ("mTail < mHead", passed);
   }

   // Tests that the queue grows when the buffer becomes full
   static void TestResize () {
      TQueue<int> queue = new ();
      queue.Enqueue (10);
      queue.Enqueue (20);
      queue.Enqueue (30);
      queue.Enqueue (40);
      queue.Enqueue (50);
      bool passed = queue.Dequeue () == 10
         && queue.Dequeue () == 20
         && queue.Dequeue () == 30
         && queue.Dequeue () == 40
         && queue.Dequeue () == 50;
      PrintStatus ("Resize", passed);
   }

   // Tests resizing when the circular buffer has wrapped around
   static void TestWrappedResize () {
      TQueue<int> queue = new ();
      queue.Enqueue (10);
      queue.Enqueue (20);
      queue.Enqueue (30);
      queue.Enqueue (40);
      queue.Dequeue ();
      queue.Dequeue ();
      queue.Enqueue (50);
      queue.Enqueue (60);
      queue.Enqueue (70);
      bool passed = queue.Dequeue () == 30
         && queue.Dequeue () == 40
         && queue.Dequeue () == 50
         && queue.Dequeue () == 60
         && queue.Dequeue () == 70;
      PrintStatus ("Wrapped resize", passed);
   }

   // Tests IsEmpty before and after adding and removing an element
   static void TestIsEmpty () {
      TQueue<int> queue = new ();
      bool passed = queue.IsEmpty;
      queue.Enqueue (10);
      passed &= !queue.IsEmpty;
      queue.Dequeue ();
      passed &= queue.IsEmpty;
      PrintStatus ("IsEmpty", passed);
   }

   // Displays status
   static void PrintStatus (string name, bool passed) {
      Write ("[");
      ForegroundColor = passed ? ConsoleColor.Green : ConsoleColor.Red;
      Write (passed ? "PASS" : "FAIL");
      ResetColor ();
      WriteLine ($"] {name}");
   }
}