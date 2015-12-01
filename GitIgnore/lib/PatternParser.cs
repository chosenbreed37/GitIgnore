namespace GitIgnore.lib
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public class PatternParser
    {
        public Pattern Parse(string pattern)
        {
            Pattern result = new Pattern();

            // An optional prefix "!" which negates the pattern; any matching file
            // excluded by a previous pattern will become included again.
            if (pattern.StartsWith("!"))
            {
                pattern = pattern.Substring(1);
                result.IsInclusive = true;
            }
            else
            {
                result.IsInclusive = false;
            }

            // Remove leading back-slash escape for escaped hash ('#') or
            // exclamation mark ('!').
            if (pattern.StartsWith("\\")) pattern = pattern.Substring(1);

            // Split pattern into segments.
            List<string> patternSegments = pattern.Split('/').ToList();

            // A pattern beginning with a slash ('/') will only match paths
            // directly on the root directory instead of any descendant paths.
            // So remove empty first segment to make pattern absoluut to root.
            // A pattern without a beginning slash ('/') will match any
            // descendant path. This is equivilent to "**/{pattern}". So
            // prepend with double-asterisks to make pattern relative to
            // root.
            if (patternSegments[0] == "")
            {
                patternSegments = patternSegments.Skip(1).ToList();
            }
            else if (patternSegments[0] != "**")
            {
                patternSegments.Insert(0, "**");
            }

            // A pattern ending with a slash ('/') will match all descendant
            // paths of if it is a directory but not if it is a regular file.
            // This is equivilent to "{pattern}/**". So, set last segment to
            // double asterisks to include all descendants.
            if(patternSegments[patternSegments.Count - 1] == "")
            {
                patternSegments[patternSegments.Count - 1] = "**";
            }

            // Build regular expression from pattern
            StringBuilder expressionBuilder = new StringBuilder();
            bool needSlash = false;

            for (int i = 0; i < patternSegments.Count; i++)
            {
                var segment = patternSegments[i];

                switch(segment)
                {
                    case "**": 
                        if(i == 0 && i == (patternSegments.Count - 1))
                        {
                            // A pattern consisting solely of double-asterisks ('**')
                            // will match every path.
                            expressionBuilder.Append(".+");
                        }
                        else if (i == 0)
                        {
                            // A normalized pattern beginning with double-asterisks
                            // ('**') will match any leading path segments.
                            expressionBuilder.Append("(?:.+/)?");
                            needSlash = false;
                        }
                        else if(i == (patternSegments.Count - 1))
                        {
                            // A normalized pattern ending with double-asterisks ('**')
                            // will match any trailing path segments.
                            expressionBuilder.Append("/.+");
                        }
                        else
                        {
                            // A pattern with inner double-asterisks ('**') will match
                            // multiple (or zero) inner path segments.
                            expressionBuilder.Append("(?:/.+)?");
                            needSlash = true;
                        }
                        continue;

                    case "*":
                        // Match single path segment.
                        if (needSlash)
                        {
                            expressionBuilder.Append("/");
                        }
                        expressionBuilder.Append("[^/]+");
                        needSlash = true;
                        continue;

                    default: 
                        if(needSlash)
                        {
                            expressionBuilder.Append("/");
                        }
                        expressionBuilder.Append(TranslateGlob(segment));
                        
                        // TODO : make it cleaner
                        if ((i + 1) < patternSegments.Count && !new[] { "**", "*", "" }.Contains(patternSegments[i + 1]))
                        {
                            expressionBuilder.Append("/");
                        }
                        continue;

                }
            }

            result.Expression = new Regex(expressionBuilder.ToString());
            return result;
        }
        
        private string TranslateGlob(string glob)
        {
            return Regex.Escape(glob).Replace(@"\*", ".*").Replace(@"\?", ".");
        }
    }
}
