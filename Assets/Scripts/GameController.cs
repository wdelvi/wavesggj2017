﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour 
{
	//Speed at which objects are moved
	public float gameSpeed;

	public GameObject player;

	[SerializeField]
	public PlayerController playerController;

	//The objects that get moved allong the x
	public List<GameObject> objectsToMove;	

	public Text distanceUI;
	public Text deathUI;

	[SerializeField]
	private LevelController levelController = new LevelController();
	private bool gameActive = true;
	private bool gamePaused = true;
	private bool lastInput = false;
		
	// Use this for initialization
	private void Start () 
	{
		playerController = GameObject.Find("Player").GetComponent<PlayerController>();
		playerController.Setup();
		this.levelController.Setup();
	}

	private void EndGame()
	{
		this.PauseGame ();
		this.gameActive = false;
		this.deathUI.gameObject.SetActive (true);
	}

	private void RestartGame()
	{
		SceneManager.LoadScene( "mainScene", LoadSceneMode.Single );
	}

	private void PauseGame()
	{
		this.gamePaused = true;
	}

	private void UnpauseGame()
	{
		this.gamePaused = false;
	}
	
	// Update is called once per frame
	private void Update () 
	{
		bool inputDown = Input.GetKey ("space") || Input.GetMouseButton(0);
		this.playerController.is_down = inputDown;

		if ( this.gameActive && !this.gamePaused )
		{
			this.playerController.UpdateFrame ();
			this.UpdateFollowObjects ();
			this.UpdateUI ();
			this.PlaySounds ( inputDown, lastInput );
			this.levelController.Update ();

			if ( this.playerController.is_dead )
			{
				this.EndGame ();
			}
		}
		else
		{
			if ( inputDown )
			{
				this.InputPressed ();
			}
		}

		this.lastInput = inputDown;
	}

	private void UpdateFollowObjects()
	{
		for (int i = 0; i < this.objectsToMove.Count; i++)
		{
			Vector3 newPosition = this.objectsToMove [i].transform.localPosition;
			newPosition.x += gameSpeed;
			this.objectsToMove [i].transform.localPosition = newPosition;
		}
	}

	private void PlaySounds( bool newInput, bool oldInput )
	{
		if (this.playerController.is_in_wave)
		{
			
		}
		else if (this.playerController.is_in_air)
		{

		}
		else if (this.playerController.is_on_ground)
		{

		}
		else if (this.playerController.is_in_air)
		{

		}
	}

	private void UpdateUI()
	{
		float inflatedXPosition = Mathf.Round ( Camera.main.transform.localPosition.x * 10000 ) / 100;
		this.distanceUI.text = "" + inflatedXPosition + " M";  
	}

	private void InputPressed()
	{
		if ( this.gameActive && this.playerController.is_dead )
		{
			this.EndGame ();
		}
		else if ( this.gamePaused )
		{
			if (this.playerController.is_dead)
			{
				this.RestartGame ();
			}
			else
			{
				this.UnpauseGame ();
			}
		}
	}
}
