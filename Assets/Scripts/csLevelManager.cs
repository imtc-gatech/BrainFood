using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class csLevelManager : MonoBehaviour {

	public GameObject prefabFood;
	public Image soundIcon;

	public AudioClip sndFoodDelivered;
	public AudioClip sndCustomerLeft;

	public Sprite soundOn;
	public Sprite soundOff;

	public Image errorBubble;
	public Text errorMsg;

	private AudioSource music;
	private AudioSource soundfx;
	private bool isMusicPaused = false;

	private bool isErrorDisplayed = false;
	private bool isCameraPanning = false;

	private Camera playCam;

	public int levelNumber;

	void Awake () {
		Application.targetFrameRate = 60;
		music = transform.FindChild ("Music").GetComponent<AudioSource> ();
		soundfx = transform.FindChild ("SoundFX").GetComponent<AudioSource> ();
		playCam = Camera.main.GetComponent<Camera> ();
		errorBubble.gameObject.SetActive (false);
		errorMsg.gameObject.SetActive (false);
	}
	
	public void NextLevel() {
		int nextlvl = levelNumber + 1;
		if (nextlvl > 3) {
			nextlvl = 0;
		}
		Application.LoadLevel (nextlvl);
	}

	public void ToggleMusicPause () {
		if (isMusicPaused) {
			music.UnPause();
			soundIcon.sprite = soundOn;
			isMusicPaused = false;
		} else {
			music.Pause();
			soundIcon.sprite = soundOff;
			isMusicPaused = true;
		}
	}

	public void Celebrate () {
		// Obviously, this would become a general function with an enumerated
		// type specifying what sort of sound to play.
		if (!isMusicPaused) {
			soundfx.PlayOneShot (sndFoodDelivered, 1.0f);
		}
	}

	public void CustomerLeft () {
		if (!isMusicPaused) {
			soundfx.PlayOneShot (sndCustomerLeft, 0.5f);
		}
	}

	public void ShowError (string error) {
		StartCoroutine (DisplayTimedError (error));
	}

	IEnumerator DisplayTimedError (string error) {
		// Use following code block if a queue of errors is desired.
		/*
		while (isErrorDisplayed) {
			yield return null;
		}
		*/
		// Use following code block to disregard new errors if one is displayed.
		if (isErrorDisplayed) {
			yield break;
		}
		isErrorDisplayed = true;
		errorMsg.text = error;
		errorBubble.gameObject.SetActive (true);
		errorMsg.gameObject.SetActive (true);
		yield return new WaitForSeconds (1.0f);
		errorBubble.gameObject.SetActive (false);
		errorMsg.gameObject.SetActive (false);
		isErrorDisplayed = false;
	}

	public void ToDining () {
		StartCoroutine (CameraToDining ());
	}

	public void ToKitchen () {
		StartCoroutine (CameraToKitchen ());
	}

	IEnumerator CameraToDining () {
		while (isCameraPanning) {
			yield return null;
		}
		isCameraPanning = true;
		while (playCam.transform.position.x > -1024) {
			playCam.transform.position += (4000 * Vector3.left * Time.deltaTime);
			yield return null;
		}
		playCam.transform.position = new Vector3 (-1024.0f, 0, -100);
		isCameraPanning = false;
	}

	IEnumerator CameraToKitchen () {
		while (isCameraPanning) {
			yield return null;
		}
		isCameraPanning = true;
		while (playCam.transform.position.x < 0) {
			playCam.transform.position -= (4000 * Vector3.left * Time.deltaTime);
			yield return null;
		}
		playCam.transform.position = new Vector3 (0, 0, -100);
		isCameraPanning = false;
	}

}
