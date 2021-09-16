namespace BuddyEllis
{
    using System;
    using System.Net.Http;
    using System.Timers;

    public class Program
    {
        public static void Main(string[] args)
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

        private class BuddyEllis
        {
            private const int InitialInterval = 1000;

            private const int Interval = 3600000 + 60000;

            private bool firstRun = true;

            public void Run()
            {
                using (var timer = new Timer(InitialInterval) { AutoReset = true })
                {
                    timer.Elapsed += async (sender, e) =>
                    {
                        if (firstRun && timer != null)
                        {
                            timer.Interval = Interval;
                            firstRun = false;
                        }

                        Console.WriteLine("attempting updoot");

                        var url =
                            "https://tools.gardenandgunmag.com/vote.php?id=1558&s=3&v1=kbkZbfjaor&v2=0a1ba1cbc54e642dc57ea5979b88b1e4&cv=y&_=1631821163291";
                        var client = new HttpClient();

                        HttpResponseMessage response = await client.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                            Console.WriteLine("success");
                        else
                            Console.WriteLine("failure");

                        Console.WriteLine("Quit? ");
                    };

                    timer.Start();
                    Console.WriteLine("Started");

                    while (true)
                    {
                        Console.WriteLine("Quit? ");
                        string? input = Console.ReadLine();
                        if (string.Equals(input, "y", StringComparison.InvariantCultureIgnoreCase))
                            break;
                    }

                    Console.WriteLine("Stopping");
                    timer.Stop();
                }
            }
        }
    }
}