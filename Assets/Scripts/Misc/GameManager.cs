using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject door;
    public GameObject restartText;
    public static GameManager instance;
    public AudioClip[] muisc;

    private AudioSource audioSource;
    private PlayerScript player;
    private int enemies;
    private int level = 0;
    private float playerHealth;
    private bool nextLevelReady = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    { 
        // Key Events
        if (Input.GetKeyDown("r") && player.getHealth() <= 0)
        {
            StartGame();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && nextLevelReady == true)
        {
            playerHealth = player.getHealth();
            SceneManager.LoadScene(level);
            nextLevelReady = false;

            audioSource.clip = muisc[level+1];
            audioSource.Play();
        }
    }

    public void ReduceEnemies()
    {
        enemies--;

        if (enemies <= 0 && nextLevelReady == false)
        {
            door.gameObject.SetActive(true);
            nextLevelReady = true;
            level++;
        }
    }

    public float getHealth()
    {
        if (level == 0)
        {
            return GameData.instance.playerHealth;
        }
        else
        {
            return playerHealth;
        } 
    }

    public void StartGame()
    {
        playerHealth = GameData.instance.playerHealth;
        SceneManager.LoadScene(0);

        door.gameObject.SetActive(false);
        nextLevelReady = false;
        level = 0;

        restartText.SetActive(false);

        // Music
        audioSource.clip = muisc[1];
        audioSource.Play();
    }

    public void setScene()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        door.gameObject.SetActive(false);
        nextLevelReady = false;
    }

    public void displayRestartText()
    {
        restartText.SetActive(true);
    }
}

