// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch- July 2026.
// Copyright (c) Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Spelling Bee - Finds valid words from a given set of letters.
// ------------------------------------------------------------------------------------------------
class Program {
   static void Main () {
      char[] letters = { 'U', 'X', 'A', 'L', 'T', 'N', 'E' };
      string[] words = File.ReadAllLines ("words.txt");
      List<string> validWords = [];
      List<bool> pangrams = [];
      List<int> scores = [];
      for (int i = 0; i < words.Length; i++) {
         string w = words[i].ToUpper ();
         if (IsValid (w)) {
            validWords.Add (w);
            bool p = IsPangram (w);
            pangrams.Add (p);
            int s = CalculateScore (w, p);
            scores.Add (s);
         }
      }
      for (int i = 0; i < validWords.Count - 1; i++) {
         for (int j = 0; j < validWords.Count - 1 - i; j++) {
            if (scores[j] < scores[j + 1] || (scores[j] == scores[j + 1]
               && validWords[j].CompareTo (validWords[j + 1]) > 0)) {
               (scores[j + 1], scores[j]) = (scores[j], scores[j + 1]);
               (validWords[j + 1], validWords[j]) = (validWords[j], validWords[j + 1]);
               (pangrams[j + 1], pangrams[j]) = (pangrams[j], pangrams[j + 1]);
            }
         }
      }
      int total = 0;
      for (int i = 0; i < validWords.Count; i++) {
         if (pangrams[i]) Console.ForegroundColor = ConsoleColor.Green;
         else Console.ResetColor ();
         Console.WriteLine ("{0,2}. {1}", scores[i], validWords[i]);
         total += scores[i];
      }
      Console.WriteLine ($"---------\n{total} total");

      // Helper functions -------------------------------------------
      bool IsValid (string word) {
         if (word.Length < 4) return false;
         if (!word.Contains (letters[0])) return false;
         for (int i = 0; i < word.Length; i++)
            if (!letters.Contains (word[i])) return false;
         return true;
      }

      bool IsPangram (string word) {
         for (int i = 0; i < letters.Length; i++)
            if (!word.Contains (letters[i])) return false;
         return true;
      }

      int CalculateScore (string word, bool pangram) {
         int score = word.Length == 4 ? 1 : word.Length;
         if (pangram) score += 7;
         return score;
      }
   }
}