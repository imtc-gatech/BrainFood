using UnityEngine;
using System.Collections;

public class csLocBatter : csLocation {

	public csLocBatterHold[] holdLocs;

	public override void Awake () {
		base.Awake();

		// Set prep time to override base location.  In code to allow upgrades.
		// Food is never ruined here.
		prepTime = 5.0f;
	}

	public override bool WillGiveFood () {
		return (ContainsFood () && (prepState != PrepState.Prepping));
	}
	
	public override csFood GiveFood () {
		csFood foodToReturn = food;
		food = null;
		StopActivity ();
		return foodToReturn;
	}

	public override bool WillTakeFood (csFood offer) {
		// Do not judge based on current occupancy.
		if (offer.foodCookState != csFood.FoodCookState.Raw) {
			ShowError("Batter raw food only!");
			return false;
		}
		if (offer.foodBatterState == csFood.FoodBatterState.Battered) {
			ShowError("Already battered!");
			return false;
		}
		if ((offer.foodType == csFood.FoodType.Steak) || (offer.foodType == csFood.FoodType.Shrimp)) {
			// Raw steak or shrimp can be battered.
			return true;
		}
		if (offer.foodType == csFood.FoodType.Chicken) {
			if (offer.foodChopState == csFood.FoodChopState.Chopped) {
				// Cut chicken (tenders) can be battered.
				return true;
			} else {
				ShowError("Chop chicken before battering!");
				return false;
			}
		}
		ShowError ("Can't batter that!");
		return false;
	}

	public override void TakeFood (csFood offer) {
		if (offer == null) return;
		TransformIn (offer);
		StartFoodPrep (offer);
		Debug.Log ("Battering state is now: " + prepState);
	}

	public override void PrepComplete () {
		food.SetBatterState (csFood.FoodBatterState.Battered);
		StopActivity ();
		// Transfer food to available batterhold station.
		foreach (csLocBatterHold holdLoc in holdLocs) {
			if (!holdLoc.ContainsFood()) {
				holdLoc.TakeFood(GiveFood());
				return;
			}
		}
		// No hold loc open; keep food.
	}

}
