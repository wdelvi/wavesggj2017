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

	private Sprite[] obstacles;
	private Sprite[] obstaclesTall;

	public void Setup () 
	{
		handStart = -1;
		this.obstacles = Resources.LoadAll<Sprite> ("Obstacles");
		this.obstaclesTall = Resources.LoadAll<Sprite> ("obstaclesTall");

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
		for (int i = 0; i < handLength; i++)
		{
			this.PlaceCardInHand ();
		}
	}

	private void PlaceCardInHand()
	{
		GameObject newLevelChunk = (GameObject)GameObject.Instantiate ( this.DrawLevelCard () );

		float newChunkX = 64;
		if (this.levelChunkHand.Count >= 1)
		{
			newChunkX = this.levelChunkHand [this.levelChunkHand.Count - 1].transform.Find ("Bounds").transform.position.x;
		}

		newLevelChunk.transform.localPosition = new Vector3 ( newChunkX, 0, 0 );
		newLevelChunk.transform.parent = this.levelChunkHolder.transform;
		this.levelChunkHand.Add ( newLevelChunk );

		this.ReplacePlaceholderObstacles (newLevelChunk); 

		if ( this.levelChunkHand.Count > handLength )
		{
			GameObject oldLevelChunk = this.levelChunkHand [0];
			this.levelChunkHand.Remove (oldLevelChunk);
			UnityEngine.Object.Destroy (oldLevelChunk);
		}
	}

	// Behind wave.
	// Lower on screen in front.
	// Set z position to y position.
	public void SetWorldZtoWorldY(GameObject spriteObject)
	{
		Vector3 world = spriteObject.transform.position;
		world.z = world.y;
		spriteObject.transform.position = world;
	}

	private void ReplacePlaceholderObstacles( GameObject levelChunk )
	{
		foreach ( Transform child in levelChunk.transform ) 
		{
			if (child.tag == "Obstacle")
			{
				this.ReplaceTransformObjectSpriteWithRandom (child, this.obstacles);
			}
			else if (child.tag == "ObstacleTall")
			{
				this.ReplaceTransformObjectSpriteWithRandom (child, this.obstaclesTall);	
			}
		}
	}

	private void ReplaceTransformObjectSpriteWithRandom( Transform toReplace, Sprite[] spriteArray )
	{
		Sprite newSprite = spriteArray [ (int)Mathf.Floor (Random.Range (0, spriteArray.Length)) ];
		GameObject spriteObject = toReplace.Find ("Sprite").gameObject;
		spriteObject.GetComponent<SpriteRenderer> ().sprite = newSprite;
		this.SetWorldZtoWorldY (spriteObject);
	}
}
