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
   #region Constructors ---------------------------------------------
   /// <summary>Loads the possible secret words and all valid words</summary>
   public WordBank () {
      mGuessWords = ReadWords (GUESSFILE);
      mValidWords = [.. ReadWords (VALIDFILE).Concat (mGuessWords)];
   }
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>True if the word is a valid guess</summary>
   public bool IsValid (string word) => mValidWords.Contains (word);

   /// <summary>Returns a random word from the guess list</summary>
   public string PickSecretWord () => mGuessWords[mRandom.Next (mGuessWords.Length)];
   #endregion

   #region Implementation -------------------------------------------
   // Reads words from an embedded resource file
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
   readonly string[] mGuessWords;                    // Possible secret words
   readonly HashSet<string> mValidWords;             // All valid words with fast lookup
   readonly Random mRandom = new ();
   #endregion
}
#endregion