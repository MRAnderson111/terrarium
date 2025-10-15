using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPController : MonoBehaviour {

	public Slider healthBar;
	public GameObject cam;
	public Transform shotDirection;
	public Animator anim;
	public AudioSource [] footsteps;
	public GameObject deathCamera;
	public GameObject bloodPrefab;
	public ParticleSystem muzzleFlash;

	float speed = 0.1f;
	float xSensitivity = 2f;
	float ySensitivity = 2f;
	float MinimumX = -90;
	float MaximumX = 90;

	Rigidbody rb ;
	CapsuleCollider capsule;

	Quaternion cameraRotation;
	Quaternion characterRotation;

	bool cursorIsLocked = true;
	bool lockCursor = true;

	float x;
	float z;

	int health = 100;
	int maxHealth = 100;

	public void TakeHit(float amount)
	{
		health = (int) Mathf.Clamp(health - amount, 0 , maxHealth);
        healthBar.value = health;
		if(health<=0)
		{
			Vector3 pos = new Vector3(this.transform.position.x, Terrain.activeTerrain.SampleHeight(this.transform.position)+0.5f,this.transform.position.z);
			GameObject deathCam = Instantiate(deathCamera, pos, this.transform.rotation);
            GameStats.gameOver = true;
			Destroy(this.gameObject);
		}
	}

	void PlayMuzzleFlash()
	{
		if(muzzleFlash!= null)
		{
			muzzleFlash.Play();
		}
	}
	
	void Start ()
	{
		rb = this.GetComponent<Rigidbody>();
		capsule = this.GetComponent<CapsuleCollider>();

		cameraRotation = cam.transform.localRotation;
		characterRotation = this.transform.localRotation;

		health = maxHealth;
		healthBar.value = health;
	}

	void ProcessCritterHit()
	{
		RaycastHit hitInfo;
		if(Physics.Raycast(shotDirection.position, shotDirection.forward, out hitInfo, 200))
		{
			Debug.DrawRay(transform.position, shotDirection.forward,Color.green, 50, true);
			GameObject hitCritter = hitInfo.collider.gameObject;
			if(hitCritter.tag == "Critter")
			{
               GameObject blood = Instantiate(bloodPrefab, hitInfo.point, Quaternion.identity);
			   Destroy(blood, 0.5f);
			   hitCritter.GetComponent<AIController>().KillCritter();			
			}
		}
	}
		
	void Update () 
	{
		if (Input.GetMouseButtonDown(0) && !anim.GetBool("fire"))
		{
           anim.SetTrigger("fire");
            PlayMuzzleFlash();
		   ProcessCritterHit();
		}

       	if (Input.GetKeyDown(KeyCode.R))
		   {
           anim.SetTrigger("reload");
		   }

		if(Mathf.Abs(x) > 0 || Mathf.Abs(z) > 0)
		{
			if(!anim.GetBool("walking"))
			{
           		anim.SetBool("walking",true);
				InvokeRepeating("PlayFootStepAudio", 0,0.5f);
			}
		}
		else if (anim.GetBool("walking"))
		{
			anim.SetBool("walking", false);
			CancelInvoke("PlayFootStepAudio");
		}
	}

	void PlayFootStepAudio()
	{
		AudioSource audioSource = new AudioSource();
		int n = Random.Range(1, footsteps.Length);

		audioSource = footsteps[n];
		audioSource.Play();
		footsteps[n] = footsteps[0];
		footsteps[0] = audioSource;
	}

	void FixedUpdate()
	{
		float yRotation = Input.GetAxis("Mouse X")* ySensitivity;
		float xRotation = Input.GetAxis("Mouse Y")* xSensitivity;

		cameraRotation *= Quaternion.Euler(-xRotation ,0,0);
		characterRotation*= Quaternion.Euler(0,yRotation,0);

		cameraRotation = ClampRotationAroundXAxis(cameraRotation);

		this.transform.localRotation = characterRotation;
		cam.transform.localRotation = cameraRotation;

		if(Input.GetKeyDown(KeyCode.Space) && IsGrounded())
			rb.AddForce(0,300,0);
		
		x = Input.GetAxis("Horizontal") * speed;
		z = Input.GetAxis("Vertical") * speed;

		transform.position += cam.transform.forward * z + cam.transform.right * x;//new Vector3(x * speed, 0, z * speed);
		UpdateCursorLock();
	}

	Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;

		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
		angleX = Mathf.Clamp(angleX,MinimumX,MaximumX);
		q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

		return q;
	}

	bool IsGrounded()
	{
		RaycastHit hitInfo;
		if(Physics.SphereCast(transform.position,capsule.radius,Vector3.down, out hitInfo, (capsule.height/2) - capsule.radius + 0.1f))
		{
			return true;
		}
		return false ;
	}

	public void SetCursorLock(bool value)
	{
		lockCursor = value;
		if(!lockCursor)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	public void UpdateCursorLock()
	{
		if(lockCursor)
			InternalLockUpdate();
	}

	public void InternalLockUpdate()
	{
		if(Input.GetKeyUp(KeyCode.Escape))
			cursorIsLocked = false;
		else if (Input.GetMouseButtonUp(0))
			cursorIsLocked = true;

		if (cursorIsLocked)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		if(!cursorIsLocked)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}
}
