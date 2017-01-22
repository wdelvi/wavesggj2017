using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed_h = 0.0f;	// horizontal speed
	public float speed_h_in_wave = -3.0f;
	public float speed_h_in_air = -3.5f;
	public float speed_h_on_ground = -3.5f;
	public float speed_h_down_add = 5.0f;
	public float speed_h_down_add_force = 2.0f;
	public float speed_h_collide = -256.0f;
	public float speed_h_acceleration = 0.0f;
	public float speed_h_jerk_down = 4.0f;
	public float speed_h_jerk_up = -0.5f;
	public float deltaTime;

	public float speed_v_in_air = -0.25f; // -0.5f;  // -1.0f;
	public float speed_v_death = -256.0f;
	public float speed_wave = 5f;	// vertical speed of wave

	public float speed_press = 10f;	// vertical speed of pressing

	public bool is_input_enabled = true;
	public bool is_dead;			// player is dead, disable input

	public bool is_in_air;		// player at top
	public bool is_in_wave;		// player in middle
	public bool is_in_right_edge;
	public bool is_on_ground;		// player is on ground
	public bool is_collide_next;
	public bool is_collide_now;

	private float speed_v;			// vertical speed
	private Rigidbody2D rb;
	private Animator anim;

	public bool is_down;
	public bool is_down_before;

	public bool is_force = true;
	public float force_multiplier = 200.0f;

	public ParticleSystem particle;

	// Use this for initialization
	public void Setup() {
		is_dead = false;
		is_down = false;
		is_down_before = false;
		is_in_wave = true;
		is_in_air = false;
		is_on_ground = false;
		is_collide_next = false;
		is_collide_now = false;
		deltaTime = 0.0f;
		rb = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		particle.enableEmission = false;
	}

	private void UpdateAnimation() {
		if (is_down) {
			if (!anim.GetBool ("isDown")) {
				anim.SetBool ("isDown", true);
				particle.enableEmission = true;
			}
		} else {
			if (anim.GetBool ("isDown")) {
				anim.SetBool ("isDown", false);
				particle.enableEmission = false;
			}
		}
	}

	private void UpdateHorizontalSpeed() {
		if (is_in_air)
		{
			speed_h = speed_h_in_air;
			speed_h_acceleration = 0.0f;
		}
		else if (is_on_ground)
		{
			speed_h = speed_h_on_ground;
			speed_h_acceleration = 0.0f;
		}
		else if (is_collide_now)
		{
			speed_h = speed_h_on_ground;
			speed_h_acceleration += speed_h_jerk_up;
		}
		else if (is_in_right_edge)
		{
			speed_h_acceleration += speed_h_jerk_up;
		}
		else if (is_in_wave)
		{
			speed_h = speed_h_in_wave;
			if (is_down)
			{
				if (is_force)
				{
					speed_h += speed_h_down_add_force;
				}
				else
				{
					speed_h += speed_h_down_add;
				}
				if (is_down_before)
				{
					speed_h_acceleration += speed_h_jerk_down * deltaTime;
				}
				else
				{
					speed_h_acceleration = 0.0f;
				}
			}
			else
			{
				speed_h_acceleration += speed_h_jerk_up * deltaTime;
			}
		}
		speed_h += speed_h_acceleration;
	}

	// force down on press
	// If out of bounds, return to bounds.
	private void UpdateVerticalSpeed() {
		if (is_in_air) {
			speed_v = speed_v_in_air;
			if (is_down)
			{
				speed_v -= speed_press;
			}
		} else if (is_on_ground) {
			speed_v = speed_wave;
		} else if (is_in_wave && is_down) {
			speed_v = speed_wave - speed_press;
		} else if (is_in_wave) {
			speed_v = speed_wave;
		}
	}

	private void UpdateCollide() {
		is_collide_now = is_collide_next;
		is_collide_next = false;
		if (is_collide_now)
		{
			speed_h += speed_h_collide;
		}
	}

	// Update is called once per frame
	// If player is alive, input moves player down.
	// Always update if the mouse button or Space key was pressed.
	// So the game controller can know when button is pressed at title screen.
	public void UpdateFrame() {
		deltaTime = Time.deltaTime;
		if (!is_dead && is_input_enabled) {
			UpdateHorizontalSpeed ();
			UpdateVerticalSpeed ();
			UpdateCollide ();
			if (is_force) {
				rb.AddRelativeForce (Vector2.up * speed_v * deltaTime * force_multiplier);
				rb.AddRelativeForce (Vector2.right * speed_h * deltaTime * force_multiplier);
			} else {
				transform.Translate (Vector2.up * speed_v * deltaTime);
				transform.Translate (Vector2.right * speed_h * deltaTime);
			}
		} else if (is_dead) {
			rb.AddRelativeForce (Vector2.up * speed_v_death * deltaTime * force_multiplier);
		}
		is_down_before = is_down;
		UpdateAnimation ();
	}

	public void SetDead (bool _is_dead) {
		is_dead = _is_dead;
	}

	public void SetInAir (bool _is_in_air) {
		is_in_air = _is_in_air;
	}
	public void SetInRightSide (bool _is_in_right_edge) {
		is_in_right_edge = _is_in_right_edge;
	}

	public void SetInWave (bool _is_in_wave) {
		is_in_wave = _is_in_wave;
	}

	public void SetOnGround (bool _is_on_ground) {
		is_on_ground = _is_on_ground;
	}

	void OnCollisionEnter2D (Collision2D other) {
		if (other.gameObject.tag == "Obstacle" || other.gameObject.tag == "ObstacleTall") {
			Debug.Log ("Hit obstacle");
			is_collide_next = true;
		}
	}

	void OnTriggerEnter2D (Collider2D other) 
	{
		if (other.gameObject.tag == "Wave") {
			Debug.Log ("Trigger Wave");
			SetDead (true);
		}
		else if (other.gameObject.tag == "Obstacle") {
			Debug.Log ("Trigger obstacle");
			//Not sure we'll kill him in either of these cases?
			//This may be a jump?
			//SetDead (true);
		}
		else if (other.gameObject.tag == "Air") {
			// Debug.Log ("Trigger Air");
			SetInAir (true);
		}
		else if (other.gameObject.tag == "RightSide") {
			// Debug.Log ("Trigger Air");
			SetInRightSide (true);
		}
		else if (other.gameObject.tag == "Ground") {
			// Debug.Log ("Trigger Ground");
			SetOnGround (true);
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.gameObject.tag == "Air") {
			SetInAir (false);
		} else if (other.gameObject.tag == "Ground") {
			SetOnGround (false);
		} else if (other.gameObject.tag == "RightSide") {
			SetInRightSide (false);
		}
	}
}
