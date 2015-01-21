using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PkmnFoundations.Web
{
    public class DependencyNode<TKey, TValue>
    {
        public DependencyNode()
        {
            Dependencies = new HashSet<TKey>();
        }

        public DependencyNode(TKey key, TValue value)
            : this()
        {
            Key = key;
            Value = value;
        }

        public DependencyNode(TKey key, TValue value, HashSet<TKey> dependencies)
            : this(key, value)
        {
            Dependencies = dependencies;
        }

        public TKey Key { get; set; }
        public TValue Value { get; set; }
        public HashSet<TKey> Dependencies { get; private set; }
    }
}
