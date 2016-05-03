using UnityEngine;
using System.Collections;

public class csLocBatterHold : csLocation {

	public override bool WillGiveFood () {
		return (ContainsFood ());
	}
	
	public override csFood GiveFood () {
		csFood foodToReturn = food;
		food = null;
		return foodToReturn;
	}

	public override void TakeFood (csFood offer) {
		if (offer == null) return;
		TransformIn (offer);
	}

}
