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
            // todo: merge dependencies if it's a dupe
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

        public virtual String Key { get; set; }
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
}
