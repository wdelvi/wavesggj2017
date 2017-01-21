﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController 
{
	//All of our options for chunks
	private List<GameObject> levelChunkOptions;

	//All of our options starting shuffled and removing as used
	private List<GameObject> levelChunkDeck;

	//The current chunks that are instantiated into the game
	private List<GameObject> levelChunkHand;

	private GameObject levelChunkHolder;

	public LevelController ( List<GameObject> levelChunkOptions, GameObject levelChunkHolder ) 
	{
		this.levelChunkOptions = levelChunkOptions;

		this.levelChunkHolder = levelChunkHolder;

		this.levelChunkHand = new List<GameObject>();

		this.ShuffleDeck ();

		this.CreateInitialHand ();
	}

	public void Update () 
	{
		if ( this.levelChunkHand.Count > 2 && this.levelChunkHand [2].transform.localPosition.x < Camera.main.transform.localPosition.x )
		{
			this.PlaceCardInHand ();
		}
	}

	private void ShuffleDeck()
	{
		this.levelChunkDeck = new List<GameObject>( this.levelChunkOptions );

		for (int i = 0; i < this.levelChunkDeck.Count; i++)
		{
			GameObject temp = this.levelChunkDeck [i];
			int randomIndex = (int) Random.Range (i, levelChunkDeck.Count); 
			this.levelChunkDeck [i] = this.levelChunkDeck [randomIndex];
			this.levelChunkDeck [randomIndex] = temp;
		}
	}

	private GameObject DrawLevelCard()
	{
		GameObject levelCard = null;

		if( this.levelChunkDeck.Count > 0 )
		{
			levelCard = this.levelChunkDeck [0];
			this.levelChunkDeck.Remove ( this.levelChunkDeck [0] );
		}
		else
		{
			this.ShuffleDeck ();
			levelCard = this.DrawLevelCard ();
		}

		return levelCard;
	}

	private void CreateInitialHand( )
	{
		GameObject chunkOne = (GameObject)GameObject.Instantiate ( this.DrawLevelCard () );
		GameObject chunkTwo = (GameObject)GameObject.Instantiate ( this.DrawLevelCard () );
		GameObject chunkThree = (GameObject)GameObject.Instantiate ( this.DrawLevelCard () );

		this.levelChunkHand.Add ( chunkOne );
		this.levelChunkHand.Add ( chunkTwo );
		this.levelChunkHand.Add ( chunkThree );
		 
		chunkOne.transform.localPosition = new Vector3 ( Screen.width, 0, 0 ); 
		chunkTwo.transform.localPosition = new Vector3 ( chunkOne.transform.Find("Bounds").transform.localPosition.x, 0, 0 );
		chunkThree.transform.localPosition = new Vector3 ( chunkTwo.transform.Find("Bounds").transform.localPosition.x, 0, 0 );

		chunkOne.transform.parent = this.levelChunkHolder.transform;
		chunkTwo.transform.parent = this.levelChunkHolder.transform;
		chunkThree.transform.parent = this.levelChunkHolder.transform;
	}

	private void PlaceCardInHand()
	{
		GameObject newLevelChunk = (GameObject)GameObject.Instantiate ( this.DrawLevelCard () );
		newLevelChunk.transform.localPosition = new Vector3 ( this.levelChunkHand[this.levelChunkHand.Count-1].transform.Find("Bounds").transform.position.x, 0, 0 );
		newLevelChunk.transform.parent = this.levelChunkHolder.transform;
		this.levelChunkHand.Add ( newLevelChunk );

		GameObject oldLevelChunk = this.levelChunkHand [0];
		this.levelChunkHand.Remove ( oldLevelChunk );
		UnityEngine.Object.Destroy ( oldLevelChunk );
	}
}
