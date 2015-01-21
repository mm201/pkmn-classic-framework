using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace PkmnFoundations.Web
{
    public abstract class RequireLinkBase : System.Web.UI.Control
    {
        protected RequireLinkBase()
            : base()
        {
            this.Init += RequireLinkBase_Init;
            this.Load += RequireLinkBase_Load;
            this.PreRender += RequireLinkBase_PreRender;
        }

        void RequireLinkBase_Init(object sender, EventArgs e)
        {
            // todo: only bind once
            Page.PreRender += Page_PreRender;
        }

        void RequireLinkBase_Load(object sender, EventArgs e)
        {
        }

        void RequireLinkBase_PreRender(object sender, EventArgs e)
        {
            DependencyGraph<String, RequireLinkBase> graph = GetDependencyGraph();

            DependencyNode<String, RequireLinkBase> node = new DependencyNode<String, RequireLinkBase>(Key, this, ParseDependencies(After ?? ""));
            if (!graph.Graph.Any(n => n.Key == Key))
                graph.Graph.Add(node);
        }

        void Page_PreRender(object sender, EventArgs e)
        {
            DependencyGraph<String, RequireLinkBase> graph = GetDependencyGraph();
            if (graph.Page == null)
            {
                Page.Header.Controls.Add(new RequireLinkRenderer(GetDependencyGraph()));
                graph.Page = Page;
            }
        }

        public String Key { get; set; }
        public String After { get; set; }

        public abstract void RenderHeader(System.Web.UI.HtmlTextWriter writer);

        /// <summary>
        /// Obtain a dependency graph which is specific to the Page instance
        /// and the control class
        /// </summary>
        private DependencyGraph<String, RequireLinkBase> GetDependencyGraph()
        {
            Type myType = this.GetType();

            Dictionary<Type, DependencyGraph<String, RequireLinkBase>> all_graphs;

            if (!Page.Items.Contains("pkmncfDependencyGraphs"))
            {
                all_graphs = new Dictionary<Type, DependencyGraph<String, RequireLinkBase>>();
                Page.Items.Add("pkmncfDependencyGraphs", all_graphs);
            }
            else all_graphs =
                (Dictionary<Type, DependencyGraph<String, RequireLinkBase>>)Page.Items["pkmncfDependencyGraphs"];

            if (all_graphs.ContainsKey(myType)) return all_graphs[myType];

            DependencyGraph<String, RequireLinkBase> myGraph = new DependencyGraph<String, RequireLinkBase>();
            all_graphs.Add(myType, myGraph);
            return myGraph;
        }

        private HashSet<String> ParseDependencies(String dependencies)
        {
            return new HashSet<String>(dependencies.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0));
        }

    }

    internal class RequireLinkRenderer : System.Web.UI.Control
    {
        public RequireLinkRenderer(DependencyGraph<String, RequireLinkBase> graph)
            : base()
        {
            m_graph = graph;
        }

        private DependencyGraph<String, RequireLinkBase> m_graph;

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            List<RequireLinkBase> links = m_graph.Resolve();

            foreach (RequireLinkBase link in links)
                link.RenderHeader(writer);
        }
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
                    throw new Exception("Circular dependency found in your links.\n" +
                        "Keys:\n" +
                        String.Join("\n", nodesList.Select(n => n.Key).ToArray()));
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
}
