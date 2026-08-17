// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch- July 2026.
// Copyright (c) Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Spelling Bee - Finds valid words from a given set of letters.
// ------------------------------------------------------------------------------------------------
class Program {
   record ScoredWord (string word, bool IsPangram, int Score);
   static void Main () {
      char[] letters = { 'U', 'X', 'A', 'L', 'T', 'N', 'E' };
      string[] words = File.ReadAllLines ("words.txt");
      List<ScoredWord> validWords = [];
      foreach (var ln in words) {
         string w = ln.Trim ().ToUpper ();
         if (!IsValid (w)) continue;
         bool isPangram = IsPangram (w);
         int score = CalculateScore (w, isPangram);
         validWords.Add (new (w, isPangram, score));
      }
      validWords.Sort ((a, b) => {
         int c = b.Score.CompareTo (a.Score);
         return c != 0 ? c : a.word.CompareTo (b.word);
      });
      int total = 0;
      foreach (var vw in validWords) {
         if (vw.IsPangram) Console.ForegroundColor = ConsoleColor.Green;
         else Console.ResetColor ();
         Console.WriteLine ("{0,2}. {1}", vw.Score, vw.word);
         total += vw.Score;
      }
      Console.WriteLine ($"---------\n{total} total");

      // Helper functions -------------------------------------------
      bool IsValid (string word)
         => word.Length >= 4 && word.Contains (letters[0]) && word.All (letters.Contains);

      bool IsPangram (string word) => letters.All (word.Contains);

      int CalculateScore (string word, bool pangram)
         => word.Length == 4 ? 1 : pangram ? word.Length + 7 : word.Length;
   }
}