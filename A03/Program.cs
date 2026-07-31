namespace A03;
class Program {
   static void Main (string[] args) {
      char[] letters = { 'U', 'X', 'A', 'L', 'T', 'N', 'E' };
      string[] words = File.ReadAllLines (@"C:\Work\words 1.txt");
      List<string> validWords = new List<string> ();
      List<bool> pangrams = new List<bool> ();
      List<int> scores = new List<int> ();
      for (int i = 0;i < words.Length; i++) { 
         string w = words[i].ToUpper();
         if (IsValid (w, letters)) {
            validWords.Add (w);
            bool p = IsPangram (w, letters);
            pangrams.Add (p);
            int s = calculateScore (w, p);
            scores.Add (s);
         }
      }
      for(int i = 0; i < validWords.Count - 1; i++) {
         for(int j = 0; j < validWords.Count - 1; j++) {
            if (scores[j] < scores[j + 1] || scores[j] == scores[j + 1] && validWords[j].CompareTo (validWords[j + 1]) > 0) {
               int tempScore = scores[j];
               scores[j] = scores[j + 1];
               scores[j+1]= tempScore;
               string tempWord = validWords[j];
               validWords[j] = validWords[j + 1];
               validWords[j+1]=tempWord;
               bool tempPangram = pangrams[j];
               pangrams[j]=pangrams[j + 1];
               pangrams[j + 1] = tempPangram;
            }  
         }
      }
      int Total = 0;
      for (int i = 0; i < validWords.Count; i++) {
         if (pangrams[i])
            Console.ForegroundColor = ConsoleColor.Green;
         else
            Console.ResetColor ();
         Console.WriteLine("{0,2}. {1}",scores[i],validWords[i]);
         Total += scores[i];
      }
      Console.WriteLine ("---------");
      Console.WriteLine (Total +" total");
   }
   static bool IsAllowed (char c, char[] letters) {
      for (int i = 0; i < letters.Length; i++)
         if (c == letters[i])
            return true;
      return false;
   }
   static bool IsValid (string word, char[] letters) { 
      if(word.Length<4)
         return false;
      if (!word.Contains (letters[0]))
         return false;
      for (int i = 0;i<word.Length;i++)
         if (IsAllowed (word[i],letters)==false)
            return false;
      return true;
   }
   static bool IsPangram(string word, char[] letters) {
      for (int i = 0; i < letters.Length; i++) {
         if (!word.Contains (letters[i]))
            return false;
      }
      return true;
   }
   static int calculateScore(string word,bool pangram) {
      int score;
      if (word.Length == 4)
         score = 1;
      else
         score = word.Length;
      if (pangram)
         score += 7;
      return score;
   }
}