using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour {
	
	//variables visible in the inspector
	public Animator UIAnimator;
	public GameObject muteButtonLine;
	
	void Start(){		
		//get the wanted audio level
		int audio = PlayerPrefs.GetInt("Audio");
		//get the opposite (if the player prefs are resetted, we do want to have sound by default)
		audio = (audio == 0) ? 1 : 0;
		//set the game volume and show the red line
		AudioListener.volume = audio;
		muteButtonLine.SetActive(audio == 0);
	}
	
	void Update(){
		//if we press enter of left mouse button start the game
		if(Input.GetKeyDown(KeyCode.Return) || (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())){
			if(!(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))){
				StartGame();
			}
		}
	}

	public void StartGame(){
		//fade to black and load the game scene
		UIAnimator.SetTrigger("Start");
		StartCoroutine(LoadScene("Game"));
	}
	
	public void Mute(){
		//set the audio volume to the opposite of our current volume
		if(AudioListener.volume == 0){
			AudioListener.volume = 1;
		}
		else{
			AudioListener.volume = 0;
		}
		
		//show the red line on the music button if the music is disabled
		muteButtonLine.SetActive(AudioListener.volume == 0);
		//update the player prefs value for the music
		PlayerPrefs.SetInt("Audio", ((int)AudioListener.volume == 0) ? 1 : 0);
	}
	
	//wait less than a second and load the given scene
	IEnumerator LoadScene(string scene){
		yield return new WaitForSeconds(0.6f);
		
		SceneManager.LoadScene(scene);
	}
}