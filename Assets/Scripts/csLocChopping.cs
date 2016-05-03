using UnityEngine;
using System.Collections;

public class csLocChopping : csLocation {

	public csLocChopHold[] holdLocs;

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
			ShowError("Chop raw food only!");
			return false;
		}
		if (offer.foodChopState == csFood.FoodChopState.Chopped) {
			ShowError("Already chopped!");
			return false;
		}
		if ((offer.foodType == csFood.FoodType.Potato) ||
		 	(offer.foodType == csFood.FoodType.Broccoli) ||
		    (offer.foodType == csFood.FoodType.Chicken)) {
			// Only some food can be chopped.
			return true;
		}
		ShowError ("Can't chop that!");
		return false;
	}

	public override void TakeFood (csFood offer) {
		if (offer == null) return;
		TransformIn (offer);
		StartFoodPrep (offer);
		Debug.Log ("Chopping state is now: " + prepState);
	}

	public override void PrepComplete () {
		food.SetChopState (csFood.FoodChopState.Chopped);
		StopActivity ();
		// Transfer food to available chophold station.
		foreach (csLocChopHold holdLoc in holdLocs) {
			if (!holdLoc.ContainsFood()) {
				holdLoc.TakeFood(GiveFood());
				return;
			}
		}
		// Couldn't transfer.  Chopping station keeps food.
	}

}
