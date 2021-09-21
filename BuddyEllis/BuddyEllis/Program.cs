namespace BuddyEllis
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Timers;

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

        private class BuddyEllis
        {
            private const int InitialInterval = 1 * 1000;

            private const int Interval = (60 * 60 * 1000) + (15 * 1000);

            private readonly bool usingInterval = false;

            public void Run()
            {
                using (var timer = new Timer(InitialInterval) { AutoReset = true })
                {
                    timer.Elapsed += async (sender, e) =>
                    {
                        Console.WriteLine("attempting updoot");

                        var url =
                            "https://tools.gardenandgunmag.com/vote.php?id=1558&s=3&v1=kbkZbfjaor&v2=0a1ba1cbc54e642dc57ea5979b88b1e4&cv=y&_=1631821163291";

                        bool success;
                        double currentInterval = timer?.Interval ?? 0;
                        double doubledInterval = currentInterval * 2;

                        using (var client = new HttpClient())
                        {
                            using (HttpResponseMessage response = await client.GetAsync(url))
                            {
                                if (!response.IsSuccessStatusCode)
                                {
                                    Console.WriteLine("failure to connect to server");
                                    Console.WriteLine("Quit?");
                                    return;
                                }

                                var updoot = await response.Content.ReadFromJsonAsync<UpdootResponse>();
                                success = updoot.Success;

                                if (updoot.Success)
                                {
                                    Console.WriteLine("updoot success");
                                }
                                else
                                {
                                    if (updoot.Error == "duplicate")
                                        Console.WriteLine(
                                            $"duplicate updoot, will try again in {doubledInterval / 1000} seconds");
                                    else
                                        Console.WriteLine(
                                            "updoot failed, try again but if it continues to fail let us know");
                                }

                                Console.WriteLine("Quit?");
                            }
                        }

                        if (timer == null)
                            return;

                        if (!usingInterval)
                        {
                            if (success || doubledInterval > Interval)
                                timer.Interval = Interval;
                            else
                                timer.Interval = doubledInterval;
                        }
                    };

                    timer.Start();
                    Console.WriteLine("Started");

                    while (true)
                    {
                        Console.WriteLine("Quit?");
                        string? input = Console.ReadLine();
                        if (string.Equals(input, "y", StringComparison.InvariantCultureIgnoreCase))
                            break;
                    }

                    Console.WriteLine("Stopping");
                    timer.Stop();
                }
            }

            private class UpdootResponse
            {
                public string Error { get; set; }

                public string Message { get; set; }

                public bool Success { get; set; }
            }
        }
    }
}