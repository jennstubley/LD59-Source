using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public List<Sprite> IconsForType;

    public static GameController Instance;
    public List<GameObject> Levels;
    public int CurrentLevelIndex = 0;
    public string LevelFailureReason = "";

    [SerializeField] private GameObject PlayButton;
    [SerializeField] private GameObject LevelCompleteScreen;
    [SerializeField] private GameObject InstructionPanel;
    [SerializeField] private GameObject WinPanel;
    [SerializeField] private AudioSource TrainSounds;

    private List<Train> trains = new List<Train>();
    private List<Station> stations = new List<Station>();

    public bool IsPlaying { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null) Instance = this;
        trains = Levels[CurrentLevelIndex].GetComponentsInChildren<Train>().ToList();
        stations = Levels[CurrentLevelIndex].GetComponentsInChildren<Station>().ToList();
        int i = 0;
        foreach (GameObject level in Levels)
        {
            level.SetActive(i == CurrentLevelIndex);
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        PlayButton.GetComponentInChildren<TMP_Text>().text = IsPlaying ? "Reset" : "Start";
        if (IsPlaying)
        {
            CheckCompleteLevel();
            bool anyTrainRunning = false;
            foreach (Train train in trains)
            {
                if (train != null && train.IsRunning())
                {
                    anyTrainRunning = true;
                    break;
                }
            }
            if (!anyTrainRunning && TrainSounds.isPlaying)
            {
                Debug.Log("No trains running, stopping train sounds.");
                TrainSounds.Stop();
            }
        }
    }

    public void PlayOrReset()
    {
        if (LevelCompleteScreen.activeSelf) return;
        IsPlaying = !IsPlaying;
        if (IsPlaying)
        {
            GetComponent<AudioController>().PlayRoundStart();
            TrainSounds.PlayDelayed(1.1f);
        }
        else
        {
            TrainSounds.Stop();
        }

        foreach (Train train in trains)
        {
            if (IsPlaying) train.StartRunning();
            else train.ResetTrain();
        }
        foreach (Station station in stations)
        {
            if (!IsPlaying) station.ResetStation();
        }
    }

    public bool CheckCompleteLevel()
    {
        foreach (Station station in stations)
        {
            if (!station.IsOccupied)
            {
                return false;
            }
        }

        if (CurrentLevelIndex == Levels.Count - 1)
        {
            WinPanel.SetActive(true);
            return true;
        }
        LevelCompleteScreen.SetActive(true);
        return true;
    }

    public void NextLevel()
    {
        if (CurrentLevelIndex < Levels.Count - 1)
        {
            CurrentLevelIndex++;
            Levels[CurrentLevelIndex - 1].SetActive(false);
            Levels[CurrentLevelIndex].SetActive(true);
            trains = Levels[CurrentLevelIndex].GetComponentsInChildren<Train>().ToList();
            stations = Levels[CurrentLevelIndex].GetComponentsInChildren<Station>().ToList();
            LevelCompleteScreen.SetActive(false);
            PlayOrReset();
        }
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowInstructions()
    {
        InstructionPanel.SetActive(true);
    }

    public void HideInstructions()
    {
        InstructionPanel.SetActive(false);
    }
}
