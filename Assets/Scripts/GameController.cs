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

	//All of our options for chunks
	public List<GameObject> levelChunkOptions;

	public Text meterCounter; 

	private LevelController levelController;

	private bool gameActive = true;
	private bool gamePaused = true;
		
	// Use this for initialization
	private void Start () 
	{
		playerController = GameObject.Find("Player").GetComponent<PlayerController>();
		playerController.Setup();
		this.levelController = new LevelController ( this.levelChunkOptions );
	}

	private void EndGame()
	{
		this.PauseGame ();
		this.gameActive = false;
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

	private void UpdateUI()
	{
		float inflatedXPosition = Mathf.Round (Camera.main.transform.localPosition.x * 10000) / 100;
		this.meterCounter.text = "" + inflatedXPosition + " M";  
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
