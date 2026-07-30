namespace A02;

class Program
{
    static void Main(string[] args)
    {
      var rd = new Random ();
      int number = rd.Next (0, 101);
      int guess;
      int attempts = 0;
      int low = 1;
      int high = 100;
      while (true) {
         Console.Write("Enter your guess:");
         while (!int.TryParse (Console.ReadLine (), out guess) && guess >= 1 && guess <= 100)
            Console.WriteLine ("Enter number between 1 to 100");
         attempts++;
         if (guess > number) {
            //Console.WriteLine(" Guess Lower");
            high = guess - 1;
            Console.WriteLine ($"Your guess is high. Try guessing between {low} to {high}");
         } else if (guess < number) {
            //Console.WriteLine("Guess Higher");
            low = guess + 1;
            Console.WriteLine ($"Your guess is low. Try guessing between {low} to {high}");
         } else {
            Console.WriteLine ($"You guessed correctly. Attempts:{attempts}");
            break;
         }
      }
   }
}
