﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject {

	public int wallDamage = 1;
	public int pointsPerfood = 10;
	public int pointsPerSoda = 20;
	public float restartLevelDelay = 1f;
	public Text foodText;

	private Animator animator;
	private int food;

	// Use this for initialization
	protected override void Start () {
		animator = GetComponent<Animator> ();
		food = GameManager.instance.playerFoodPoints;
		foodText.text = string.Format ("Food: {0}", food);

		base.Start ();
	}

	private void OnDisable(){	
		GameManager.instance.playerFoodPoints = food;
	}

	protected override void AttemptMove<T> (int xDir, int yDir){
		food--;
		foodText.text = string.Format ("Food: {0}", food);
		base.AttemptMove<T> (xDir, yDir);
		RaycastHit2D hit;

		if(Move(xDir, yDir, out hit)){
			// call randomisesfx sound manager to play the move sound
		}

		CheckIfGameOver ();
		GameManager.instance.playersTurn = false;
	}


	private void CheckIfGameOver(){
		if (food <= 0)
			GameManager.instance.GameOver ();
	}

	// Update is called once per frame
	void Update () {
		if (!GameManager.instance.playersTurn)
			return;

		int horizontal = 0;
		int vertical = 0;

		horizontal = (int)Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");

		if (horizontal != 0)
			vertical = 0;

		if (horizontal != 0 || vertical != 0)
			AttemptMove<Wall> (horizontal, vertical);		
	}

	private void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Exit") {
			Invoke ("Restart", restartLevelDelay);
			enabled = false;
		} else if (other.tag == "Food") {
			food += pointsPerfood;
			foodText.text = string.Format ("+{0} Food: {1}", pointsPerfood, food);

			other.gameObject.SetActive (false);
		} else if (other.tag == "Soda") {
			food += pointsPerSoda;
			foodText.text = string.Format ("+{0} Food: {1}", pointsPerSoda, food);
			other.gameObject.SetActive (false);
		}
	}

	protected override void OnCantMove<T>(T component){
		Wall hitWall = component as Wall;
		hitWall.DamageWall (wallDamage);
		animator.SetTrigger ("playerChop");
	}

	private void Restart(){
		//Application.LoadLevel (Application.loadedLevel)
		SceneManager.LoadScene (0);

		//SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void LoseFood(int loss){
		animator.SetTrigger ("playerHit");
		food -= loss;
		foodText.text = string.Format ("-{0} Food: {1}", loss, food);

		CheckIfGameOver ();
	}
}
