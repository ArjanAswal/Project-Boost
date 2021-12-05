using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] float rcsThrust = 250f;
    [SerializeField] float mainThrust = 250f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;
    Rigidbody rigidBody;
    AudioSource audioSource;

    bool collisionsDisabled = false;

    // Start is called before the first frame update
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        if (state == State.Alive) {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if (Debug.isDebugBuild) { RespondToDebugKeys(); }
    }

    private void RespondToDebugKeys() {
        if (Input.GetKeyDown(KeyCode.L)) {
            LoadNextLevel();
        } else if (Input.GetKeyDown(KeyCode.C)) {
            collisionsDisabled = !collisionsDisabled;
        }
    }

    private void OnCollisionEnter(Collision collision) {

        if (state != State.Alive || collisionsDisabled) {
            return;
        }

        switch (collision.gameObject.tag) {
            case "Friendly":
                break;

            case "Finish":
                StartSuccessSequence();
                break;

            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartDeathSequence() {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void StartSuccessSequence() {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void LoadFirstLevel() {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel() {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void RespondToThrustInput() {
        if (Input.GetKey(KeyCode.Space)) {
            ApplyThrust();
        } else {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust() {
        float rotationThisFrame = mainThrust * Time.deltaTime;

        rigidBody.AddRelativeForce(Vector3.up * rotationThisFrame);
        if (!audioSource.isPlaying) {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput() {
        rigidBody.angularVelocity = Vector3.zero;
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A)) {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        } else if (Input.GetKey(KeyCode.D)) {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
    }

}