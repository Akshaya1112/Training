// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Finds all anagram groups in a word list and prints them sorted by group size in descending order
// ------------------------------------------------------------------------------------------------
using System.Reflection;

#region Class Program -----------------------------------------------------------------------------
/// <summary>Finds all anagram groups and prints them sorted by group size</summary>
class Program {
   #region Methods --------------------------------------------------
   /// <summary>Reads the word list, groups the anagrams and prints them</summary>
   static void Main () {
      // Words with the same sorted letters are anagrams
      var groups = ReadWords (WORDFILE)
                   .GroupBy (w => new string ([.. w.Order ()]))
                   .Where (g => g.Count () > 1);
      // Bucket the groups by size, then print the largest size first
      foreach (var bucket in groups.ToLookup (g => g.Count ()).OrderByDescending (b => b.Key))
         foreach (var group in bucket)
            Console.WriteLine ($"{bucket.Key} {string.Join (" ", group)}");
   }
   #endregion

   #region Implementation -------------------------------------------
   // Reads all non-empty, trimmed words (one per line) of an embedded resource
   static string[] ReadWords (string file) {
      var asm = Assembly.GetExecutingAssembly ();
      string resource = asm.GetManifestResourceNames ().First (a => a.EndsWith (file));
      using Stream stream = asm.GetManifestResourceStream (resource)
                            ?? throw new InvalidOperationException ($"Resource not found");
      using StreamReader reader = new (stream);
      return [.. reader.ReadToEnd ().Split (Environment.NewLine)];
   }
   #endregion

   #region Constants ------------------------------------------------
   const string WORDFILE = "words.txt";      // Embedded Resource holding the word list
   #endregion
}
#endregion