using UnityEngine;
using System.Collections;

public class csLocGrill : csLocation {

	public override void Awake () {
		base.Awake();

		// Set prep and ruin time to override base location.  In code to allow upgrades.
		prepTime = 15.0f;
		ruinTime = 10.0f;
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
			// Raw food only.
			ShowError("Grill raw food only!");
			return false;
		}
		if (offer.foodBatterState == csFood.FoodBatterState.Battered) {
			// No battered food on the grill.
			ShowError("Can't grill battered food!");
			return false;
		}
		if ((offer.foodType == csFood.FoodType.Broccoli) && (offer.foodChopState != csFood.FoodChopState.Chopped)) {
			// No unchopped broccoli.
			ShowError("Chop broccoli first!");
			return false;
		}
		if ((offer.foodType == csFood.FoodType.Potato) && (offer.foodChopState == csFood.FoodChopState.Chopped)) {
			// No chopped potatoes.  (Fries get fried, not grilled.)
			ShowError("Can't grill fries!");
			return false;
		}
		if ((offer.foodType == csFood.FoodType.Chicken) && (offer.foodChopState == csFood.FoodChopState.Chopped)) {
			// Chopped chicken (tenders) needs to be battered and fried, not grilled.
			ShowError("Can't grill tenders!");
			return false;
		}
		return true;
	}

	public override void TakeFood (csFood offer) {
		if (offer == null) return;
		TransformIn (offer);
		StartFoodPrep (offer);
		Debug.Log ("Grill state is now: " + prepState);
	}

	public override void PrepComplete () {
		food.SetCookState (csFood.FoodCookState.Cooked);
		StartFoodRuining ();
	}

	public override void RuinComplete () {
		RuinFood ();
	}

}
