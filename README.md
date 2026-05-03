# 🐕 Canicross — Repositorio de Desenvolvimento

Jogo de Canicross cinematografico com estetica low-poly. 
Unity 6 + URP + WebGL.

## Estrutura

```
cani-game/
├── GDD.md                  # Documento de design completo
├── Canicross/              # Projeto Unity
│   ├── Assets/_Game/       # Scripts, Shaders, Prefabs, Materiais
│   │   ├── Scripts/
│   │   │   ├── Core/       # GameManager, RaceManager, GameSetupWizard
│   │   │   ├── Player/     # DogController, RunnerController, TetherSystem, CharacterBuilder, BondSystem
│   │   │   ├── Camera/     # POVCamera
│   │   │   ├── Systems/    # StaminaSystem, SpeedSystem
│   │   │   ├── Environment/# TrackGenerator, TrackManager, Obstacle, CheckpointTrigger
│   │   │   ├── UI/         # HUDController, MainMenuController, StaminaBar, TimerDisplay
│   │   │   └── Editor/     # AutoTester, DogPrefabBuilder, GameSetupWizard, MenuSceneBuilder
│   │   ├── Materials/      # Dog_Brown, Tether_Blue, etc.
│   │   ├── Shaders/        # ToonLit
│   │   └── Scenes/         # MainMenu, Track_01_Lago (geradas via Editor)
│   └── Packages/           # URP + dependencias
├── Characters/             # Modelagem 3D
│   ├── Adao_Dog/           # Cao Adao (Malinois)
│   └── Alexandre_Runner/   # Corredor Alexandre
└── scripts/                # Scripts utilitarios
```

## Setup Rapido

1. Abra o projeto no Unity 6
2. Menu: `Canicross > Setup Game Scene` — configura a cena de jogo
3. Menu: `Canicross > Create Menu Scene` — cria a cena do menu
4. Play!

Ou via linha de comando:
```
scripts\setup_game.bat
scripts\run_unity_test.bat
```

## Personagens

| Personagem | Conta | Raca/Descricao |
|-----------|-------|----------------|
| Adao | [@adao.superacao](https://www.instagram.com/stories/highlights/18259015006201085/) | Cao Malinois |
| Alexandre | @adao.superacao | Corredor humano |

## Referencias

- YouTube Shorts: [Campeonato regional com Mia](https://youtube.com/shorts/yAVK8g4SIig)
- Instagram Reel: [@juleskennel](https://www.instagram.com/reel/DU6EEwqjNoV/)
