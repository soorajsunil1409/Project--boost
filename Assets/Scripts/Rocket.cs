using UnityEngine;
using UnityEngine.SceneManagement; 

public class Rocket : MonoBehaviour
{
    // todo fix lighting bug

    [SerializeField] float mainThrust = 100f;
    [SerializeField] float rcsThrust = 200f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    Rigidbody rigidbody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending };
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }


    void OnCollisionEnter (Collision collision)
    {
        if (state != State.Alive) { return; }

        switch (collision.gameObject.tag)
        {
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

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextScene", 2f);
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadFirstLevel", 2f);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1); // todo allow for more than 2 levels
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void RespondToThrustInput()
    {
        rigidbody.freezeRotation = true;

        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }

        rigidbody.freezeRotation = false;
    }

    private void ApplyThrust()
    {
        rigidbody.AddRelativeForce(Vector3.up * mainThrust);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput()
    {
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
    }


}
