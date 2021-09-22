namespace BuddyEllis
{
    using System;

    public class Program
    {
        public static void Main()
        {
            try
            {
                Console.WriteLine("Starting");
                var buddy = new BuddyEllis();
                buddy.Run();
                Console.WriteLine("Stopped");
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem. Report this message: {0}", ex);
            }
        }
    }
}