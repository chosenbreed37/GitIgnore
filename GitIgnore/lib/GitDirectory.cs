using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GitIgnore.lib
{
    public class GitDirectory
    {
        private readonly IEnumerable<Pattern> _rules;

        private readonly string _path;

        public GitDirectory(string path, IEnumerable<string> gitIgnoreRules)
        {
            _path = path;
            PatternParser parser = new PatternParser();
            _rules = gitIgnoreRules.Select(rule => parser.Parse(rule));
        }

        public IEnumerable<string> GetFilesWithoutIgnored()
        {
            return Directory.GetFiles(_path).Where(x => _rules.All(r => !r.Exclude(x)));
        }
    }
}
