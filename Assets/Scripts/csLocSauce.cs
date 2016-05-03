using UnityEngine;
using System.Collections;

public class csLocSauce : csLocation {

	// Represents a kitchen location (surface/station) that holds sauces which instantly
	// apply to any ready-to-serve food.

	// DOES NOT TRANSFER.  Attempts to modify offered food directly.
	
	public csFood.SauceType sauceType;

	public override bool WillModifyFood (csFood offer) {
		if (offer.foodCookState != csFood.FoodCookState.Cooked) {
			ShowError ("Cook first!");
			return false;
		}
		if (offer.sauceType != csFood.SauceType.None) {
			ShowError ("Already sauced!");
			return false;
		}
		return true;
	}

	public override void ModifyFood (csFood offer) {
		offer.AddSauce (sauceType);
	}

}
