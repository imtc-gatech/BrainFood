using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class csNavManager : MonoBehaviour {

	private float GRID_SPACING = 96.0f;
	private float CONNECTION_RANGE = 150.0f;  // No diagonal links for now.
	private float GRID_BUFFER = 24.0f;
	private float MIN_X = -512.0f;
	private float MAX_X = 512.0f;
	private float MIN_Y = -384.0f;
	private float MAX_Y = 384.0f;

	private List<NavNode> nodes;

	void Awake () {
		// Create the navigation grid/map for this level.
		CreateNavMap ();
	}

	void Start() {
		DrawGrid();
	}

	void CreateNavMap () {
		nodes = new List<NavNode> ();
		MakeGrid ();
		ConnectNodes ();
	}

	void MakeGrid () {
		// Create points in a grid, excluding those within a short distance from any collider.
		for (float x = MIN_X; x <= MAX_X; x += GRID_SPACING) {
			for (float y = MIN_Y; y < MAX_Y; y += GRID_SPACING) {
				if (!Physics2D.OverlapCircle(new Vector2(x, y), GRID_BUFFER)) {
					// We can put a node here.
					nodes.Add(new NavNode(new Vector2(x,y)));
				}
			}
		}
	}

	void ConnectNodes () {
		// Connect together all nodes within a certain distance of each other.
		// TODO: check colliders in the way, if the corners turn out to be a problem.
		foreach (NavNode node in nodes) {
			// TODO: range check this preliminarily instead of being exhaustive.
			foreach (NavNode otherNode in nodes) {
				if (Vector2.Equals(node.loc, otherNode.loc)) continue;

				if (Vector2.Distance(node.loc, otherNode.loc) <= CONNECTION_RANGE) {
					node.AddLink(otherNode);
				}
			}
		}
	}
	
	void DrawGrid () {
		foreach (NavNode node in nodes) {
			Debug.DrawLine(node.loc - new Vector2(5.0f,5.0f), node.loc + new Vector2(5.0f,5.0f), Color.black, 10.0f);
			Debug.DrawLine(node.loc + new Vector2(-5.0f,5.0f), node.loc + new Vector2(5.0f,-5.0f), Color.black, 10.0f);
	 		foreach (NavNode otherNode in node.links.Keys) {
				Debug.DrawLine(node.loc, otherNode.loc, Color.blue, 10.0f);
			}
		}
	}

	public List<Vector2> GetPathToLocation (Vector2 from, Vector2 to) {
		// TODO: smart culling rather than full check.
		NavNode startNode = GetNearestNode (from);
		NavNode endNode = GetNearestNode (to);

		List<NavNode> mapPath = FindMapPath (startNode, endNode);
		List<Vector2> vecPath = new List<Vector2> ();

		foreach (NavNode node in mapPath) {
			vecPath.Add(node.loc);
		}

		if (vecPath.Count > 0) {
			vecPath.Insert(0, from);
			vecPath.Add (to);

			vecPath = OptimizePath(vecPath);
		}

		return vecPath;
	}

	List<NavNode> FindMapPath (NavNode from, NavNode to) {
		// Simple BFS for now.
		// TODO: could precompute dijkstra/etc from all nodes to known destinations (not all nodes) and have effectively
		// no run-time pathfinding necessary.  Hmm...
		Queue<PathNode> search = new Queue<PathNode> ();

		search.Enqueue (new PathNode (from));

		while (search.Count > 0) {
			PathNode pathNode = search.Dequeue();

			if (pathNode.node.Equals(to)) {
				pathNode.path.Add (pathNode.node);
				return pathNode.path;
			}

			foreach (NavNode neighbor in pathNode.node.links.Keys) {
				if (!pathNode.path.Contains(neighbor)) {
					search.Enqueue(new PathNode(neighbor, pathNode.path, pathNode.node));
				}
			}
		}

		return new List<NavNode> ();
	}

	NavNode GetNearestNode (Vector2 loc) {
		float mindist = 10000.0f;
		NavNode minnode = null;
		foreach (NavNode node in nodes) {
			float thisdist = Vector2.Distance(loc, node.loc);
			if (thisdist < mindist) {
				mindist = thisdist;
				minnode = node;
			}
		}
		return minnode;
	}

	List<Vector2> OptimizePath(List<Vector2> path) {
		List<Vector2> optimal = new List<Vector2> ();
		int i = 0;
		while (i < path.Count) {
			optimal.Add(path[i]);
			int farthestSeen = i+1;
			for (int j = i+1; j < path.Count; j++) {
				if (!Physics2D.Raycast(path[i], path[j] - path[i], Vector2.Distance(path[i],path[j]))) {
					// Can see.
					farthestSeen = j;
				}
			}
			i = farthestSeen;
		}

		return optimal;
	}

}
