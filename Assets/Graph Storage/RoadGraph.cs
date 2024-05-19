using System.Collections.Generic;

//used for storing the road system.
public class RoadGraph 
{
    public List<Node> nodes;
    public List<Edge> edges;

    public RoadGraph() { 
        nodes = new List<Node>();
        edges = new List<Edge>();
    }
}
