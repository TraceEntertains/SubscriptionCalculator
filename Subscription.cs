namespace SubscriptionCalculator
{
    public class Subscription
    {
        public string SubscriptionName { get; set; }
        public bool IsOwned { get; set; }
        public int? OwnedIndex { get; set; }
        public List<SubscriptionData> SubscriptionDataList { get; set; } // Subscription type, price per billing period

        public static SubscriptionType? PromptSubscriptionType()
        {
            // get a list of every subscription type
            SubscriptionType[] subscriptionTypes = (SubscriptionType[])Enum.GetValues(typeof(SubscriptionType));

            // list all subscription types
            int i = 1;
            foreach (SubscriptionType subscriptionType in subscriptionTypes)
            {
                Console.WriteLine($"{i}. {subscriptionType.ToFriendlyString()}");
                i++;
            }

            // read a key for finding the subscription type
            ConsoleKeyInfo subType = Console.ReadKey(true);

            i = 1;
            // for every subscription type
            foreach (SubscriptionType subscriptionType in subscriptionTypes)
            {
                // if i is the same as the pressed character (because of how i was used earlier)
                if (subType.KeyChar == i.ToString().ToCharArray().Single())
                {
                    // return the subscription type
                    return subscriptionType;
                }
                i++;
            }

            return null;
        }
    }

    public class SubscriptionData
    {
        public SubscriptionType SubscriptionType { get; set; }
        public double Price { get; set; }

        public SubscriptionData(SubscriptionType subscriptionType, double price)
        {
            SubscriptionType = subscriptionType;
            Price = price;
        }
    }

    public enum SubscriptionType
    {
        Monthly, // 1 Month
        TriMonthly, // 3 Month
        Yearly // 12 Month
    }

    public static class SubscriptionTypeExtensions
    {
        public static string ToFriendlyString(this SubscriptionType type)
        {
            return type switch
            {
                SubscriptionType.Monthly => "Monthly",
                SubscriptionType.TriMonthly => "Tri-Monthly",
                SubscriptionType.Yearly => "Yearly",
                _ => "Monthly",
            };
        }

        public static SubscriptionType ToSubscriptionType(this string type)
        {
            return type switch
            {
                "Monthly" => SubscriptionType.Monthly,
                "Tri-Monthly" => SubscriptionType.TriMonthly,
                "Yearly" => SubscriptionType.Yearly,
                _ => SubscriptionType.Monthly,
            };
        }

        public static double ToPriceType(this SubscriptionData subData, SubscriptionType targetType)
        {
            double priceAsMonthly = 0;

            // get any subscription type down to a monthly
            priceAsMonthly = subData.SubscriptionType switch
            {
                SubscriptionType.Monthly => subData.Price,
                SubscriptionType.TriMonthly => subData.Price / 3,
                SubscriptionType.Yearly => subData.Price / 12,
                _ => subData.Price,
            };

            // bring it to the target type
            return targetType switch
            {
                SubscriptionType.Monthly => priceAsMonthly,
                SubscriptionType.TriMonthly => (priceAsMonthly * 3),
                SubscriptionType.Yearly => (priceAsMonthly * 12),
                _ => priceAsMonthly,
            };
        }

        public static string ToComparisonString(this SubscriptionType type, SubscriptionType target)
        {
            if (type == target) return type.ToFriendlyString();

            return $"{type.ToFriendlyString()} -> {target.ToFriendlyString()}";
        }
    }
}