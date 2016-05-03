using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class csLocTrash : csLocation {

	public override bool WillTakeFood (csFood offer) {
		// The trash always takes food.  (Could make it ruined food only.)
		return true;
	}

	public override void TakeFood (csFood offer) {
		Destroy (offer.gameObject, 0.1f);
	}

}
