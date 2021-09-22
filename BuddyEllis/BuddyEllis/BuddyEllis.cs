namespace BuddyEllis
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Timers;

    internal class BuddyEllis
    {
        private const int InitialInterval = 1 * 1000;

        private const int Interval = (60 * 60 * 1000) + (15 * 1000);

        private readonly string updootUrl =
            "https://tools.gardenandgunmag.com/vote.php?id=1558&s=3&v1=kbkZbfjaor&v2=0a1ba1cbc54e642dc57ea5979b88b1e4&cv=y&_=1631821163291";

        private bool usingInterval;

        public void Run()
        {
            usingInterval = false;
            using (var timer = new Timer(InitialInterval))
            {
                SetupTimer(timer);

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

        private void SetupTimer(Timer timer)
        {
            timer.Elapsed += async (sender, e) =>
            {
                Console.WriteLine("attempting updoot");

                bool success;
                double currentInterval = timer.Interval;
                double doubledInterval = currentInterval * 2;

                using (var client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync(updootUrl))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("failure to connect to server");
                            Console.WriteLine("Quit?");
                            return;
                        }

                        var updoot = await response.Content.ReadFromJsonAsync<UpdootResponse>();
                        success = updoot.Success;

                        if (!usingInterval)
                        {
                            if (success || doubledInterval > Interval)
                            {
                                timer.Interval = Interval;
                                usingInterval = true;
                            }
                            else
                            {
                                timer.Interval = doubledInterval;
                            }
                        }
                        else
                        {
                            if (!success)
                            {
                                usingInterval = false;
                                timer.Interval = InitialInterval;
                            }
                        }

                        if (updoot.Success)
                        {
                            Console.WriteLine($"updoot success, will updoot again in {timer.Interval / 1000} seconds");
                        }
                        else
                        {
                            if (string.Equals(updoot.Error, UpdootResponse.Duplicate,
                                StringComparison.InvariantCultureIgnoreCase))
                            {
                                Console.Write("duplicate updoot, ");
                                Console.WriteLine($"will try again in {timer.Interval / 1000} seconds");
                            }
                            else
                            {
                                Console.WriteLine(
                                    $"updoot failed, try again but if it continues to fail let us know, will try again in {timer.Interval / 1000} seconds");
                            }
                        }

                        Console.WriteLine("Quit?");
                    }
                }
            };
        }

        private class UpdootResponse
        {
            public static readonly string Duplicate = "duplicate";

            public string Error { get; set; }

            public string Message { get; set; }

            public bool Success { get; set; }
        }
    }
}