# 🐕 Canicross — Unity Project Setup

## Requisitos
- **Unity 6** (6000.3.0+) com módulo **WebGL**
- **Git** (para clonar)

## Setup Inicial

### 1. Abrir o projeto
1. Abra o Unity Hub
2. Clique em **Add** → Selecione a pasta `Canicross/`
3. Unity irá baixar os pacotes automaticamente (URP, etc.)
4. Aguarde a importação completa

### 2. Configurar URP
```bash
# No Unity Editor:
1. Window → Rendering → Render Pipeline Converter
2. Selecione "Built-in to URP"
3. Clique em "Initialize and Convert"
4. Delete a pasta "URP Settings" default (se criada duplicada)
```

### 3. Configurar WebGL
```bash
1. File → Build Profiles
2. Selecione "Web" como plataforma ativa
3. Clique em "Switch Platform"
4. Player Settings → Resolution → 1280x720 (default)
```

### 4. Criar a Cena Inicial
```bash
1. Crie um GameObject vazio → nomeie "GameManager"
   - Adicione o script: _Game/Scripts/Core/GameManager.cs
2. Crie um GameObject → nomeie "Dog"
   - Tag: "Dog"
   - Adicione: Rigidbody, DogController.cs
3. Crie um GameObject → nomeie "Runner"
   - Tag: "Runner"
   - Adicione: CharacterController, RunnerController.cs
4. Crie um GameObject → nomeie "Tether"
   - Filho de "Dog"
   - Adicione: LineRenderer, TetherSystem.cs
5. Crie um GameObject → nomeie "HUD"
   - Adicione: Canvas, HUDController.cs, StaminaBar.cs, TimerDisplay.cs
6. Crie um GameObject → nomeie "CameraRig"
   - Adicione: Camera, POVCamera.cs
```

### 5. Testar
```bash
1. Dê Play
2. Controles:
   - WASD / Setas: movimento
   - Espaço: pular
   - E: check-in com o cão
   - Esc: pausar
```

## Estrutura
```
Canicross/
├── Assets/
│   ├── _Game/
│   │   ├── Scenes/       # Cenas do jogo
│   │   ├── Prefabs/      # Prefabs (cão, corredor, obstáculos)
│   │   ├── Scripts/      # C# scripts (12 já criados)
│   │   │   ├── Core/     # GameManager, RaceManager
│   │   │   ├── Player/   # Dog, Runner, Tether, Bond, Input
│   │   │   ├── Systems/  # Stamina, Speed
│   │   │   ├── Camera/   # POV Camera
│   │   │   ├── Environment/ # Track, Checkpoint, Obstacle
│   │   │   └── UI/       # HUD, StaminaBar, Timer
│   │   ├── Materials/    # Materiais (vazio — criar no Unity)
│   │   └── Shaders/      # ToonLit.shader
│   ├── Art/              # Modelos, texturas, UI assets
│   └── Audio/            # SFX e música
├── Packages/
│   └── manifest.json     # URP + dependências
├── ProjectSettings/      # Configurações Unity
└── GDD.md                # Documento de design
```

## Scripts Implementados (12)

| Script | Função |
|--------|--------|
| `GameManager.cs` | Singleton, estados (Menu/Countdown/Racing/Finished) |
| `RaceManager.cs` | Timer, checkpoints, recordes (PlayerPrefs) |
| `DogController.cs` | Movimento do cão: speed, boost, jump, steering |
| `RunnerController.cs` | Corredor segue cão via tether elástico |
| `TetherSystem.cs` | LineRenderer azul da corda |
| `BondSystem.cs` | Sistema de vínculo, check-in a cada ~30s |
| `InputHandler.cs` | Mapeamento de input → ações |
| `StaminaSystem.cs` | Stamina dupla (cão + corredor) com eventos |
| `SpeedSystem.cs` | Cálculo e smoothing de velocidade km/h |
| `POVCamera.cs` | Câmera POV, FOV dinâmico, sway, tilt |
| `HUDController.cs` | Orquestrador do HUD (stamina, tempo, velocidade, bond) |
| `StaminaBar.cs` | Barra de stamina individual com cor dinâmica |
| `TimerDisplay.cs` | Display de tempo mm:ss.ff |
| `TrackManager.cs` | Gerenciador de trilha (posições start/finish) |
| `CheckpointTrigger.cs` | Trigger de checkpoint e linha de chegada |
| `Obstacle.cs` | Tipos de obstáculo: raiz, lama, água, pedra, galho |
| `ToonLit.shader` | Shader low-poly com rampa de luz/sombra + fresnel |
