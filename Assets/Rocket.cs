using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {
    [SerializeField] float rcsThrust = 250f;
    [SerializeField] float mainThrust = 250f;


    Rigidbody rigidBody;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        Thrust();
        Rotate();
    }

    private void Thrust() {
        if (Input.GetKey(KeyCode.Space)) {
            float rotationThisFrame = mainThrust * Time.deltaTime;

            rigidBody.AddRelativeForce(Vector3.up * rotationThisFrame);
            if (!audioSource.isPlaying) {
                audioSource.Play();
            }
        } else {
            audioSource.Stop();
        }
    }

    private void Rotate() {

        rigidBody.freezeRotation = true;
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A)) {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        } else if (Input.GetKey(KeyCode.D)) {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false;

    }

}


