using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.PyroParticles{
	public class makeFire : MonoBehaviour {
		private bool isPlaying = false;
		private Transform[] originalTransforms;
		private shipController lander;
        private AudioSource audioFire;
		// Use this for initialization
		void Start () {
            audioFire = GetComponent<AudioSource>(); 
			originalTransforms = gameObject.GetComponentsInChildren<Transform> ();
			lander = transform.parent.GetComponent<shipController> ();
		}

		// Update is called once per frame
		void Update () {
#if UNITY_EDITOR
            if ((Input.GetKey(KeyCode.Space) ||
                Input.GetAxis(ControllerMap.Startup.inputAxis) == ControllerMap.Startup.triggerDown)
                && lander.fuelLevel > 0 && lander.transform.position.y < lander.maxHeight && !lander.touchDown)
#else
			if((Input.GetKey (KeyCode.Space) || Input.GetAxis("rockets_win") == 1))   //build only
#endif

                
			{
				//Debug.Log ("Space pressed.  isPlaying:" + isPlaying.ToString());
				if (!isPlaying) {
					foreach (ParticleSystem p in gameObject.GetComponentsInChildren<ParticleSystem>()) {
						p.Play ();
						isPlaying = true;
                        audioFire.volume = 20;
                        audioFire.Play();
					}
				}
				foreach (Transform t in gameObject.GetComponentsInChildren<Transform>()) {
					if (t.localScale.x < 2) {
						t.localScale += new Vector3 (.04f, .04f, .04f);
					}
				}
			}
			#if UNITY_EDITOR 
			if ((Input.GetKeyUp (KeyCode.Space) ||
				Input.GetAxis (ControllerMap.Startup.inputAxis) == ControllerMap.Startup.triggerUp)
				&& !Input.GetKey(KeyCode.Space) || lander.fuelLevel <= 0 || 
				lander.transform.position.y >= lander.maxHeight || lander.touchDown )
#else
			if((Input.GetKey (KeyCode.Space) || Input.GetAxis("rockets_win") == 1)
			&& !Input.GetKey(KeyCode.Space))   //build only
#endif
                
			{
				foreach (ParticleSystem p in gameObject.GetComponentsInChildren<ParticleSystem>()) {
					p.Stop ();
				}
                for (int i = 0; i < originalTransforms.Length; i++)
                {
                    originalTransforms[i].localScale = new Vector3(0.1f, 0.1f, 0.1f);
                }
                fadeOut();
                isPlaying = false;
			}
              
		}


        

        
        void fadeOut()
        {
            audioFire.volume -= 2 * Time.deltaTime;
            if (audioFire.volume <= 0)
            {
                audioFire.Stop();
            }
        }
	}
}

