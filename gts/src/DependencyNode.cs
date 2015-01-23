using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace PkmnFoundations.Web
{
    internal class DependencyNode<TKey, TValue>
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

    internal class DependencyGraph<TKey, TValue>
    {
        public DependencyGraph()
        {
            Graph = new List<DependencyNode<TKey, TValue>>();
        }

        public DependencyGraph(List<DependencyNode<TKey, TValue>> graph)
        {
            Graph = graph;
        }

        public List<DependencyNode<TKey, TValue>> Graph;
        public Page Page = null;

        public List<TValue> Resolve()
        {
            // fixme: we should clone the DependencyNodes so they don't get modified.
            LinkedList<DependencyNode<TKey, TValue>> nodesList = new LinkedList<DependencyNode<TKey, TValue>>(Graph);
            List<TValue> result = new List<TValue>(nodesList.Count);

            // Remove inexistent keys from dependency lists.
            // First, obtain a complete set of existent keys.
            HashSet<TKey> validKeys = new HashSet<TKey>();
            foreach (DependencyNode<TKey, TValue> node in nodesList)
                validKeys.Add(node.Key);

            // Then, intersect each dependency list with this validKeys list
            foreach (DependencyNode<TKey, TValue> node in nodesList)
                node.Dependencies.IntersectWith(validKeys);

            while (nodesList.Count > 0)
            {
                LinkedListNode<DependencyNode<TKey, TValue>> nodeLinked = nodesList.First;

                while (nodeLinked != null)
                {
                    if (nodeLinked.Value.Dependencies.Count == 0) break;
                    nodeLinked = nodeLinked.Next;
                }

                if (nodeLinked == null)
                {
                    // we hit the end of the list without finding a dependency-less node.
                    // this means there has to be a circular dependency.
                    // ie. every remaining node in the list depends on some other remaining
                    // node in the list.

                    // todo: display the actual cycle (or an example if there's more than one)
                    // in this exception.
                    // algo:
                    // Keep a collection of found nodes.
                    // Start at the first node, adding it to the collection.
                    // Move to the node of the first dependency of this node and add it to the collection.
                    // Repeat until you reach a node which is already in the collection.
                    // Output the collection of nodes, starting at the node which was already found.
                    throw new CircularDependencyException("Circular dependency found in your links.\n" +
                        "Keys:\n" +
                        String.Join("\n", nodesList.Select(n => n.Key.ToString()).ToArray()));
                }

                // add this node to output, remove it as a dependency from the remaining nodes
                DependencyNode<TKey, TValue> nodeOutput = nodeLinked.Value;
                nodesList.Remove(nodeLinked);

                foreach (DependencyNode<TKey, TValue> nodeValue in nodesList)
                    nodeValue.Dependencies.Remove(nodeOutput.Key);

                result.Add(nodeOutput.Value);
            }
            return result;
        }
    }

    public class CircularDependencyException : Exception
    {
        public CircularDependencyException(String message) : base(message)
        {

        }
    }
}
