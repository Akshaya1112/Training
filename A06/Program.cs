// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Solves the Eight Queens problem using backtracking.
// ------------------------------------------------------------------------------------------------
using System.Text;
using static System.Console;
using ConsoleKey = System.ConsoleKey;

class Program {
   static void Main () {
      WriteLine ("Eight Queens Problem");
      WriteLine ("(A)ll Solutions");
      WriteLine ("(U)nique Solutions");
      Write ("Select an option: ");
      ConsoleKey key;
      while (true) { key = ReadKey (true).Key; if (key is ConsoleKey.A or ConsoleKey.U) break; }
      Clear ();
      switch (key) {
         case ConsoleKey.A:
            PrintSolutions (FindSolutions (false), "All Solutions");
            break;
         case ConsoleKey.U:
            PrintSolutions (FindSolutions (true), "Unique Solutions");
            break;
      }

      // Finds all valid queen placements using backtracking.
      List<int[]> FindSolutions (bool findUnique) {
         List<int[]> solutions = [];
         int[] pos = new int[N];
         Place (0);
         return solutions;

         // Places a queen in each row and continues with the next row.
         void Place (int row) {
            for (pos[row] = 0; pos[row] < N; pos[row]++) {
               if (Valid (row)) {
                  if (row < N - 1) Place (row + 1);
                  else {
                     if (!findUnique) { solutions.Add ([.. pos]); } else if (IsUnique (pos)) {
                        solutions.Add ([.. pos]);
                     }
                  }
               }
            }
         }

         // Checks whether the queen placement in the specified row is valid.
         bool Valid (int row) {
            for (int r = 0; r < row; r++) {
               int dy = row - r, dx = Math.Abs (pos[row] - pos[r]);
               if (dx == 0 || dx == dy) return false;
            }
            return true;
         }

         // Checks whether the solution is different from its rotations and mirror images.
         bool IsUnique (int[] solution) {
            int[] temp = solution;
            for (int i = 0; i < 4; i++) {
               if (Exists (temp) || Exists (Mirror (temp))) return false;
               temp = Rotate (temp);
            }
            return true;
         }

         // Checks whether a solution already exists in the list.
         bool Exists (int[] test) => solutions.Any (x => x.SequenceEqual (test));
      }

      // Rotates a queen placement by 90 degrees.
      int[] Rotate (int[] a) {
         int[] b = new int[N];
         for (int i = 0; i < N; i++) b[a[i]] = N - i - 1;
         return b;
      }

      // Creates the mirror image of a queen placement.
      int[] Mirror (int[] a) => [.. a.Reverse ()];

      // Displays the solutions and allows navigation between them.
      void PrintSolutions (List<int[]> solutions, string title) {
         OutputEncoding = Encoding.UTF8;
         int soln = 0;
         while (soln >= 0 && soln < solutions.Count) {
            SetCursorPosition (0, 0);
            WriteLine ($"{title}");
            WriteLine ($"Solution {soln + 1} of {solutions.Count}\n");
            PrintBoard (solutions[soln]);
            WriteLine ("\n← Previous    → Next    Esc Exit");
            ConsoleKey key;
            while (true) {
               key = ReadKey (true).Key;
               if (key is ConsoleKey.RightArrow or ConsoleKey.LeftArrow or ConsoleKey.Escape) break;
            }
            switch (key) {
               case ConsoleKey.RightArrow:
                  soln++;
                  break;
               case ConsoleKey.LeftArrow:
                  soln--;
                  break;
               case ConsoleKey.Escape:
                  Clear ();
                  return;
            }
         }
      }

      // Displays a queen placement as a chess board.
      void PrintBoard (int[] queens) {
         WriteLine (Border (TOP));
         for (int row = 0; row < N; row++) {
            Write (VERTICAL);
            for (int column = 0; column < N; column++)
               Write ((queens[row] == column ? QUEEN : EMPTY) + VERTICAL);
            WriteLine ();
            if (row < N - 1) WriteLine (Border (MID));
         }
         WriteLine (Border (BOTTOM));
      }

      // Creates a board border using the specified pattern.
      string Border (string pattern)
         => pattern[0] + string.Join (pattern[1], Enumerable.Repeat (HORIZONTAL, N)) + pattern[2];
   }

   const string TOP = "┌┬┐";
   const string MID = "├┼┤";
   const string BOTTOM = "└┴┘";
   const string VERTICAL = "│";
   const string HORIZONTAL = "────";
   const string EMPTY = "    ";
   const string QUEEN = " ♕  ";
   const int N = 8;
}