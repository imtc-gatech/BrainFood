using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavNode {
	// Data class for custom navigation nodes.
	public Vector2 loc;
	public Dictionary<NavNode,float> links;

	public NavNode() {
		loc = Vector2.zero;
		links = new Dictionary<NavNode,float> ();
	}

	public NavNode (Vector2 _loc) {
		loc = _loc;
		links = new Dictionary<NavNode,float> ();
	}

	public bool AddLink (NavNode otherNode) {
		//if (!Physics2D.Raycast (loc, otherNode.loc - loc, 2000.0f)) {
			// Clear line of sight from all colliders.  Add it.

			links.Add(otherNode, Vector2.Distance(loc, otherNode.loc));
			return (true);

		//}

		//return (false);
	}
	
}
