using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[SerializeField]
	private float speed_h = 0.5f;	// horizontal speed

	[SerializeField]
	private float speed_wave = 2f;	// vertical speed of wave

	[SerializeField]
	private float speed_press = 3f;	// vertical speed of pressing

	[SerializeField]
	private bool is_dead;			// player is dead, disable input

	[SerializeField]
	private bool is_in_wave;		// player is inside of a wave

	private float speed_v;			// vertical speed
	private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		is_dead = false;
		is_in_wave = true;
		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!is_dead) {	// if player is alive, accept input
			// movement
			if (is_in_wave && Input.GetKey ("space")) {
				// force down
				speed_v = speed_wave - speed_press;
			} else if (is_in_wave) {
				speed_v = speed_wave;
			}
			if (speed_v >= 0) {
				rb.AddForce (Vector2.up * speed_v);
			} else {
				rb.AddForce (Vector2.down * speed_v * -1);
			}
			transform.Translate (Vector2.right * speed_h * Time.deltaTime);
		}
	}

	public void SetDead (bool _is_dead) {
		is_dead = _is_dead;
	}

	public void SetInWave (bool _is_in_wave) {
		is_in_wave = _is_in_wave;
	}

	void OnColliderEnter2D (Collision2D other) {
		if (other.gameObject.tag == "Obstacle") {
			Debug.Log ("Hit obstacle");
			SetDead (true);
		} 
	}

	void OnTriigerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Wave") {
			Debug.Log ("Enter wave");
			SetInWave (true);
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.gameObject.tag == "Wave") {
			Debug.Log ("Leave wave");
			SetInWave (false);
		}
	}
}
