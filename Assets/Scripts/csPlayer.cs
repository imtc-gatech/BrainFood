using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class csPlayer : MonoBehaviour {

	public LayerMask exclusionLayer;
	public Image hustleCooldownButton;

	private float MOVE_SPEED = 8.0f;
	private float HUSTLE_TIME = 5.0f;
	private float HUSTLE_COOLDOWN = 30.0f;

	private csFood hand = null;
	private Transform transHand;
	private SpriteRenderer cantDo;

	private List<csLocation> actionQueue;
	private List<Vector2> currentPath;
	private csNavManager nav;
	
	private bool newGoal = false;

	private bool isHustling = false;

	private ParticleSystem trail;
	private float hustleCooldownTime = 0.0f;


	void Awake () {
		nav = Camera.main.GetComponent<csNavManager> ();
		actionQueue = new List<csLocation> ();
		currentPath = new List<Vector2> ();
		transHand = transform.FindChild ("Hand");
		trail = transform.FindChild ("HustlingEffect").GetComponent<ParticleSystem> ();
		cantDo = transform.FindChild ("CantDo").GetComponent<SpriteRenderer> ();
		cantDo.enabled = false;
	}

	void Update () {
		// If there is a location in the action queue...
		if (actionQueue.Count > 0) {
			// And the player has reached it, take action and pop the queue.
			if (Vector2.Distance(transform.position, actionQueue[0].getPathPos()) < 1.0f) {
				csLocation loc = actionQueue[0];

				bool actionSuccess = false;
				if (ContainsFood() && loc.WillModifyFood(hand)) {
					// Try to apply this location's modification to our food.
					loc.ModifyFood(hand);
					actionSuccess = true;
				} else if (ContainsFood() && loc.ContainsFood()) {
					// Try to exchange food.
					if (loc.WillGiveFood() && loc.WillTakeFood(hand)) {
						// Both pieces of the exchange will work.
						csFood temp = loc.GiveFood();
						loc.TakeFood(GiveFood());
						TakeFood(temp);
						actionSuccess = true;
					}
				} else if (ContainsFood()) {
					// Try to give food only.
					if (WillGiveFood() && loc.WillTakeFood(hand)) {
						loc.TakeFood(GiveFood());
						actionSuccess = true;
					}
				} else if (loc.ContainsFood()) {
					// Try to take food only.
					if (loc.WillGiveFood() && WillTakeFood()) {
						TakeFood(loc.GiveFood());
						actionSuccess = true;
					}
				}
				if (!actionSuccess) {
					CantDo();
				}
				PopQueue();
			} else if (Vector2.Distance(transform.position, currentPath[0]) < 1.0f) {
				// If the player has reached a waypoint (but not the current goal), switch
				// to the next waypoint.
				PopPath();
			} else {
				// Otherwise, move towards the current waypoint at MOVE_SPEED.
				if (isHustling) {
					transform.position = Vector2.MoveTowards(transform.position, currentPath[0], MOVE_SPEED * 3.0f);
				} else {
					transform.position = Vector2.MoveTowards(transform.position, currentPath[0], MOVE_SPEED);
				}
			}
		}

		if (hustleCooldownTime > 0.0f) {
			hustleCooldownTime -= Time.deltaTime;
			if (hustleCooldownTime <= 0.0f) {
				hustleCooldownTime = 0.0f;
			}
			hustleCooldownButton.fillAmount = Mathf.Clamp01(hustleCooldownTime / HUSTLE_COOLDOWN);
		}
	}

	/***
	 *   Food management functions, very similar to what csLocation does for all locations.
	 ***/

	bool ContainsFood () {
		return (hand != null);
	}

	bool WillTakeFood () {
		// This will have to become a little more complex when the player is doing
		// more than just placing and taking food (e.g. combining).
		return !ContainsFood ();
	}

	void TakeFood (csFood food) {
		// Guaranteed receipt IF WillTakeFood() returned true this frame.
		hand = food;
		food.transform.position = transHand.position;
		food.transform.parent = transHand;
		food.SortForPlayer ();
	}

	bool WillGiveFood () {
		return ContainsFood ();
	}

	csFood GiveFood () {
		csFood foodToReturn = hand;
		hand = null;
		foodToReturn.SortForRoom ();
		return foodToReturn;
	}


	/***
	 *   Power-up or special ability management functions.
	 ***/

	IEnumerator DoHustle () {
		hustleCooldownTime = HUSTLE_COOLDOWN;
		isHustling = true;
		trail.Play ();
		yield return new WaitForSeconds (HUSTLE_TIME);
		trail.Stop ();
		isHustling = false;
	}

	/***
	 *   Give error feedback to the player when an invalid action is attempted.
	 ***/

	IEnumerator ShowCantDo (float showForSeconds) {
		cantDo.enabled = true;
		yield return new WaitForSeconds (showForSeconds);
		cantDo.enabled = false;
	}

	void CantDo () {
		StartCoroutine (ShowCantDo (0.5f));
	}
	

	/***
	 *   Actor path maintenance functions (not the same as action queue or goal, but rather planned path to reach goal).
	 *   Should be extracted to a generic actor class (or a generic pathfinding class) if retained.
	 ***/

	void PlanPath () {
		currentPath.Clear ();
		
		if (actionQueue.Count <= 0) {
			return;
		}
		
		Vector2 goal = actionQueue[0].getPathPos ();
		currentPath = nav.GetPathToLocation (transform.position, goal);
	}

	void PopPath () {
		if (currentPath.Count <= 0) {
			return;
		}
		
		currentPath.RemoveAt (0);
	}


	/***
	 *   Actor goal management queue.  Should be placed in a more generic actor base class if there will ever
	 *   be actors beyond just the player who need it.
	 ***/

	public void AddLocToQueue (csLocation loc) {
		if (!actionQueue.Contains (loc)) {
			actionQueue.Add(loc);
			RenumberQueue();
		}
	}
	
	void PopQueue () {
		if (actionQueue.Count <= 0) {
			return;
		}
		
		actionQueue [0].SetQueuePosText ("");
		actionQueue.RemoveAt (0);
		
		RenumberQueue ();
	}
	
	void RenumberQueue () {
		for (int i = 0; i < actionQueue.Count; i++) {
			actionQueue[i].SetQueuePosText((i+1).ToString());
		}
		PlanPath ();
	}
	
	void ClearQueue () {
		foreach (csLocation loc in actionQueue) {
			loc.SetQueuePosText ("");
		}
		actionQueue.Clear ();
		RenumberQueue ();
	}


	/***
	 *   UI call in functions.  (e.g. hustle, attention)
	 ***/

	public void HustleClick () {
		if (hustleCooldownTime <= 0.0f) {
			StartCoroutine (DoHustle ());
		}
	}

	public void AttentionClick () {
		ClearQueue ();
	}

}
