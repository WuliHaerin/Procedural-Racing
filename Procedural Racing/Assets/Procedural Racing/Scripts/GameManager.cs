using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	
	//visible in the inspector
	public Text scoreLabel;
	public Text timeLabel;
	public Text gameOverScoreLabel;
	public Text gameOverBestLabel;
	public Animator scoreEffect;
	public Animator UIAnimator;
	public Animator gameOverAnimator;
	public AudioSource gameOverAudio;
	public Car car;
	
	//not visible in the inspector
	float time;
	int score;
	
	bool gameOver;
	
	void Start(){
		//show the initial score of 0
		UpdateScore(0);
	}
	
	void Update(){
		//show the current time
		UpdateTimer();
		
		//restart the game if we're game over and pressed enter or left mouse button
		if(gameOver && (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))){
			UIAnimator.SetTrigger("Start");
			StartCoroutine(LoadScene(SceneManager.GetActiveScene().name));
		}
	}
	
	void UpdateTimer(){
		//add time
		time += Time.deltaTime;
		int timer = (int)time;
		
		//get the minutes and seconds
		int seconds = timer % 60;
		int minutes = timer/60;
		
		//put those in a string with correct 0s
		string secondsRounded = ((seconds < 10) ? "0" : "") + seconds;
		string minutesRounded = ((minutes < 10) ? "0" : "") + minutes;
		
		//show the time
		timeLabel.text = minutesRounded + ":" + secondsRounded;
	}
	
	public void UpdateScore(int points){
		//add score
		score += points;
		
		//update the score text
		scoreLabel.text = "" + score;
		
		//show the blue vignette animation
		if(points != 0)
			scoreEffect.SetTrigger("Score");
	}

	public GameObject AdPanel;
	public bool isCancelAd;
	public bool isInvincible;

	public void SetAdPanel(bool a)
	{
		AdPanel.SetActive(a);
		Time.timeScale = a ? 0 : 1;
	}

	public void PreDie()
	{
		if (isCancelAd)
		{
			GameOver();
		}
		else
		{
			SetAdPanel(true);
		}
	}

	public void CancelAd()
	{
		isCancelAd = true;
		SetAdPanel(false);
		GameOver();
	}


	public void Revive()
	{
        AdManager.ShowVideoAd("192if3b93qo6991ed0",
           (bol) => {
               if (bol)
               {
                   SetAdPanel(false);
                   StartCoroutine(Invincibel());

                   AdManager.clickid = "";
                   AdManager.getClickid();
                   AdManager.apiSend("game_addiction", AdManager.clickid);
                   AdManager.apiSend("lt_roi", AdManager.clickid);


               }
               else
               {
                   StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
               }
           },
           (it, str) => {
               Debug.LogError("Error->" + str);
               //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
           });

	}

	public GameObject CountDown;
	public IEnumerator Invincibel()
	{
		isInvincible= true;
		CountDown.SetActive(true);
		yield return new WaitForSeconds(3);
		isInvincible = false;
		CountDown.SetActive(false);
	}

	public void GameOver(){
		//the game cannot be over multiple times so we need to return if the game was over already
		if(gameOver)
			return;
		
		//update the score and highscore
		SetScore();
		
		//show the game over animation and play the audio
		gameOverAnimator.SetTrigger("Game over");
		gameOverAudio.Play();
		
		//the game is over
		gameOver = true;
		
		//break the car
		car.FallApart();
		
		//stop the world from moving or rotating
		foreach(BasicMovement basicMovement in GameObject.FindObjectsOfType<BasicMovement>()){
			basicMovement.movespeed = 0;
			basicMovement.rotateSpeed = 0;
		}

        AdManager.ShowInterstitialAd("1lcaf5895d5l1293dc",
           () => {
               Debug.Log("--插屏广告完成--");

           },
           (it, str) => {
               Debug.LogError("Error->" + str);
           });
    }
	
	void SetScore(){
		//update the highscore if our score is higher then the previous best score
		if(score > PlayerPrefs.GetInt("best"))
			PlayerPrefs.SetInt("best", score);
		
		//show the score and the high score
		gameOverScoreLabel.text =score.ToString();
		gameOverBestLabel.text = PlayerPrefs.GetInt("best").ToString();
	}
	
	//wait less than a second and load the given scene
	IEnumerator LoadScene(string scene){
		yield return new WaitForSeconds(0.6f);
		
		SceneManager.LoadScene(scene);
	}
}
