using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour 
{
	//Speed at which objects are moved
	public float gameSpeed;
	public float distance;
	public int distanceWhole;
	public float timeToBlockInputAfterDeath = 2;

	public GameObject player;

	[SerializeField]
	public PlayerController playerController;

	//The objects that get moved allong the x
	public List<GameObject> objectsToMove;	

	public Text distanceUI;
	public Text deathUI;

	public List<AudioClip> speedUpStart;
	public List<AudioClip> speedUpSustain;
	public List<AudioClip> speedUpEnd;
	public List<AudioClip> wipeoutSounds;
	public List<AudioClip> hitSounds;
	public List<AudioClip> airSounds;
	public List<AudioClip> groundSounds;

	public GameObject waveChunkPrefab;
	public GameObject waveChunkParent;
	public GameObject wave;
	public float maxWaveLerpTime = 15.0f;
	public float minWaveLerpTime = 5.0f;

	[SerializeField]
	private LevelController levelController = new LevelController();
	private bool gameActive = true;
	private bool gamePaused = true;
	private bool lastInput = false;
	private AudioSource [] audioSources;
	private float inputBlockTimer;
	private List<GameObject> waveBackgrounds;
	private int numberOfBackgroundChunks = 5;

	private Vector3 waveTargetPosition;
	private Vector3 gameStartWavePosition;
	private Vector3 lerpStartWavePosition;
	private float waveLerpLength;
	private float waveLerpStartTime;

		
	// Use this for initialization
	private void Start () 
	{
		this.distanceUI.text = "";
		distance = 0.0f;
		distanceWhole = 0;
		this.gameStartWavePosition = this.wave.transform.localPosition;
		this.waveTargetPosition = Vector3.zero;
		playerController = GameObject.Find("Player").GetComponent<PlayerController>();
		playerController.Setup();
		this.audioSources = this.GetComponents<AudioSource> ();
		this.levelController.Setup();
		this.inputBlockTimer = this.timeToBlockInputAfterDeath;
		this.CreateInitialWaveBackgroundChunks ();
		this.UpdateWavePosition ();
		this.deathUI.gameObject.SetActive (false);
	}

	private void EndGame()
	{
		this.PauseGame ();
		this.gameActive = false;
		this.deathUI.gameObject.SetActive (true);
		int highest_score = this.GetHighestScore ();
		if (distanceWhole > highest_score) {
			this.UpdateHighestScore(distanceWhole);
			highest_score = distanceWhole;
		}
		this.deathUI.text = "You surfed " + distanceWhole + "m. Best record is " + highest_score + "m. Tap to restart.";
		this.inputBlockTimer = 0;
		this.objectsToMove.Remove (Camera.main.gameObject);

		this.waveTargetPosition = new Vector3 ( 26.5f, this.wave.transform.localPosition.y, this.wave.transform.localPosition.z ); 
		this.lerpStartWavePosition = this.wave.transform.localPosition;
		this.waveLerpLength = 3.0f;
		this.waveLerpStartTime = Time.time;
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
	private void Update ( ) 
	{
		bool inputDown = Input.GetKey ("space") || Input.GetMouseButton(0);
		this.playerController.is_down = inputDown;

		if ( this.gameActive && !this.gamePaused )
		{
			this.playerController.UpdateFrame ();
			this.UpdateFollowObjects ();
			this.UpdateUI ();
			this.UpdateSounds ( inputDown, lastInput );
			this.UpdateWaveBackgrounds ();
			this.UpdateWavePosition();
			this.levelController.Update ();

			if ( this.playerController.is_dead )
			{
				this.EndGame ();
			}
		}
		else
		{
			this.inputBlockTimer += Time.deltaTime;

			if ( inputDown && inputBlockTimer >= this.timeToBlockInputAfterDeath )
			{
				this.InputPressed ();
			}
		}

		if ( !this.gameActive && this.gamePaused )
		{
			this.UpdateWavePosition ();
		}

		this.lastInput = inputDown;
	}

	private void UpdateFollowObjects()
	{
		distance += gameSpeed;
		for (int i = 0; i < this.objectsToMove.Count; i++)
		{
			Vector3 newPosition = this.objectsToMove [i].transform.localPosition;
			newPosition.x += gameSpeed;
			this.objectsToMove [i].transform.localPosition = newPosition;
		}
	}

	private void UpdateSounds( bool newInput, bool oldInput )
	{
		if (this.playerController.is_dead && this.gameActive)
		{
			this.PlayRandomSound ( this.wipeoutSounds, true, 0 );
		}
		else if (newInput == true && oldInput == false)
		{
			this.PlayRandomSound ( this.speedUpStart, true, 1 );	
		}
		else if (newInput == true && oldInput == true)
		{
			this.PlayRandomSound ( this.speedUpSustain, false, 1 );	
		}
		else if (newInput == false && oldInput == true)
		{
			this.PlayRandomSound ( this.speedUpEnd, true, 1 );
		}
		else if (this.playerController.is_collide_now)
		{
			this.PlayRandomSound ( this.hitSounds, true, 0 );
		}
		else if (this.playerController.is_in_air)
		{
			this.PlayRandomSound ( this.airSounds, false, 0 );
		}
		else if (this.playerController.is_on_ground)
		{
			this.PlayRandomSound ( this.groundSounds, false, 0 );
		}
	}

	private void PlayRandomSound( List<AudioClip> soundClips, bool interuptSounds, int audioSourceIndex )
	{
		if ( soundClips.Count > 0)
		{
			if (!this.audioSources[audioSourceIndex].isPlaying || interuptSounds)
			{
				AudioClip clipToPlay = soundClips [Random.Range (0, soundClips.Count - 1)]; 
				this.audioSources[audioSourceIndex].clip = clipToPlay;
				this.audioSources[audioSourceIndex].Play ();
			}
		}
	}

	private void UpdateUI()
	{
		distanceWhole = (int)(Mathf.Round(distance));
		this.distanceUI.text = distanceWhole + " m";  
	}

	private void UpdateWaveBackgrounds()
	{
		if (this.waveBackgrounds [2].transform.localPosition.x < Camera.main.transform.localPosition.x)
		{
			CreateWaveBackgroundChunk ();
		}
	}

	private void CreateInitialWaveBackgroundChunks()
	{
		this.waveBackgrounds = new List<GameObject> ();
		for (int i = 0; i < numberOfBackgroundChunks; i++)
		{
			this.CreateWaveBackgroundChunk ();
		}
	}

	private void CreateWaveBackgroundChunk()
	{
		GameObject newBackgroundChunk = (GameObject)GameObject.Instantiate ( this.waveChunkPrefab );

		float newChunkX = -16;
		if ( this.waveBackgrounds.Count >= 1 )
		{
			newChunkX = this.waveBackgrounds [this.waveBackgrounds.Count - 1].transform.Find ("Edge").transform.position.x;
		}

		newBackgroundChunk.transform.parent = this.waveChunkParent.transform;
		newBackgroundChunk.transform.localPosition = new Vector3 ( newChunkX,this.waveChunkPrefab.transform.position.y, 0);
		this.waveBackgrounds.Add ( newBackgroundChunk );

		if ( this.waveBackgrounds.Count > this.numberOfBackgroundChunks )
		{
			GameObject oldBackgroundChunk = this.waveBackgrounds [0];
			this.waveBackgrounds.Remove (oldBackgroundChunk);
			UnityEngine.Object.Destroy (oldBackgroundChunk);
		}
	}

	private void UpdateWavePosition()
	{
		if ( this.waveTargetPosition == Vector3.zero )
		{
			Vector3 newWaveTarget = this.gameStartWavePosition;
			newWaveTarget.x = newWaveTarget.x + Random.Range (-5.0f, 5.0f);
			this.waveTargetPosition = newWaveTarget; 
			this.waveLerpLength = Random.Range (this.minWaveLerpTime, this.maxWaveLerpTime);
			this.lerpStartWavePosition = this.wave.transform.localPosition;
			this.waveLerpStartTime = Time.time;
		}

		if ( this.wave.transform.localPosition == this.waveTargetPosition && this.gameActive)
		{
			this.waveTargetPosition = Vector3.zero;
		}
		else
		{
			this.wave.transform.localPosition = Vector3.Lerp( this.lerpStartWavePosition, this.waveTargetPosition, ( Time.time - this.waveLerpStartTime ) / this.waveLerpLength );
		}
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

	private void UpdateHighestScore(int score)
	{
		System.IO.File.WriteAllText("Assets/HighestScore.txt", score.ToString());
	}

	private int GetHighestScore()
	{
		int highest_score = 0;
		int.TryParse(System.IO.File.ReadAllText("Assets/HighestScore.txt"), out highest_score);
		return highest_score;
	}
}
