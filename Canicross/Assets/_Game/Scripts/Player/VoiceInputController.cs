using UnityEngine;
using System.Runtime.InteropServices;
using Canicross.Core;

namespace Canicross.Player
{
    public class VoiceInputController : MonoBehaviour
    {
        [Header("Voice Commands (Portuguese)")]
        [SerializeField] private string[] rightCommands = { "direita", "right", "vira direita", "pra direita" };
        [SerializeField] private string[] leftCommands = { "esquerda", "left", "vira esquerda", "pra esquerda" };
        [SerializeField] private string[] boostCommands = { "vai", "acelera", "boost", "corre" };
        [SerializeField] private string[] jumpCommands = { "pula", "jump", "pulo", "pular" };
        [SerializeField] private string[] brakeCommands = { "para", "stop", "freia", "freia", "devagar" };
        [SerializeField] private string[] checkInCommands = { "aqui", "check", "bom garoto", "bom menino" };

        [Header("Settings")]
        [SerializeField] private float commandHoldDuration = 0.5f;
        [SerializeField] private bool showDebugLogs = true;

        [Header("UI")]
        [SerializeField] private UnityEngine.UI.Text statusText;

        private DogController dogController;
        private BondSystem bondSystem;

        private float lateralInput;
        private float brakeInput;
        private bool boostInput;
        private bool jumpInput;
        private float commandTimer;

        private bool isListening;

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern int VoiceControl_Init(string callbackObjectName, string callbackMethodName);

        [DllImport("__Internal")]
        private static extern int VoiceControl_Start();

        [DllImport("__Internal")]
        private static extern int VoiceControl_Stop();

        [DllImport("__Internal")]
        private static extern int VoiceControl_IsSupported();
#endif

        private void Awake()
        {
            dogController = FindFirstObjectByType<DogController>();
            bondSystem = FindFirstObjectByType<BondSystem>();
        }

        private void Start()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            int supported = VoiceControl_IsSupported();
            if (supported == 1)
            {
                VoiceControl_Init(gameObject.name, "OnVoiceCommand");
                StartListening();
            }
            else
            {
                UpdateStatus("Voz nao suportada neste navegador");
            }
#else
            UpdateStatus("Comandos de voz apenas em WebGL");
#endif
        }

        private void OnDestroy()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            VoiceControl_Stop();
#endif
        }

        public void StartListening()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            int result = VoiceControl_Start();
            isListening = result == 1;
            UpdateStatus(isListening ? "Ouvindo... (fale 'direita' ou 'esquerda')" : "Erro ao iniciar microfone");
#endif
        }

        public void StopListening()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            VoiceControl_Stop();
            isListening = false;
            UpdateStatus("Microfone desligado");
#endif
        }

        public void ToggleListening()
        {
            if (isListening)
                StopListening();
            else
                StartListening();
        }

        public void OnVoiceCommand(string command)
        {
            if (showDebugLogs)
                Debug.Log("[VoiceInput] Command: " + command);

            UpdateStatus("Comando: " + command);

            if (MatchCommand(command, rightCommands))
            {
                lateralInput = 1f;
                commandTimer = commandHoldDuration;
            }
            else if (MatchCommand(command, leftCommands))
            {
                lateralInput = -1f;
                commandTimer = commandHoldDuration;
            }
            else if (MatchCommand(command, boostCommands))
            {
                boostInput = true;
                commandTimer = commandHoldDuration;
            }
            else if (MatchCommand(command, jumpCommands))
            {
                jumpInput = true;
                commandTimer = 0.1f;
            }
            else if (MatchCommand(command, brakeCommands))
            {
                brakeInput = 1f;
                commandTimer = commandHoldDuration;
            }
            else if (MatchCommand(command, checkInCommands))
            {
                bondSystem?.RespondToCheckIn();
            }
        }

        private bool MatchCommand(string input, string[] commands)
        {
            foreach (var cmd in commands)
            {
                if (input.Contains(cmd))
                    return true;
            }
            return false;
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != Core.GameState.Racing)
                return;

            // Decay voice inputs
            if (commandTimer > 0)
            {
                commandTimer -= Time.deltaTime;
            }
            else
            {
                lateralInput = 0f;
                brakeInput = 0f;
                boostInput = false;
            }

            if (jumpInput && commandTimer <= 0)
            {
                jumpInput = false;
            }

            // Apply to dog controller (combined with keyboard input)
            float keyboardLateral = Input.GetAxis("Horizontal");
            float keyboardBrake = Input.GetAxis("Vertical") < 0 ? Mathf.Abs(Input.GetAxis("Vertical")) : 0f;
            bool keyboardBoost = Input.GetAxis("Vertical") > 0.1f;
            bool keyboardJump = Input.GetKeyDown(KeyCode.Space);

            float combinedLateral = Mathf.Abs(keyboardLateral) > 0.1f ? keyboardLateral : lateralInput;
            float combinedBrake = Mathf.Max(keyboardBrake, brakeInput);
            bool combinedBoost = keyboardBoost || boostInput;
            bool combinedJump = keyboardJump || jumpInput;

            dogController?.SetInput(combinedLateral, combinedBrake, combinedBoost, combinedJump);

            if (Input.GetKeyDown(KeyCode.E))
            {
                bondSystem?.RespondToCheckIn();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameManager.Instance != null)
                {
                    if (GameManager.Instance.CurrentState == Core.GameState.Racing)
                        GameManager.Instance.SetState(Core.GameState.Paused);
                    else if (GameManager.Instance.CurrentState == Core.GameState.Paused)
                        GameManager.Instance.SetState(Core.GameState.Racing);
                }
            }
        }

        private void UpdateStatus(string message)
        {
            if (statusText != null)
                statusText.text = message;
        }

        public void SetTouchInput(float lateral)
        {
            lateralInput = lateral;
            commandTimer = 0.1f;
        }

        public bool IsListening => isListening;
    }
}
