using UnityEngine;
using UnityEngine.SceneManagement;
using Canicross.Player;
using Canicross.Systems;
using Canicross.Environment;
using Canicross.UI;

namespace Canicross.Core
{
    public enum GameState
    {
        Menu,
        Countdown,
        Racing,
        Paused,
        Finished
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState CurrentState { get; private set; } = GameState.Menu;

        [SerializeField] private DogController dogController;
        [SerializeField] private RunnerController runnerController;
        [SerializeField] private StaminaSystem staminaSystem;
        [SerializeField] private BondSystem bondSystem;
        [SerializeField] private RaceManager raceManager;
        [SerializeField] private TrackManager trackManager;
        [SerializeField] private HUDController hudController;

        public DogController Dog => dogController;
        public RunnerController Runner => runnerController;
        public StaminaSystem Stamina => staminaSystem;
        public BondSystem Bond => bondSystem;
        public RaceManager Race => raceManager;
        public TrackManager Track => trackManager;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void StartCountdown()
        {
            CurrentState = GameState.Countdown;
            if (hudController == null) hudController = FindFirstObjectByType<HUDController>();
            hudController?.ShowCountdown(OnCountdownComplete);
        }

        private void OnCountdownComplete()
        {
            CurrentState = GameState.Racing;
            if (raceManager == null) raceManager = FindFirstObjectByType<RaceManager>();
            raceManager?.StartRace();
        }

        public void FinishRace()
        {
            CurrentState = GameState.Finished;
            if (raceManager == null) raceManager = FindFirstObjectByType<RaceManager>();
            raceManager?.StopRace();
            if (hudController == null) hudController = FindFirstObjectByType<HUDController>();
            hudController?.ShowResults(raceManager != null ? raceManager.FinalTime : 0f,
                raceManager != null ? raceManager.BestLap : 0f,
                bondSystem != null ? bondSystem.BondLevel : 0);
        }

        public void SetState(GameState newState)
        {
            CurrentState = newState;
        }

        public void LoadMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void LoadTrack(string trackName)
        {
            SceneManager.LoadScene(trackName);
        }

        private void Start()
        {
            SceneManager.activeSceneChanged += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene oldScene, Scene newScene)
        {
            if (newScene.name == "Track_01_Lago")
            {
                CurrentState = GameState.Menu;
                FindSceneReferences();
                if (raceManager != null)
                {
                    StartCountdown();
                }
            }
        }

        private void FindSceneReferences()
        {
            if (dogController == null) dogController = FindFirstObjectByType<DogController>();
            if (runnerController == null) runnerController = FindFirstObjectByType<RunnerController>();
            if (staminaSystem == null) staminaSystem = FindFirstObjectByType<StaminaSystem>();
            if (bondSystem == null) bondSystem = FindFirstObjectByType<BondSystem>();
            if (raceManager == null) raceManager = FindFirstObjectByType<RaceManager>();
            if (trackManager == null) trackManager = FindFirstObjectByType<TrackManager>();
            if (hudController == null) hudController = FindFirstObjectByType<HUDController>();
        }
    }
}