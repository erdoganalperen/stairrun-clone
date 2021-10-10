using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject startText;
    [SerializeField]private GameObject nextLevelText;
    private bool gameStarted = false;
    private bool nextLevel = false;
    public PlayerController _playerController;
    public LevelManager LevelManager;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!gameStarted)
            {
                gameStarted = true;
                startText.SetActive(false);
                _playerController.speedZ = 10;
                _playerController.speedY = 10;
                _playerController.playerState = PlayerState.Running;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (nextLevel)
            {
                nextLevelText.SetActive(false);
                startText.SetActive(true);
                gameStarted = false;
                if (LevelManager.NextLevel())
                {
                    FindObjectOfType<PlayerController>().Reset();
                }
            }
        }
    }

   public void NextLevel()
    {
        nextLevelText.SetActive(true);
        nextLevel = true;
    }
}
