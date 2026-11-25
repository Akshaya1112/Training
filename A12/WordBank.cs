// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// WordBank.cs
// Loads the word lists with the possible secret words and all the valid words
// ------------------------------------------------------------------------------------------------
using System.Reflection;

#region Class WordBank ----------------------------------------------------------------------------
/// <summary>Loads the word lists with the possible secret words and all the valid words</summary>
class WordBank {
   #region Methods --------------------------------------------------
   /// <summary>Loads the puzzle list and the dictionary</summary>
   public WordBank () {
      mGuessWords = ReadWords (GUESSFILE);
      mValidWords = [.. ReadWords (VALIDFILE).Concat (mGuessWords)];
   }

   /// <summary>Returns a random word from the puzzle list</summary>
   public string PickSecretWord () => mGuessWords[mRandom.Next (mGuessWords.Length)];

   /// <summary>True if the upper case word is a valid guess</summary>
   public bool IsValid (string word) => mValidWords.Contains (word);
   #endregion

   #region Implementation -------------------------------------------
   // Reads the five letter words in upper case from a file
   static string[] ReadWords (string file) {
      string resource = Assembly.GetExecutingAssembly ()
                                .GetManifestResourceNames ()
                                .First (a => a.EndsWith (file));
      using Stream stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream (resource)!;
      using StreamReader reader = new (stream);
      return [.. reader.ReadToEnd ().Split (Environment.NewLine)];
   }
   #endregion

   #region Constants ------------------------------------------------
   const string GUESSFILE = "guess.txt";              // Words that can be the secret word
   const string VALIDFILE = "Validate.txt";           // All valid words
   #endregion

   #region Private data ---------------------------------------------
   readonly string[] mGuessWords;                    // Possible secret words with random access
   readonly HashSet<string> mValidWords;             // All valid words with fast lookup
   readonly Random mRandom = new ();
   #endregion
}
#endregion