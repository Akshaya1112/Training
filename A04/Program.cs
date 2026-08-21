// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Finds the frequency of all letters in a word list and displays the top 7 most frequent letters.
// ------------------------------------------------------------------------------------------------

using static System.Console;
class Program {
static void Main () {
      string[] words = File.ReadAllLines ("words.txt");
      Dictionary<char, int> freq = [];
      foreach (string word in words)
         foreach (char ch in word.ToUpper ())
            if (freq.TryGetValue (ch, out int value)) freq[ch] = ++value;
            else freq[ch] = 1;
      WriteLine ("Letter Count");
      WriteLine ("------ -----");
      int count = 0;
      foreach (var item in freq.OrderByDescending (x => x.Value).ThenBy (x => x.Key).Take (7)) {
         WriteLine ($"{item.Key,3}{item.Value,9}");
         count++;
      }
   }
}

