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
      Dictionary<char, int> freq = [];
      foreach (char ch in File.ReadAllText ("words.txt").ToUpper ())
         if (char.IsLetter (ch))
            freq[ch] = freq.GetValueOrDefault (ch) + 1;
      WriteLine ("Letter | Count");
      WriteLine ("-------+------");
      foreach (var item in freq.OrderByDescending (x => x.Value).Take (7))
         WriteLine ($"{item.Key,6} | {item.Value,5}");
   }
}