using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sizeIncrease = 0.1f;
    public float sizeDecreaseFactor = 0.5f;
    private Vector2 movement;

    public GameObject doorPrefab;
    private GameObject doorInstance;
    private int totalFoodCount = 0;
    private int collectedFoodCount = 0;

    public GameObject PausePanel;
    public GameObject SuccessPanel;
    public GameObject GameOverPanel;

    void Start()
    {
        doorInstance = Instantiate(doorPrefab, new Vector2(0, 0), Quaternion.identity);
        doorInstance.SetActive(false);
    }

    void Update()
    {
        movement.x = Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0;
        movement.y = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;

        Vector3 newPosition = transform.position + (Vector3)movement * moveSpeed * Time.deltaTime;

        Camera cam = Camera.main;
        float halfWidth = cam.orthographicSize * cam.aspect;
        float halfHeight = cam.orthographicSize;

        newPosition.x = Mathf.Clamp(newPosition.x, -halfWidth, halfWidth);
        newPosition.y = Mathf.Clamp(newPosition.y, -halfHeight, halfHeight);

        transform.position = newPosition;

        CheckForRemainingObjects();

        if (Input.GetKeyDown(KeyCode.P))
        {
            PausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food"))
        {
            transform.localScale += new Vector3(sizeIncrease, sizeIncrease, 0);
            Destroy(other.gameObject);
            collectedFoodCount++;
            CheckForRemainingObjects();
        }
        else if (other.CompareTag("Laxative"))
        {
            transform.localScale *= sizeDecreaseFactor;
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Enemy"))
        {
            float mySize = transform.localScale.x;
            float enemySize = other.transform.localScale.x;

            if (mySize > enemySize)
            {
                transform.localScale += new Vector3(0.1f, 0.1f, 0);
                Destroy(other.gameObject);
            }
            else if (mySize < enemySize)
            {
                other.transform.localScale += new Vector3(0.1f, 0.1f, 0);
                Destroy(gameObject);

                GameOverPanel.SetActive(true);
                Time.timeScale = 0f;
            }

            CheckForRemainingObjects();
        }
        else if (other.CompareTag("Door"))
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (currentSceneIndex == 3)
            {
                Time.timeScale = 0f;
                SuccessPanel.SetActive(true);
            }
            else
            {
                SceneManager.LoadScene(currentSceneIndex + 1);
            }
        }
    }

    public void AddToTotalFoodCount()
    {
        totalFoodCount++;
    }

    private void CheckForRemainingObjects()
    {
        GameObject[] foodObjects = GameObject.FindGameObjectsWithTag("Food");
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

        if (collectedFoodCount >= totalFoodCount && foodObjects.Length == 0 && enemyObjects.Length == 0)
        {
            doorInstance.SetActive(true);
        }
    }

    public void mainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void Resume()
    {
        PausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level1");
    }
    public void PlayAgain()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Level1");
    }
}
