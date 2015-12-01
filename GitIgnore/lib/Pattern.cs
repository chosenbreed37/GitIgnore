namespace GitIgnore.lib
{
    using System.Text.RegularExpressions;

    public class Pattern
    {
        public Regex Expression { get; set; }
        public bool IsInclusive { get; set; }

        public bool Include(string path)
        {
            return IsInclusive && Expression.IsMatch(path);
        }

        public bool Exclude(string path)
        {
            return !IsInclusive && Expression.IsMatch(path);
        }


    }
}
