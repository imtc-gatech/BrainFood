using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathNode {
	public NavNode node;
	public List<NavNode> path;

	public PathNode (NavNode _node) {
		node = _node;
		path = new List<NavNode> ();
	}

	public PathNode (NavNode _node, List<NavNode> _path) {
		node = _node;
		path = _path;
	}

	public PathNode (NavNode _node, List<NavNode> _path, NavNode _extend) {
		node = _node;
		path = new List<NavNode> (_path);
		path.Add (_extend);
	}
}
