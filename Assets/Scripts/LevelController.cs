using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelController 
{
	public GameObject levelChunkHolder;

	//All of our options for chunks
	public List<GameObject> levelChunkLibrary;
	public List<GameObject> levelChunkOptions;

	//All of our options starting shuffled and removing as used
	public List<GameObject> levelChunkDeck;

	//The current chunks that are instantiated into the game
	public List<GameObject> levelChunkHand;
	public int handLength = 3;
	public int handStep = 1;
	public int handStart = -1;

	public void Setup () 
	{
		handStart = -1;
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

	private void IncrementLibrary()
	{
		handStart = Mathf.Min(handStart + handStep, this.levelChunkLibrary.Count - handLength);
		this.levelChunkOptions = this.levelChunkLibrary.GetRange(handStart, handLength);
	}

	private void ShuffleDeck()
	{
		IncrementLibrary();
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
