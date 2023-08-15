using System.Diagnostics;
using System.Text.Json;

namespace SubscriptionCalculator
{
    internal class Program
    {
        private static List<Subscription> SubscriptionsList { get; set; }

        public static T LoadJsonFromFile<T>(string path)
        {
            if (!File.Exists(path))
                return (T)Activator.CreateInstance(typeof(T))!;

            string json = File.ReadAllText(path);
            T jsonObj = JsonSerializer.Deserialize<T>(json)!;

            return jsonObj;
        }

        public static void SaveJsonToFile<T>(T type, string path)
        {
            if (File.Exists(path))
                File.Delete(path);

            string json = JsonSerializer.Serialize(type);
            File.WriteAllText(path, json);
        }

        static void Main(string[] args)
        {
            SubscriptionsList = LoadJsonFromFile<List<Subscription>>("subscriptions.json");

            bool mainLoop = true;
            while (mainLoop)
            {
                Console.Clear();

                Console.WriteLine("--- Subscription Calculator ---");
                Console.WriteLine("(Press enter to exit)");
                Console.WriteLine();
                Console.WriteLine("1. Add a subscription");
                Console.WriteLine("2. Remove a subscription");
                Console.WriteLine("3. List subscriptions");
                Console.WriteLine("4. Calculate total");

                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.KeyChar)
                {
                    case '1':
                        AddSubscription();
                        break;

                    case '2':
                        RemoveSubscription();
                        break;

                    case '3':
                        ListSubscriptions();
                        break;

                    case '4':
                        CalculateTotal();
                        break;

                    case (char)13:
                    case '\0':
                    case ' ':
                        mainLoop = false;
                        break;
                }
            }

            SaveJsonToFile(SubscriptionsList, "subscriptions.json");
        }
        
        static void AddSubscription()
        {
            Console.Clear();
            Console.Write("What is the name of the subscription? ");
            string subscriptionName = Console.ReadLine()!;

            Subscription subscription = new()
            {
                SubscriptionName = subscriptionName,            };

            bool isMoreSubscriptionTypes = true;
            while (isMoreSubscriptionTypes)
            {
                Console.Clear();
                Console.WriteLine("What time period of subscription is it? (Or press enter to return to the menu)");
                Console.WriteLine();

                SubscriptionType currentSubscriptionType;
                SubscriptionData? currentSubscriptionData;
                SubscriptionType? tempSubType = Subscription.PromptSubscriptionType();
                if (tempSubType == null)
                {
                    if (subscription.SubscriptionDataList == null)
                        return;

                    isMoreSubscriptionTypes = false;
                    continue;
                }

                if (subscription.SubscriptionDataList == null)
                    subscription.SubscriptionDataList = new();

                currentSubscriptionType = tempSubType.Value;

                Console.Write("\nWhat is the price of the subscription? ");
                double price = double.Parse(Console.ReadLine()!);

                // set some stuff (current subscription type, current subscription data), and add the current subscription data to a list
                currentSubscriptionData = new(currentSubscriptionType, price);
                subscription.SubscriptionDataList.Add(currentSubscriptionData);

                // ask if owned, if y, set the owned flag to true and the ownedindex to the indexof currentsubscriptiondata
                Console.Write("Do you own this subscription type? (y/N) ");

                ConsoleKeyInfo doesOwn = Console.ReadKey(true);
                switch ($"{doesOwn.KeyChar}".ToLower())
                {
                    case "y":
                        subscription.IsOwned = true;
                        subscription.OwnedIndex = subscription.SubscriptionDataList.IndexOf(currentSubscriptionData);
                        break;
                }
            }

            SubscriptionsList.Add(subscription);
        }

        static void RemoveSubscription()
        {
            Console.Clear();
            Console.Write("What subscription would you like to remove? (or press enter to exit) ");
            string subscription = Console.ReadLine()!;

            if (subscription == null || subscription == "" || subscription == " " || SubscriptionsList.Find(pred => pred.SubscriptionName == subscription) == null)
                return;

            SubscriptionsList.Remove(SubscriptionsList.Find(pred => pred.SubscriptionName == subscription)!);
            Console.WriteLine($"Removed {subscription}!");
        }

        static void ListSubscriptions()
        {
            Console.Clear();

            Console.WriteLine("What subscriptions would you like to list?");
            Console.WriteLine();
            Console.WriteLine("1. Owned");
            Console.WriteLine("2. Not Owned");
            Console.WriteLine("3. All");

            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.KeyChar)
            {
                case '1':
                    Console.Clear();
                    Console.WriteLine("Owned Subscriptions: ");
                    Console.WriteLine();
                    foreach (Subscription sub in SubscriptionsList)
                    {
                        if (!sub.IsOwned)
                        {
                            continue;
                        }

                        SubscriptionData subData = sub.SubscriptionDataList[sub.OwnedIndex!.Value];
                        Console.WriteLine($"  {sub.SubscriptionName}, {subData.SubscriptionType.ToFriendlyString()}");
                    }
                    Console.WriteLine();
                    Console.WriteLine("Press any key to return to the menu...");
                    Console.ReadKey(true);

                    break;

                case '2':
                    
                    break;

                case '3':
                    
                    break;

                case (char)13:
                case '\0':
                case ' ':
                    break;
            }
        }

        static void CalculateTotal()
        {
            Console.Clear();
            Console.WriteLine("What billing period would you like to view your prices as?");
            Console.WriteLine();

            SubscriptionType currentSubscriptionType = default;
            SubscriptionType? tempSubType = Subscription.PromptSubscriptionType();
            if (tempSubType == null) // if they want to leave, return
                return;

            currentSubscriptionType = tempSubType.Value;

            Console.Clear();
            Console.WriteLine($"Subscriptions as {currentSubscriptionType.ToFriendlyString()}: ");
            Console.WriteLine();

            double total = 0;
            foreach (Subscription sub in SubscriptionsList)
            {
                if (!sub.IsOwned)
                {
                    continue;
                }

                SubscriptionData subData = sub.SubscriptionDataList[sub.OwnedIndex!.Value];
                double price = subData.ToPriceType(currentSubscriptionType);
                Console.WriteLine($"  {price:C} ({sub.SubscriptionName}, {subData.SubscriptionType.ToComparisonString(currentSubscriptionType)})");
                total += price;
            }

            Console.WriteLine("----------");
            Console.WriteLine($"Total: {total:C}");
            Console.WriteLine();
            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey(true);
        }
    }
}