using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    private int count;

    private float movementX;
    private float movementY;

    public float speed = 0;

    public TextMeshProUGUI countText;

    public GameObject winTextObject;

    // --- VARIABLES DE AUDIO ---
    public AudioClip pickupSound;
    public AudioClip deathSound;
    public AudioClip winSound;       // Nuevo: Sonido de victoria
    public AudioClip collisionSound; // Nuevo: Sonido al chocar con paredes/objetos

    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        count = 0;

        SetCountText();

        winTextObject.SetActive(false);
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        rb.AddForce(movement * speed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            other.gameObject.SetActive(false);

            count = count + 1;

            SetCountText();

            // Sonido al recoger un cubo
            if (audioSource != null && pickupSound != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();

        if (count >= 12)
        {
            // 1. Detiene todos los audios activos (música de fondo, etc.)
            AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (AudioSource audio in allAudioSources)
            {
                audio.Stop();
            }

            // 2. Reproduce el sonido de victoria
            if (winSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(winSound);
            }

            winTextObject.SetActive(true);

            Destroy(GameObject.FindGameObjectWithTag("Enemy"));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Si colisiona con el enemigo -> Derrota
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Apaga todos los audios
            AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (AudioSource audio in allAudioSources)
            {
                audio.Stop();
            }

            // Reproduce el sonido de muerte
            if (deathSound != null)
            {
                AudioSource.PlayClipAtPoint(deathSound, transform.position);
            }

            Destroy(gameObject);

            winTextObject.gameObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
        }
        // Si colisiona con cualquier otro objeto (paredes, obstáculos, etc.)
        else
        {
            if (collisionSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collisionSound);
            }
        }
    }
}