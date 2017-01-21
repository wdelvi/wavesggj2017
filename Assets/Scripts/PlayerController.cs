using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed_h = 0.0f;	// horizontal speed
	public float speed_h_in_wave = -2.0f;
	public float speed_h_in_air = -3.0f;
	public float speed_h_on_ground = -3.0f;
	public float speed_h_down_add = 4.0f;

	public float speed_wave = 5f;	// vertical speed of wave

	public float speed_press = 10f;	// vertical speed of pressing

	public bool is_input_enabled = true;
	public bool is_dead;			// player is dead, disable input

	public bool is_in_air;		// player at top
	public bool is_in_wave;		// player in middle
	public bool is_on_ground;		// player is on ground

	private float speed_v;			// vertical speed
	private float air_y = -1.0f;
	private float ground_y = -1.0f;
	private Rigidbody2D rb;

	// private float starting_y;

	public bool is_down;

	// Use this for initialization
	public void Setup() {
		is_dead = false;
		is_down = false;
		is_in_wave = true;
		is_in_air = false;
		is_on_ground = false;
		rb = GetComponent<Rigidbody2D> ();
		// this.starting_y = this.gameObject.transform.localPosition.y;
	}

	private void UpdateHorizontalSpeed() {
		if (is_in_air)
		{
			speed_h = speed_h_in_air;
		}
		else if (is_on_ground)
		{
			speed_h = speed_h_on_ground;
		}
		else if (is_in_wave)
		{
			speed_h = speed_h_in_wave;
		}
		if (is_down)
		{
			speed_h += speed_h_down_add;
		}
	}

	// force down on press
	// If out of bounds, return to bounds.
	private void UpdateVerticalSpeed() {
		if (is_in_air) {
			speed_v = -speed_wave;
		} else if (is_on_ground) {
			speed_v = speed_wave;
		} else if (is_in_wave && is_down) {
			speed_v = speed_wave - speed_press;
		} else if (is_in_wave) {
			speed_v = speed_wave;
		}
	}

	// Update is called once per frame
	// If player is alive, input moves player down.
	// Always update if the mouse button or Space key was pressed.
	// So the game controller can know when button is pressed at title screen.
	public void UpdateFrame() {
		if (!is_dead && is_input_enabled) {
			UpdateHorizontalSpeed();
			UpdateVerticalSpeed();
			/*
			if (speed_v >= 0) {
				rb.AddForce (Vector2.up * speed_v);
			} else {
				rb.AddForce (Vector2.down * speed_v * -1);
			}
			 */
			transform.Translate ( Vector2.up * speed_v * Time.deltaTime );
			transform.Translate ( Vector2.right * speed_h * Time.deltaTime );
		}
	}

	public void SetDead (bool _is_dead) {
		is_dead = _is_dead;
	}

	public void SetInAir (bool _is_in_air) {
		is_in_air = _is_in_air;
	}

	public void SetInWave (bool _is_in_wave) {
		is_in_wave = _is_in_wave;
	}

	public void SetOnGround (bool _is_on_ground) {
		is_on_ground = _is_on_ground;
	}

	void OnColliderEnter2D (Collision2D other) {
		if (other.gameObject.tag == "Obstacle") {
			Debug.Log ("Hit obstacle");
			//Not sure we'll kill him in either of these cases?
			//This may be a jump?
			//SetDead (true);
		}
	}

	void OnTriggerEnter2D (Collider2D other) 
	{
		if (other.gameObject.tag == "Wave") {
			Debug.Log ("Hit Wave");
			SetDead (true);
		}
		else if (other.gameObject.tag == "Air") {
			// Debug.Log ("Hit Air");
			SetInAir (true);
		}
		else if (other.gameObject.tag == "Ground") {
			// Debug.Log ("Hit Ground");
			SetOnGround (true);
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.gameObject.tag == "Air") {
			// if (other.relativeVelocity.y < 0.0f)
			{
				SetInAir (false);
			}
		} else if (other.gameObject.tag == "Ground") {
			// if (0.0f < other.relativeVelocity.y)
			{
				SetOnGround (false);
			}
		}
	}
}
