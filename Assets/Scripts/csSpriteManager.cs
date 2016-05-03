using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class csSpriteManager : MonoBehaviour {

	public Sprite[] spriteList;

	public Dictionary<string,Sprite> sprites;

	void Awake () {
		// Build a sprite dictionary by name.
		sprites = new Dictionary<string, Sprite> ();

		foreach (Sprite tmpSprite in spriteList) {
			sprites[tmpSprite.name] = tmpSprite;
		}
	}

}
