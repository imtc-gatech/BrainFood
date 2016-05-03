using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class csLocation : MonoBehaviour {

	// Parent class for csLoc stations.
	
	public enum PrepState {
		None,
		Prepping,
		Ruining,
		Ruined
	};

	public float prepTime = 0.0f;
	public float ruinTime = 0.0f;
	
	protected csFood food = null;

	private TextMesh queuePosText;
	private Text qpt;
	private Vector3 pathPos;
	private csPlayer player;

	protected Slider timerViz;
	protected Image timerVizColor;
	protected float timeRemaining;
	protected PrepState prepState;

	public virtual void Awake () {
		player = GameObject.Find ("Player").GetComponent<csPlayer> ();
		pathPos = transform.FindChild ("PathPos").position;
		if (transform.FindChild ("QueuePos") != null) {
			queuePosText = transform.FindChild ("QueuePos").GetComponent<TextMesh> ();
		} else {
			qpt = transform.FindChild ("Canvas").FindChild ("QueuePos").GetComponent<Text> ();
		}
		SetQueuePosText ("");
		prepState = PrepState.None;
		timerViz = transform.FindChild ("Canvas").FindChild ("LocationTimer").GetComponent<Slider> ();
		timerVizColor = timerViz.transform.FindChild ("Fill Area").FindChild ("Fill").GetComponent<Image> ();
		timerViz.gameObject.SetActive (false);
	}

	void Update () {
		if (prepState == PrepState.Prepping) {
			timeRemaining = Mathf.Max (timeRemaining - Time.deltaTime, 0.0f);
			timerViz.value = timeRemaining / prepTime;
			if (timeRemaining <= 0.0f) {
				// Food's done.
				PrepComplete ();
			}
		} else if (prepState == PrepState.Ruining) {
			timeRemaining = Mathf.Max (timeRemaining - Time.deltaTime, 0.0f);
			timerViz.value = 1.0f - (timeRemaining / ruinTime);
			if (timeRemaining <= 0.0f) {
				// Food is ruined.
				RuinFood();
			}
		}
	}

	void OnMouseDown () {
		// Offer this location to the player's movement queue.
		Debug.Log (player);
		Debug.Log (this);
		player.AddLocToQueue (this);
		Debug.Log ("Player clicked " + transform.position);
	}

	public void SetQueuePosText (string pos) {
		if (queuePosText != null) {
			queuePosText.text = pos;
		} else {
			qpt.text = pos;
		}
	}

	public Vector3 getPathPos() {
		return (pathPos);
	}

	public virtual bool ContainsFood () {
		return (food != null);
	}

	// Will this location give up its current food right now?
	public virtual bool WillGiveFood () {
		return false;
	}

	public virtual csFood GiveFood () {
		return null;
	}

	// WOULD this location take the offered food if it were empty?  (Also check ContainsFood().)
	public virtual bool WillTakeFood (csFood offer) {
		return false;
	}

	public virtual void TakeFood (csFood offer) {
	}

	public virtual bool WillModifyFood (csFood offer) {
		return false;
	}

	public virtual void ModifyFood (csFood offer) {
	}

	public virtual void StartFoodPrep (csFood _food) {
		food = _food;
		prepState = PrepState.Prepping;
		timeRemaining = prepTime;
		timerVizColor.color = new Color (0.2f, 0.9f, 0.2f);
		timerViz.value = 1.0f;
		timerViz.gameObject.SetActive (true);
	}

	public virtual void PrepComplete () {
		// Called back when prep on contained food is complete.  Override for stations that prep.
	}

	public virtual void StartFoodRuining () {
		prepState = PrepState.Ruining;
		timeRemaining = ruinTime;
		timerVizColor.color = new Color (0.9f, 0.2f, 0.2f);
		timerViz.value = 0.0f;
		timerViz.gameObject.SetActive (true);
	}

	public virtual void RuinComplete() {
		// Called back when ruining of contained food is complete.  Override for stations that ruin food.
	}

	public virtual void RuinFood () {
		prepState = PrepState.Ruined;
		timerViz.gameObject.SetActive (false);
		food.SetCookState (csFood.FoodCookState.Ruined);
	}

	public void TransformIn (csFood offer) {
		food = offer;
		food.transform.position = transform.position;
		food.transform.parent = transform;
	}

	public virtual void StopActivity () {
		// Usually called when food is being given away.  Cease all prep/ruin/etc activity.
		prepState = PrepState.None;
		timerViz.gameObject.SetActive (false);
	}

	protected void ShowError (string error) {
		Camera.main.GetComponent<csLevelManager> ().ShowError (error);
	}

}
