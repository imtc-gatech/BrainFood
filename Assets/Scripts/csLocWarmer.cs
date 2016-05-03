using UnityEngine;
using System.Collections;

public class csLocWarmer : csLocation {

	public override void Awake () {
		base.Awake();

		// Set prep and ruin time to override base location.  In code to allow upgrades.
		ruinTime = 60.0f;
	}

	public override bool WillGiveFood () {
		return (ContainsFood ());
	}
	
	public override csFood GiveFood () {
		csFood foodToReturn = food;
		food = null;
		StopActivity ();
		return foodToReturn;
	}

	public override bool WillTakeFood (csFood offer) {
		// Do not judge based on current occupancy.
		if (offer.foodCookState != csFood.FoodCookState.Cooked) {
			// Cooked food only.
			ShowError("Cooked food only!");
			return false;
		}
		return true;
	}

	public override void TakeFood (csFood offer) {
		if (offer == null) return;
		TransformIn (offer);
		StartFoodRuining ();
		Debug.Log ("Warmer state is now: " + prepState);
	}

	public override void RuinComplete () {
		RuinFood ();
	}

}
