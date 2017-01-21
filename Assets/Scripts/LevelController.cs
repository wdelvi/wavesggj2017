using System.Collections;
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

	public LevelController ( List<GameObject> levelChunkOptions ) 
	{
		this.levelChunkOptions = levelChunkOptions;

		this.levelChunkHand = new List<GameObject>();

		this.ShuffleDeck ();

		this.CreateInitialHand ();
	}

	public void Update () 
	{
		
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

		this.levelChunkHand.Add ( chunkOne );
		this.levelChunkHand.Add ( chunkTwo );
		 
		chunkOne.transform.localPosition = new Vector3 ( 0, 0, 0 ); 
		chunkTwo.transform.localPosition = new Vector3 ( chunkOne.transform.Find("bounds").transform.localPosition.x, 0, 0 );
	}
}
