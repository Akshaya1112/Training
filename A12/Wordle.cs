// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Wordle.cs
// Console implementation of the Wordle word guessing game
// ------------------------------------------------------------------------------------------------
using System.Text;
using static System.Console;

#region Class Wordle ------------------------------------------------------------------------------
/// <summary>Console implementation of the Wordle word guessing game</summary>
class Wordle {
   #region Properties -----------------------------------------------
   /// <summary>True once the word is guessed or all the guesses are used up</summary>
   public bool GameOver => mWon || mGuesses.Count == MAXGUESSES;

   // Column where the board starts so that it is centered in the console
   static int Left => Math.Max (0, (WindowWidth - BOARDWIDTH) / 2);
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Public interface routine to run the game</summary>
   public void Run () {
      ClearScreen ();
      SelectWord ();
      DisplayBoard ();
      while (!GameOver) {
         ConsoleKeyInfo key = ReadKey (true);
         UpdateGameState (key);
         DisplayBoard ();
      }
      PrintResult ();
   }

   // Runs the game
   static void Main () => new Wordle ().Run ();
   #endregion

   #region Implementation -------------------------------------------
   // Prepares the console with UTF8 output for the dot and circle and no blinking cursor
   static void ClearScreen () {
      OutputEncoding = Encoding.UTF8;
      CursorVisible = false;
      ResetColor ();
      Clear ();
   }

   // Loads the word lists and picks a random secret word
   void SelectWord () {
      mWords = new WordBank ();
      mSecretWord = mWords.PickSecretWord ();
   }

   // Applies a keypress with Enter to submit and Backspace or Left arrow to delete
   void UpdateGameState (ConsoleKeyInfo key) {
      mMessage = "";
      switch (key.Key) {
         case ConsoleKey.Enter:
            SubmitWord ();
            break;
         case ConsoleKey.Backspace or ConsoleKey.LeftArrow:
            if (mTypedWord.Length > 0) mTypedWord = mTypedWord[..^1];
            break;
         default:
            char c = char.ToUpper (key.KeyChar);
            if (c is >= 'A' and <= 'Z' && mTypedWord.Length < WORDLENGTH) mTypedWord += c;
            break;
      }
   }

   // Submits the typed word if it is complete and is a valid word
   void SubmitWord () {
      if (mTypedWord.Length < WORDLENGTH) return;
      if (mWords.IsValid (mTypedWord)) {
         mGuesses.Add (mTypedWord);
         mWon = mTypedWord == mSecretWord;
         mTypedWord = "";
      } else {
         mMessage = $"{mTypedWord} is not a word";
         return;
      }
   }

   // Returns the state of each letter of a guess and handles repeated letters correctly
   ELetterState[] GetLetterStates (string guess) {
      var states = new ELetterState[WORDLENGTH];
      var unmatched = new int[26];                        // Secret letters that are not green
      for (int i = 0; i < WORDLENGTH; i++) {
         if (guess[i] == mSecretWord[i]) states[i] = ELetterState.Correct;
         else unmatched[mSecretWord[i] - 'A']++;
      }
      for (int i = 0; i < WORDLENGTH; i++) {
         if (states[i] == ELetterState.Correct) continue;
         int k = guess[i] - 'A';
         if (unmatched[k] > 0) {
            states[i] = ELetterState.Present;
            unmatched[k]--;
         } else states[i] = ELetterState.Absent;
      }
      return states;
   }

   // Redraws the whole board by overprinting so nothing scrolls
   void DisplayBoard () {
      DrawGrid ();
      DrawLine (KEYBOARDTOP - 1);
      DrawKeyboard ();
      PrintMessage ();
   }

   // Draws the guesses with submitted words in color and the typed word in white
   void DrawGrid () {
      for (int row = 0; row < MAXGUESSES; row++) {
         bool submitted = row < mGuesses.Count;
         bool typing = row == mGuesses.Count && !GameOver;
         var states = submitted ? GetLetterStates (mGuesses[row]) : new ELetterState[WORDLENGTH];
         for (int col = 0; col < WORDLENGTH; col++) {
            char letter = EMPTYCELL;
            var color = ConsoleColor.DarkGray;
            if (submitted) (letter, color) = (mGuesses[row][col], GetColor (states[col]));
            else if (typing && col <= mTypedWord.Length) {
               letter = col < mTypedWord.Length ? mTypedWord[col] : NEXTCELL;
               color = ConsoleColor.White;
            }
            WriteAt (Left + GRIDLEFT + col * SPACING, 1 + row * SPACING, letter, color);
         }
      }
   }

