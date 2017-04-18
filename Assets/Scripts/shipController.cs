using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class shipController : MonoBehaviour {

	public float rocketPower = 1.0f;
	public Text fuelText;
	public Text horzSpeed;
	public Text vertSpeed;
	public Text altText;
	public Text scoreText;
	public Text endScreenLine1;
	public Text endScreenLine2;
    public float maxHeight = 70f;
	[HideInInspector]
	public float fuelLevel = 1000.0f;
    [HideInInspector]
    public int bonusFactor = 1;
	[HideInInspector]
	public bool touchDown = false;
    [SerializeField]
	private float fuelLossRate = 1f;
	private float move;
	private Vector3 forceDirection;
	private Rigidbody rigidBody;
	private float horizontalSpeed = 0f;
	private float verticalSpeed = 0f;
	private float altitude = 0f;
	private Vector3 velocity;
	//private float originalZ;
	private int score = 0;
	private GameObject endScreen;
    private bool isAltHigh;
    private bool isFuelLow;

    private float initialpositionX;
    private float initialpositionY;
    private float initialpositionZ;
    private float initialRotationX;
    private float initialRotationY;
    private float initialRotationZ;

    // varaibles for Explosion prefab initialization
    [SerializeField] private GameObject explosionPrefab;
    private GameObject _explosion;

    // SOUNDS!!!!
    private AudioSource LandingSound;
    private AudioSource YouWonAudio;
    GameObject YouLandedMusic;
    GameObject LowFuelSoundObject;
    private AudioSource LowFuelAudio;

    // mesh renderer component to enable and disable
    MeshRenderer shipMesh;

    // -----------------------------------
	// Use this for initialization
	void Start () {
        LowFuelSoundObject = GameObject.Find("LowFuelSF");
        LowFuelAudio = LowFuelSoundObject.GetComponent<AudioSource>();
        YouLandedMusic = GameObject.Find("Mars_Menu_Motif");
        YouWonAudio = YouLandedMusic.GetComponent<AudioSource>();
        LandingSound = GetComponent<AudioSource>();
		rigidBody = GetComponent<Rigidbody> ();
		fuelText.text = " Fuel: 1000";
        initialpositionX = transform.position.x;
        initialpositionY = transform.position.y;
        initialpositionZ = transform.position.z;
        initialRotationX = transform.eulerAngles.x;
        initialRotationY = transform.eulerAngles.y;
        initialRotationZ = transform.eulerAngles.z;
        shipMesh = GetComponent<MeshRenderer>();
		endScreen = GameObject.Find ("CrashOrLand");
        isAltHigh = false;
        isFuelLow = false;
        //originalZ = transform.position.z;


        // Audio For landing sequence
        
	}
	
	// Update is called once per frame
	void Update () {
		scoreText.text = " Score: " + score.ToString ();
		if (!touchDown) {
			UpdateHUD ();
		}
	}

	void FixedUpdate(){
		move = Input.GetAxis ("Vertical");
		transform.Rotate (move, 0f, 0f);
		#if UNITY_EDITOR 
		if ((Input.GetKey (KeyCode.Space) || 
			Input.GetAxis (ControllerMap.Startup.inputAxis) == ControllerMap.Startup.triggerDown) &&
			fuelLevel > 0f && transform.position.y <= maxHeight && touchDown == false)   
		#else
		if((Input.GetKey (KeyCode.Space) || Input.GetAxis("rockets_win") == 1) && 
		fuelLevel > 0f)   //build only
		#endif
		{
			forceDirection = rocketPower * transform.up;
			rigidBody.AddForce (forceDirection, ForceMode.Acceleration);
			fuelLevel -= fuelLossRate;

			fuelText.text = " Fuel: " + fuelLevel.ToString ();
            

			if (fuelLevel < 0f) {
				fuelLevel = 0f;
			}


		}
	}

	void LateUpdate(){
		if (transform.position.z < 0) {
			transform.position = new Vector3 (transform.position.x, transform.position.y, 500);
		}

		if (transform.position.z > 500) {
			transform.position = new Vector3 (transform.position.x, transform.position.y, 0);
		}
	}
			
	void UpdateHUD(){
		velocity = rigidBody.GetPointVelocity (Vector3.zero);
		horizontalSpeed = -velocity.z;
		verticalSpeed = -velocity.y;
		altitude = rigidBody.transform.position.y;
		horzSpeed.text = " Horizontal Speed: " + horizontalSpeed.ToString ("F2");
		vertSpeed.text = " Vertical Speed: " + verticalSpeed.ToString ("F2");
		if (altitude > 70) {
            if (isAltHigh == false)
                { LowFuelAudio.Play();
                isAltHigh = true;
                }
			altText.text = " Altitude: " + altitude.ToString ("F2") + "\n TO HIGH!!!  CUTTING ENGINES!!";
			altText.color = new Color (1f, 0f, 0f, 1f);
		} else {
			altText.text = " Altitude: " + altitude.ToString ("F2");
			altText.color = new Color (1f, 1f, 1f, 1f);
		}
		if (fuelLevel < 100){
            if (isFuelLow == false)
            {
                LowFuelAudio.Play(); //play warning audio
                isFuelLow = true;
            }

			fuelText.text = " Fuel: " + Mathf.Ceil(fuelLevel).ToString ()+"\n LOW FUEL!!!!";
			fuelText.color = new Color(1f, 0f, 0f, 1f);
		}
		else{
			fuelText.text = " Fuel: " + Mathf.Ceil(fuelLevel).ToString ();
		}

        // added this for warning sound effect
        if (altitude < 70)
        {
            isAltHigh = false;
        }
	}



	void OnCollisionEnter(Collision other) {
		float angle = transform.rotation.eulerAngles.x;
		float xnormal = other.contacts [0].normal.x; 
		float znormal = other.contacts [0].normal.z; 
		int lostFuel;
		if (angle > 180) {
			angle = 360 - angle;
		}
		if (other.gameObject.tag == "Surface"  && !touchDown) {
			touchDown = true;
			if ((Mathf.Abs (horizontalSpeed) < 4f) && (Mathf.Abs (verticalSpeed) < 4f) &&
				(angle < 15f) && (xnormal < 0.4f) && (znormal < 0.4f)) {
//				Debug.Log ("Perfect Landing");
				score += 100*bonusFactor;
				Time.timeScale = 0f;
				endScreenLine1.text = "Congratulations!\nA perfect landing!";
				endScreenLine2.text = (100*bonusFactor).ToString() + " points.";
                LandingSound.Play();
                YouWonAudio.Play();
                StartCoroutine(ResetPerfectLandingShip());
            } else {
//				Debug.Log ("Kaboom!");
//				Debug.Log ("horizontal speed: " + horizontalSpeed.ToString ());
//				Debug.Log ("vertical speed: " + verticalSpeed.ToString ());
//				Debug.Log ("rotation: " + angle.ToString());
//				Debug.Log ("other normal: " + other.contacts[0].normal.ToString());
				lostFuel = (int) Mathf.Ceil(Random.Range (fuelLevel / 4, fuelLevel / 3));
				fuelLevel -= lostFuel;
                _explosion = Instantiate(explosionPrefab) as GameObject;
                _explosion.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
				if (fuelLevel <= 0.0f) {
					endScreenLine1.text = "Out of Fuel.";
					endScreenLine2.text = "Game Over.";
					StartCoroutine (LoadMenu ());
				} else {
					string[] quip = {
						"You created a 2 mile crater!",
						"A Slitheen must have taught you to drive!",
						"Wubbalubbadubdub!  Rick Sanchez at the helm!"
					};
					int i = Random.Range (0, 3);
					endScreenLine1.text = "Auxiliary fuel tanks destroyed.";
					endScreenLine2.text = lostFuel.ToString () + " fuel units lost.\n" + quip [i];
                    StartCoroutine(ResetShip());
				}
			}
			endScreen.GetComponent<Canvas>().enabled = true;
			
		}
	}

	bool bonus(float target, float tolerance) {
		if (Mathf.Abs(transform.position.z-target) < tolerance) {
			return true;
		} else {
			return false;
		}
	}

    IEnumerator ResetShip()
    {
        rigidBody.velocity = Vector3.zero;
        enabled = false;
        shipMesh.enabled = false;
		Time.timeScale = 1f;
		Camera.main.GetComponent<cameraController>().resetCamera();
        yield return new WaitForSeconds(5);
		touchDown = false;
        shipMesh.enabled = true;
        enabled = true;
        rigidBody.constraints = RigidbodyConstraints.None;
        transform.position = new Vector3(initialpositionX, initialpositionY, initialpositionZ);
        transform.eulerAngles = new Vector3(initialRotationX, initialRotationY, initialRotationZ);
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
		endScreen.GetComponent<Canvas>().enabled = false;

    }
    IEnumerator ResetPerfectLandingShip()
    {
        rigidBody.velocity = Vector3.zero;
        Time.timeScale = 1f;
        Camera.main.GetComponent<cameraController>().resetCamera();
        yield return new WaitForSeconds(5);
        touchDown = false;
        shipMesh.enabled = true;
        enabled = true;
        rigidBody.constraints = RigidbodyConstraints.None;
        transform.position = new Vector3(initialpositionX, initialpositionY, initialpositionZ);
        transform.eulerAngles = new Vector3(initialRotationX, initialRotationY, initialRotationZ);
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        endScreen.GetComponent<Canvas>().enabled = false;

    }

    IEnumerator LoadMenu()
	{
		rigidBody.velocity = Vector3.zero;
		enabled = false;
		shipMesh.enabled = false;
		yield return new WaitForSeconds(3);
		SceneManager.LoadScene (1);
	}
		
}
