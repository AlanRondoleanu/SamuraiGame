using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Server Data
[System.Serializable]
public class DataBase
{
    public string time;
    public int ranged_use;
    public int melee_use;
    public int enemies_killed;
}

public class GameManager : MonoBehaviour
{
    public GameObject door;
    public static GameManager instance;
    public AudioClip[] muisc;
    public DataBase data;

    public TMP_Text timerText;
    public TMP_Text restartText;

    private AudioSource audioSource;
    private PlayerScript player;
    private int enemies;
    private int level = 0;
    private bool nextLevelReady = false;
    private bool sleep = false;
    private float gameTimer = 0;
    private bool gameOver = false;

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
            GameData.instance = GetComponent<GameData>();
        }

        PlayerPrefs.SetFloat("Health", GameData.instance.playerHealth);
        PlayerPrefs.SetInt("Stance", 1);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        string jsonData = JsonUtility.ToJson(data);
        //StartCoroutine(ClientServer.ResetData(jsonData));
        data.time = "";
        data.enemies_killed = 0;
        data.melee_use = 0;
        data.ranged_use = 0;
    }

    void Update()
    { 
        // Key Events
        if (Input.GetKeyDown("r") && player.getHealth() <= 0)
        {
            SceneManager.LoadScene(1);
            PrepareNewGame(1);
        }

        if (gameOver == false)
            gameTimer += Time.deltaTime;
            timerText.text = TimeToString(gameTimer);
            data.time = timerText.text;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && nextLevelReady == true)
        {
            PlayerPrefs.SetFloat("Health", player.getHealth());
            PlayerPrefs.SetInt("Stance", player.getStance());
            SceneManager.LoadScene(level);
            nextLevelReady = false;

            door.SetActive(false);
            audioSource.clip = muisc[level];
            audioSource.Play();
        }
    }

    /// <summary>
    /// comment
    /// </summary>
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

    public void PrepareNewGame(int t_level)
    {
        //SceneManager.LoadScene(4);

        door.SetActive(false);
        nextLevelReady = false;
        level = 1;

        PlayerPrefs.SetFloat("Health", GameData.instance.playerHealth);
        PlayerPrefs.SetInt("Stance", 1);

        restartText.gameObject.SetActive(false);

        // Music
        audioSource.clip = muisc[1];
        audioSource.Play();

        // Timer
        gameTimer = 0;
        timerText.gameObject.SetActive(true);
    }

    public void setScene()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        door.SetActive(false);
        nextLevelReady = false;
    }

    public void displayRestartText()
    {
        restartText.gameObject.SetActive(true);
    }

    public void Sleep()
    {
        if (sleep == true)
            return;

        Time.timeScale = 0.0f;
        StartCoroutine(Wait(0.05f));
    }

    public void Pause()
    {
        Time.timeScale = 0.0f;
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;
    }

    IEnumerator Wait(float t_duration)
    {
        sleep = true;

        yield return new WaitForSecondsRealtime(t_duration);

        Time.timeScale = 1.0f;
        sleep = false;
    }

    public void StopTimer()
    {
        gameOver = true;
        //returnToMenuText.gameObject.SetActive(true);
    }

    string TimeToString(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        int milliseconds = (int)((time * 1000) % 1000);
        return minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + milliseconds.ToString("00");
    }

    public void ManualRestart()
    {
        SceneManager.LoadScene(1);
        PrepareNewGame(1);
    }
}

