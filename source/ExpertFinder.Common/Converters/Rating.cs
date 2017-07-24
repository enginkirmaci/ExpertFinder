namespace ExpertFinder.Common.Converters
{
    static public class Rating
    {
        static public string ToString(double? rating)
        {
            if (rating.HasValue)
            {
                if (rating.Value > 4.5)
                    return "five";
                else if (rating.Value > 3.5)
                    return "four";
                else if (rating.Value > 2.5)
                    return "three";
                else if (rating.Value > 1.5)
                    return "two";
                else if (rating.Value > 0.5)
                    return "one";
            }

            return "no";
        }
    }
}