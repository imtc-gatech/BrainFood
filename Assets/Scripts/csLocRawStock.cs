using UnityEngine;
using System.Collections;

public class csLocRawStock : csLocation {

	// Represents a kitchen location (surface/station) that holds infinite ingredients of a single type.

	// TRANSFER IN: never.
	// TRANSFER OUT: requires one free hand.  Also creates a new copy of food.
	
	public csFood.FoodType foodType;


	public override void Awake () {
		Debug.Log ("Child awake.");
		base.Awake ();

		// Create one copy of food type (raw).
		MakeFood ();
	}

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void MakeFood () {
		// Creates a new food object.  This is the only place this occurs.
		food = csFood.MakeFood(foodType, transform.position);
	}

	public override bool WillGiveFood () {
		return true;
	}

	public override csFood GiveFood () {
		csFood foodToReturn = food;
		MakeFood ();
		return foodToReturn;
	}

	// No override implementation for raw stock taking food, as they never will.

}
