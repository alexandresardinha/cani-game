# 🐕 Canicross — Game Design Document

> Projeto: Jogo de Canicross com estética cinematográfica
> Engine: Unity (URP) · Target: WebGL · Perspectiva: POV do corredor
> Data: Maio 2026

---

## 1. Visão Geral

### 1.1 Conceito
Um jogo de canicross **experiencial e cinematográfico** — não apenas corrida arcade, mas uma experiência imersiva que combina velocidade, vínculo com o cão e paisagens deslumbrantes. O jogador controla um corredor sendo puxado por seu cão por trilhas que alternam entre beira d'água, floresta densa e campos abertos dourados.

### 1.2 Referências
- **YouTube Shorts:** [Campeonato regional com Mia](https://youtube.com/shorts/yAVK8g4SIig) — Ação competitiva, POV, linha de partida "DEPART", trilha de floresta com luz filtrada, clareiras abertas
- **Instagram Reel:** [@juleskennel](https://www.instagram.com/reel/DU6EEwqjNoV/) — Cinematográfico, lente grande angular, trilha à beira d'água, cachorro olhando pra trás, transições de luz dramáticas

### 1.3 Tom & Estilo
- **Ritmo variável:** rápido → calmo → rápido (como o Reel)
- **Vínculo** com o cão, não apenas velocidade
- **Paisagem como protagonista** — a trilha é a estrela
- **Luz dourada**, sombras longas, contraste alto

---

## 2. Análise Visual dos Vídeos de Referência

### 2.1 YouTube Shorts (87s, 360x640)
| Frame | Descrição | Implicação |
|-------|-----------|------------|
| 00 | Linha de partida, placa "DEPART", vários corredores, grama amarelada, céu azul | **Largada de campeonato** — atmosfera competitiva |
| 01–03 | Floresta fechada, trilha de terra, folhas, luz manchada pelo dossel | **Core da trilha** — sombra e luz dançante |
| 03 | Cachorro pulando obstáculo | **Mecânica de salto** |
| 04 | Clareira aberta, campo verde, árvore solitária, céu azul intenso | **Transição de ambiente** — respiro |
| 05 | Área estruturada, cerca de metal, asfalto com sombras | **Chegada / zona de transição** |

### 2.2 Instagram Reel (51s, 720x1280)
| Frame | Descrição | Implicação |
|-------|-----------|------------|
| 00 | Trilha à beira de lago, luz âmbar, banco de madeira, árvores inclinadas | **Abertura serena** — passeio, não corrida |
| 01–03 | Trilha continua, água visível, texto "juleskennel" no canto | **POV imersivo** — mãos + corda visíveis |
| 04–06 | Vegetação exuberante, luz solar forte | **Densidade visual** — natureza viva |
| 07 | ⭐ Cachorro **parou e olhou pra trás**! | **Mecânica de vínculo** — check-in emocional |
| 08–09 | Floresta → campo dourado aberto, túnel escuro à frente | **Transição dramática** — luz vs sombra |

### 2.3 Elementos Visuais Extraídos
| Elemento | Descrição | Uso no Jogo |
|----------|-----------|-------------|
| Corda azul | Fina, azul vibrante, diagonal na tela | LineRenderer visível, cor `#00BFFF` |
| Lente grande angular | Distorção nas bordas, sensação de velocidade | FOV 90-100°, leve barrel distortion |
| Texto overlay | Branco puro, fino, canto superior direito | HUD estilo "juleskennel" |
| Banco de madeira | Elemento de parque que humaniza | Props de ambientação |
| Água à direita | Lago/rio acompanhando a trilha | Reflexos, perigo lateral |
| Alternância luz/sombra | Trechos escuros e claros se alternam | Ritmo de gameplay |

### 2.4 Paleta de Cores (Extraída Programaticamente)

**YouTube Shorts:**
- `#202020` — Sombras profundas (24-54% dos pixels)
- `#606640` — Verde floresta médio
- `#96A076` — Verde dourado claro (luz filtrada)
- `#5A524D` — Marrom terra/trilha
- `#E0E0D4` — Luz solar / branco
- `#FFFFFF` — Overlay de texto

**Instagram Reel:**
- `#000000` — Preto puro (19.1% — contraste alto, edição cinematográfica)
- `#202020` + `#404020` — Sombras e terra
- `#1E90FF` → `#60C0E0` → `#80C0E0` — Degradê de azul céu
- `#228B22` — Verde folhagem viva
- `#DAA520` — Dourado/âmbar (luz do sol baixo)
- `#F5F5F5` — Branco overlay (texto "juleskennel")

---

## 3. Paleta de Cores Definitiva do Jogo

```
┌─────────────────────────────────────────────────┐
│ #1E90FF  ████████████  Azul corda (ícone)       │
│ #60C0E0  ████████████  Azul céu                 │
│ #228B22  ████████████  Verde folhagem viva      │
│ #006400  ████████████  Verde sombra profunda    │
│ #406020  ████████████  Verde musgo / trilha     │
│ #DAA520  ████████████  Dourado âmbar (god rays) │
│ #8B4513  ████████████  Marrom terra             │
│ #1C1C1C  ████████████  Preto (pelagem do cão)   │
│ #202020  ████████████  Sombra profunda          │
│ #F5F5F5  ████████████  Branco overlay (HUD)     │
└─────────────────────────────────────────────────┘
```

---

## 4. Gameplay

### 4.1 Core Loop
```
Largada → Corre pela trilha → Gerencia stamina → Desvia obstáculos 
→ Check-in com o cão → Chega ao fim → Melhor tempo → Repete
```

### 4.2 Controles
| Input | Ação | Feedback Visual |
|-------|------|-----------------|
| `A` / `D` ou `←` `→` | Direcionar o cão | Cão inclina, rastro na terra |
| `W` ou `↑` | "Vai!" (boost de velocidade) | Motion lines nas bordas, overlay "VAI!" |
| `Espaço` | Pular obstáculo | Cão + corredor saltam, câmera treme ao pousar |
| `S` ou `↓` | Frear / segurar | Poeira levanta, cão olha pra trás |
| `Espaço` (contextual) | **Check-in** com o cão | +Bond, +Stamina, cão animado |

### 4.3 Sistema de Stamina Dupla
| Barra | Drena ao... | Regenera... | Local no HUD |
|-------|-------------|-------------|--------------|
| ⚡ Cão | Usar boost (`W`) | Rápido (em ritmo normal) | Topo esquerda |
| 💨 Corredor | Correr (sempre, mais rápido = mais drena) | Lento | Topo direita |

### 4.4 Mecânica de Vínculo (Bond)
- Inspirada no **reel_f07**: o cão olha pra trás
- A cada ~30s, o cão faz um "check-in visual"
- Se o jogador pressiona `Espaço` nesse momento: **+10 Bond, +20% stamina do cão**
- Bond alto = cão responde mais rápido aos comandos, faz check-in mais frequente
- Bond baixo = cão hesita, ignora comandos com chance baixa

### 4.5 Tipos de Terreno
| Superfície | Efeito | Visual |
|------------|--------|--------|
| Terra batida | Normal | Marrom escuro, textura lisa |
| Lama | Reduz velocidade 30% | Marrom brilhante, pegadas |
| Folhas secas | Escorregadio (drift) | Laranja/marrom, partículas |
| Raízes | Obstáculo — pular! | Marrom 3D cruzando a trilha |
| Água (beira do lago) | Perigo — caiu = penalidade | Azul com reflexo |

### 4.6 Obstáculos
- **Raízes expostas** — pular (`Espaço`)
- **Galhos baixos** — o cão desvia sozinho, mas corredor precisa abaixar? (complexidade extra)
- **Poças de lama** — desviar ou enfrentar lentidão
- **Pedras soltas** — pequeno stumble, perde velocidade
- **Curvas fechadas** — drifting nas folhas

---

## 5. Design de Trilhas

### 5.1 Estrutura Geral
Três trilhas fixas, progressão de dificuldade.

### 5.2 Track 01 — "Lago do Parque" (Iniciante)
**Duração alvo:** ~2 minutos
**Ritmo:**

| Tempo | Ambiente | Iluminação | Ritmo | Evento |
|-------|----------|------------|-------|--------|
| 0:00–0:15 | Beira do lago, banco de madeira | Luz âmbar, sol baixo | Lento | Intro, câmera panorâmica |
| 0:15–0:40 | Trilha de terra, água à direita | Luz entre árvores | Acelerando | Primeiros obstáculos (raízes) |
| 0:40–0:55 | Clareira, campo dourado | Céu azul aberto | Rápido | Velocidade máxima, boost livre |
| 0:55–1:05 | Cachorro check-in | Sombra suave | Pausa | Interação bond |
| 1:05–1:35 | Túnel de árvores | Escuro, manchas de luz | Intenso | Baixa visibilidade, confiar no cão |
| 1:35–2:00 | Beira do lago — chegada | Luz dourada, água brilha | Sprint final | Linha de chegada, reflexos |

### 5.3 Track 02 — "Floresta do Vale" (Intermediário)
Mais sombra, mais obstáculos, curvas técnicas. Duração: ~3 min.

### 5.4 Track 03 — "Campeonato Regional" (Avançado)
A mais longa e intensa. Mistura todos os biomas. Inclui trecho de subida.

---

## 6. Câmera

### 6.1 Configuração Base
```
Tipo:      Terceira pessoa (atrás do corredor) — POV imersivo
Posição:   z=-3m do corredor, y=+1.6m (altura dos olhos)
Look-at:   Corredor + offset pra baixo (ver o cão)
FOV:       90° (grande angular, sensação de velocidade)
Sway:      Balanço lateral suave (0.3m amplitude)
Tilt:      Inclinação pra frente ao acelerar (+5°)
Tremor:    Ao pousar de salto (0.1m amplitude, 0.2s duração)
```

### 6.2 Efeitos de Pós-Processamento
- **Motion blur** nas bordas quando em boost (estilo Reel)
- **Vignette** sutil (20% bordas)
- **Leve barrel distortion** (simula lente grande angular)
- **Bloom** nos raios de sol e reflexos da água
- **Color grading:** alto contraste, blacks levemente crushed

---

## 7. Interface (HUD)

### 7.1 Layout
```
┌──────────────────────────────────┐
│ ⚡ 85%           MIA        01:23 │  ← Topo: stamina + nome + tempo
│                                  │
│          🌲  🌲  🐕             │
│        🌲    ╱                   │
│            ╱     🏃              │
│           ╱   🌲    🌲          │
│          🌲         🌲          │
│ ~~~~ água ~~~~                  │  ← Ambiente visível
│                                  │
│  ▼ 14 km/h          🔗 Bond 72  │  ← Base: velocidade + bond
└──────────────────────────────────┘
```

### 7.2 Elementos do HUD
| Elemento | Posição | Estilo | Inspiração |
|----------|---------|--------|------------|
| Stamina cão ⚡ | Topo esquerda | Barra fina azul | Reels |
| Nome do cão | Topo centro | Texto branco fino | "juleskennel" |
| Tempo | Topo direita | Monospace, branco | Cronômetro |
| Velocidade | Base esquerda | "▼ 14 km/h" | Overlay Reel |
| Bond 🔗 | Base direita | Ícone + número | Original |
| "VAI!" | Centro (temporário) | Grande, branco, fade | Boost |

### 7.3 Fontes
- Principal: **Inter** (sans-serif, limpa, moderna) — WebGL-friendly
- Monospace (tempo): **JetBrains Mono** ou **Roboto Mono**

---

## 8. Personagens

### 8.1 Cão (Mia — Malinois)
- **Modelo:** Low poly, pelagem preta (#1C1C1C)
- **Tamanho:** ~0.6m altura no ombro
- **Posição:** ~2.5m à frente do corredor (corda esticada)
- **Animações:** Correr, saltar, olhar pra trás (check-in), abanar rabo, latir
- **Comportamento:** Segue input do jogador + steering autônomo para desviar de micro-obstáculos

### 8.2 Corredor (Primeira Pessoa Implícita)
- **Não modelado diretamente** (POV)
- **Mãos visíveis** segurando a corda (opcional, versão futura)
- **Sombra no chão** e passos audíveis confirmam presença

---

## 9. Arquitetura Técnica

### 9.1 Stack
```
Engine:         Unity 2023 LTS ou superior
Render Pipeline: URP (Universal Render Pipeline)
Target:         WebGL 2.0
Linguagem:      C# (.NET Standard 2.1)
Física:         Unity Physics (PhysX)
```

### 9.2 Estrutura de Diretórios
```
Assets/
├── _Game/
│   ├── Scenes/
│   │   ├── Boot.unity              # Inicialização, loading WebGL
│   │   ├── MainMenu.unity          # Tela inicial
│   │   ├── Track_01_Lago.unity     # Trilha 1: Lago do Parque
│   │   ├── Track_02_Floresta.unity # Trilha 2: Floresta do Vale
│   │   └── Track_03_Campeonato.unity # Trilha 3: Campeonato
│   ├── Prefabs/
│   │   ├── Dog.prefab              # Cão com Rigidbody + SpringJoint
│   │   ├── Runner.prefab           # Corredor (CharacterController)
│   │   ├── Tether.prefab           # Corda (LineRenderer)
│   │   ├── TrackSegment.prefab     # Segmento de trilha
│   │   ├── Checkpoint.prefab       # Gate de checkpoint
│   │   └── Obstacle_*.prefab       # Variantes de obstáculos
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── GameManager.cs      # Singleton, estado global
│   │   │   ├── TrackManager.cs     # Controle da trilha, checkpoints
│   │   │   └── RaceManager.cs      # Largada, chegada, timer
│   │   ├── Player/
│   │   │   ├── DogController.cs    # Input → movimento do cão
│   │   │   ├── RunnerController.cs # Posição do corredor relativa ao cão
│   │   │   ├── TetherSystem.cs     # Física da corda (SpringJoint)
│   │   │   └── BondSystem.cs       # Sistema de vínculo/check-in
│   │   ├── Systems/
│   │   │   ├── StaminaSystem.cs    # Stamina dupla (cão + corredor)
│   │   │   └── SpeedSystem.cs      # Cálculo de velocidade km/h
│   │   ├── Camera/
│   │   │   └── POVCamera.cs        # Câmera atrás do corredor, sway, FOV
│   │   ├── Environment/
│   │   │   ├── TrackGenerator.cs   # Montagem de trilha fixa
│   │   │   └── SegmentData.cs      # Dados de cada segmento
│   │   └── UI/
│   │       ├── HUDController.cs    # Controle geral do HUD
│   │       ├── StaminaBar.cs       # Barra de stamina individual
│   │       └── TimerDisplay.cs     # Display de tempo
│   ├── Materials/
│   │   ├── Ground_Forest.mat
│   │   ├── Ground_Mud.mat
│   │   ├── Foliage_Green.mat
│   │   ├── Foliage_Shadow.mat
│   │   ├── Sky_Blue.mat
│   │   ├── Water_Lake.mat
│   │   ├── Tether_Blue.mat         # Corda azul
│   │   └── Overlay_White.mat       # HUD
│   └── Shaders/
│       └── ToonLit.shader           # Flat shading low poly
├── Art/
│   ├── Models/                      # Low poly .fbx (Blender)
│   │   ├── Dog_Malinois.fbx
│   │   ├── Tree_Oak.fbx
│   │   ├── Tree_Pine.fbx
│   │   ├── Rock_Small.fbx
│   │   ├── Bench_Park.fbx
│   │   ├── Sign_Wooden.fbx
│   │   └── Checkpoint_Gate.fbx
│   ├── Textures/
│   │   └── Terrain_Atlas.png
│   └── UI/
│       ├── Sprites/
│       └── Fonts/
├── Audio/
│   ├── SFX/
│   │   ├── Footsteps_Dirt.wav
│   │   ├── Footsteps_Leaves.wav
│   │   ├── Dog_Bark.wav
│   │   ├── Dog_Pant.wav
│   │   ├── Breathing.wav
│   │   ├── Jump.wav
│   │   └── Splash.wav
│   └── Music/
│       ├── Track01_Calm.ogg
│       ├── Track01_Fast.ogg
│       └── Victory.ogg
└── Settings/
    └── URP_Settings.asset
```

### 9.3 Diagrama de Dependências
```
GameManager
  ├── TrackManager
  │     ├── RaceManager
  │     └── TrackGenerator
  ├── DogController ── Input System
  │     └── TetherSystem ── RunnerController
  ├── StaminaSystem
  │     ├── DogController (stamina do cão)
  │     └── RunnerController (stamina do corredor)
  ├── BondSystem
  │     └── DogController (check-in)
  ├── POVCamera
  │     └── RunnerController (posição)
  └── HUDController
        ├── StaminaBar
        ├── TimerDisplay
        └── BondSystem (display)
```

---

## 10. Física

### 10.1 Sistema de Tração (Tether)
```
Cão:           Rigidbody + SpringJoint (ponto A)
Corredor:      CharacterController (ponto B)
Corda:         LineRenderer entre A e B
SpringJoint:   Spring = 50, Damper = 10, MaxDistance = 3.0
```

### 10.2 Movimento
- **Cão:** `Rigidbody.AddForce()` na direção do input + steering
- **Corredor:** Posição = segue o cão via `SpringJoint`, mas com `CharacterController.Move()` para controle fino
- **Velocidade máxima:** 25 km/h (boost), 15 km/h (normal), 8 km/h (lama)

### 10.3 Obstáculos
- **Raiz/Pedra:** Collider como trigger — se não pular, stumble (breve pausa)
- **Lama:** Collider que aplica `speedMultiplier = 0.7`
- **Água:** Collider de borda — se pisar, reset para último checkpoint

---

## 11. Áudio

### 11.1 Sound Design
| Evento | Som |
|--------|-----|
| Passos (terra) | Loop, varia com velocidade |
| Passos (folhas) | Loop, mais crunch |
| Cão latindo | Random a cada 15-30s |
| Respiração | Loop, intensidade proporcional à stamina |
| Boost "Vai!" | Whoosh + latido |
| Salto | Whoosh curto, impacto ao pousar |
| Check-in | Som suave de "pling" + latido feliz |
| Checkpoint | Sino curto |
| Chegada | Música de vitória, crowd (opcional) |

### 11.2 Música
- **Track 01:** Calma/ambiente → acelera → épica no final
- **Menu:** Lo-fi suave

---

## 12. Roadmap de Desenvolvimento

### Fase 1 — Protótipo (MVP)
- [x] GDD escrito
- [ ] Setup Unity + WebGL + URP
- [ ] Cena Track_01 com geometria básica
- [ ] DogController + RunnerController + Tether
- [ ] POVCamera funcional
- [ ] HUD básico (stamina, tempo, velocidade)
- [ ] Primeiro obstáculo (raiz)
- [ ] Build WebGL funcional

### Fase 2 — Polimento Core
- [ ] Sistema de stamina completo
- [ ] Sistema de bond/check-in
- [ ] Múltiplos tipos de terreno
- [ ] Vários obstáculos
- [ ] Transições de ambiente (escuro/claro)
- [ ] Áudio básico

### Fase 3 — Conteúdo
- [ ] Track 02 e Track 03
- [ ] Modelos low poly (árvores, cão, props)
- [ ] Leaderboard local
- [ ] Menu principal
- [ ] Efeitos de pós-processamento

### Fase 4 — Lançamento
- [ ] Otimização WebGL
- [ ] Leaderboard online (opcional)
- [ ] Localização PT/EN
- [ ] Publicação Itch.io

---

## 13. Referências Visuais Externas

| Jogo/Filme | O que roubar |
|------------|-------------|
| **Lonely Mountains: Downhill** | Low poly natureza, iluminação, sensação de velocidade |
| **A Short Hike** | Cores aconchegantes, atmosfera relaxante |
| **Alto's Adventure** | Transições de luz/ambiente, fluidez |
| **Steep** | Câmera POV de esportes, motion blur |

---

## 14. Notas Técnicas WebGL

### 14.1 Limitações
- **Memória:** Navegadores limitam a ~2GB (atenção a texturas grandes)
- **Multithreading:** C# Jobs não funcionam em WebGL; manter single-threaded
- **Asset Bundles:** Usar Addressables para carregamento progressivo (opcional)

### 14.2 Otimizações Planejadas
- Draw call batching com SRP Batcher (URP)
- LODs para árvores distantes
- Fog para esconder pop-in
- Texturas atlas (combinação de texturas pequenas)
- Compressão de áudio (Ogg Vorbis)
- Occlusion culling

---

*Documento vivo — atualizado conforme o desenvolvimento avança.*
