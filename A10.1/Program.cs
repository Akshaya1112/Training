// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Implements parsing of a string representation of a file path
// ------------------------------------------------------------------------------------------------
using System.Text;
using static EState;
using static System.Console;

# region Class Program ----------------------------------------------------------------------------
/// <summary>Tests the implementation of file path parser</summary>
class Progran {
   #region Implementation -------------------------------------------
   // Parses each test path and prints the drive, folder, file name and extension
   // A path passes if all four parts could be extracted
   static void Main () {
      WriteLine ($"{"INPUT",-32} {"DRIVE",-8} {"FOLDER",-20}" +
         $" {"FILE",-20} {"EXTENSION",-12} {"STATUS"}");
      WriteLine (new string ('-', 105));
      int passedCount = 0, failedCount = 0;
      foreach (var path in sTestPaths) {
         var (drive, folder, fileName, extension) = Parse (path);
         var passed = drive.Length > 0 && folder.Length > 0 &&
                      fileName.Length > 0 && extension.Length > 0;
         // Show a dash instead of the parts when parsing failed
         string driveStr = passed ? drive : "-";
         string folderStr = passed ? folder : "-";
         string fileStr = passed ? fileName : "-";
         string extensionStr = passed ? "." + extension : "-";
         Write ($"{path,-32} {driveStr,-8} {folderStr,-20} {fileStr,-20} {extensionStr,-12} ");
         ForegroundColor = passed ? ConsoleColor.Green : ConsoleColor.Red;
         WriteLine (passed ? "PASS" : "FAIL");
         ResetColor ();
         if (passed) passedCount++;
         else failedCount++;
      }
      WriteLine (new string ('-', 105));
      WriteLine ($"Total: {passedCount + failedCount} " +
                 $"| Passed: {passedCount} | Failed: {failedCount}");
   }

   //Splits a file path like C:\DIR\FILE.TXT into its parts using a state machine
   static (string, string, string, string) Parse (string input) {
      OutputEncoding = Encoding.UTF8;
      EState s = A;
      Action none = () => { }, todo;
      StringBuilder drive = new (), folder = new (), fileName = new (), extension = new ();
      // A '~' is added at the end so the state machine can detect the end of the path
      foreach (var ch in input.Trim () + '~') {
         (s, todo) = (s, ch) switch {
            // Drive letter, then ':' and '\'
            (A, >= 'A' and <= 'Z') => (B, () => drive.Append (ch)),
            (B, ':') => (C, none),
            (C, '\\') => (D, none),
            // Name characters; a '\' means the name so far was a folder, so add it to the folder
            (D or E, >= 'A' and <= 'Z') => (E, () => fileName.Append (ch)),
            (E, '\\') => (D, () => {
               if (folder.Length > 0) folder.Append ('→');
               folder.Append (fileName);
               fileName.Clear ();
            }
            ),
            // A '.' starts the extension, which must end at the end marker
            (E, '.') => (F, none),
            (F or G, >= 'A' and <= 'Z') => (G, () => extension.Append (ch)),
            (G, '~') => (H, none),
            // Anything else is not a valid path
            _ => (Z, none)
         };
         todo ();
      }
      return (s == H)
         ? (drive.ToString (), folder.ToString (), fileName.ToString (), extension.ToString ())
         : ("", "", "", "");
   }
   #endregion

   #region Private data ---------------------------------------------
   // Paths used to test the parser
   static readonly string[] sTestPaths = [
      @"Cz:\abc\def\r.txt", @"C:\abc\def\readme.txt", @"C:\README.TXT", @"C:\abc\.bcf",
      @"C:\abc\bcf.", @"Readme.txt", @"C:\abc\def", @"C:\abc d", @"\abcd\Readme.txt", " ",
      @"C:\ab.c\def\r.txt", @"C:\abc:d", @".\abc", ".abc", "abc", @"C:\abc6\def\r.txt",
      @"C:\DIR\ARCHIVE.TAR.GZ", @"C:\work\r.txt", @"C:\\work~\\r.txt~",
      @"C:\A\B\C\D\E\F\G\FILE.TXT", @"C:\DIR\\FILE.TXT",@"D:\WORK\TRAINING\FILE.TXT"];
   #endregion
}
#endregion

#region Enum EState -------------------------------------------------------------------------------
/// <summary>The states of the file path parser</summary>
enum EState {
   A,    // Start: expecting the drive letter
   B,    // Got the drive letter: expecting ':'
   C,    // Got ':': expecting '\'
   D,    // Got '\': expecting the first letter of a folder or file name
   E,    // Inside a folder or file name: expecting a letter, '\' or '.'
   F,    // Got '.': expecting the first letter of the extension
   G,    // Inside the extension: expecting a letter or the end of the path
   H,    // Done: the path is valid
   Z     // Error: the path is not valid
}
#endregion