   // Draws the keys from A to Z colored by the best state found so far
   void DrawKeyboard () {
      var best = new ELetterState[26];
      foreach (string guess in mGuesses) {
         var states = GetLetterStates (guess);
         for (int i = 0; i < WORDLENGTH; i++)
            if (states[i] > best[guess[i] - 'A']) best[guess[i] - 'A'] = states[i];
      }
      for (int i = 0; i < 26; i++) {
         int x = Left + 1 + i % KEYSPERROW * KEYSPACING;
         WriteAt (x, KEYBOARDTOP + i / KEYSPERROW, (char)('A' + i), GetColor (best[i]));
      }
   }

   // Prints the message below the keyboard which is not a word or the game result
   void PrintMessage () {
      for (int y = MESSAGETOP; y < BOARDHEIGHT; y++)
         WriteAt (0, y, new string (' ', WindowWidth - 1));
      if (GameOver) {
         DrawLine (MESSAGETOP);
         string unit = mGuesses.Count == 1 ? "try" : "tries";
         string result = mWon ? $"You found the word in {mGuesses.Count} {unit}"
                              : $"Sorry - the word was {mSecretWord}";
         var color = mWon ? ConsoleColor.Green : ConsoleColor.Yellow;
         WriteCentered (MESSAGETOP + 1, result, color);
         WriteCentered (MESSAGETOP + 3, "Press any key to quit");
      } else if (mMessage != "") WriteCentered (MESSAGETOP + 1, mMessage, ConsoleColor.Yellow);
   }

   // Waits for a key as the result is already on the screen then moves below the board
   static void PrintResult () {
      ReadKey (true);
      ResetColor ();
      SetCursorPosition (0, BOARDHEIGHT);
      CursorVisible = true;
   }

   // Returns the display color for a letter state
   static ConsoleColor GetColor (ELetterState state) => state switch {
      ELetterState.Correct => ConsoleColor.Green,
      ELetterState.Present => ConsoleColor.Blue,
      ELetterState.Absent => ConsoleColor.DarkGray,
      _ => ConsoleColor.White
   };

   // Draws a separator line across the board
   static void DrawLine (int y)
       => WriteAt (Left, y, new string (LINECHAR, BOARDWIDTH), ConsoleColor.DarkGray);

   // Writes text at the given column and row
   static void WriteAt (int x, int y, object text, ConsoleColor color = ConsoleColor.White) {
      SetCursorPosition (x, y);
      ForegroundColor = color;
      Write (text);
   }

   // Writes text centered in the console
   static void WriteCentered (int y, string text, ConsoleColor color = ConsoleColor.White)
       => WriteAt (Math.Max (0, (WindowWidth - text.Length) / 2), y, text, color);
   #endregion

   #region Constants ------------------------------------------------
   const int WORDLENGTH = 5;                     // Letters in the secret word
   const int MAXGUESSES = 6;                     // Guesses allowed
   const int BOARDWIDTH = 21;                    // Width of the board
   const int GRIDLEFT = 6;                       // Column of the first letter on the board
   const int SPACING = 2;                        // Spacing of letters and guesses
   const int KEYBOARDTOP = 13;                   // Screen line of the first keyboard row
   const int KEYSPERROW = 7;                     // Keys in each row of the keyboard
   const int KEYSPACING = 3;                     // Screen columns between two keys
   const int MESSAGETOP = 17;                    // Screen line where the messages start
   const int BOARDHEIGHT = 22;                   // Screen lines used by the whole board
   const char EMPTYCELL = '\u00b7';              // Cell with no letter
   const char NEXTCELL = '\u25cc';               // Cell where the next letter goes (dotted circle)
   const char LINECHAR = '\u2500';               // Separator line
   #endregion

   #region Private data ---------------------------------------------
   WordBank mWords = null!;                      // Puzzle words and dictionary
   readonly List<string> mGuesses = [];          // Words submitted so far
   string mSecretWord = "";                      // The word to guess
   string mTypedWord = "";                       // Word being typed
   string mMessage = "";                         // Message such as not a word
   bool mWon;                                    // True if the secret word was guessed
   #endregion
}
#endregion

#region Enum ELetterState -------------------------------------------------------------------------
/// <summary>State of a letter in a guessed word or on the keyboard on screen</summary>
enum ELetterState {
   Unused,    // Letter not tried yet
   Absent,    // Letter is not in the secret word
   Present,   // Letter is in the secret word but at another position
   Correct    // Letter is in the secret word at this position
}
#endregion