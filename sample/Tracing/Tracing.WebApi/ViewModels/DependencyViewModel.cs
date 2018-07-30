using System.Collections.Generic;

namespace Tracing.Server.ViewModels
{
    public class DependencyViewModel
    {
        public ICollection<NodeViewModel> Nodes { get; set; } = new List<NodeViewModel>();

        public ICollection<EdgeViewModel> Edges { get; set; } = new List<EdgeViewModel>();
    }

    public class NodeViewModel
    {
        public string Name { get; set; }
        
        public long Value { get; set; }
    }

    public class EdgeViewModel
    {
        public string Source { get; set; }

        public string Target { get; set; }

        public long Value { get; set; }
    }
}