# 🎬 Referências de Movimento — Canicross

Frames extraídos dos vídeos de referência.

## Vídeos de Referência (já baixados)

| Arquivo | Origem | Conteúdo |
|---------|--------|----------|
| `canicross_video.mp4` | YouTube Shorts | Campeonato regional com Mia |
| `canicross_reel.mp4` | Instagram Reel | @juleskennel — cinematográfico |

## Vídeos dos Personagens (a baixar)

| Arquivo | Origem | Conteúdo |
|---------|--------|----------|
| `adao_superacao_cover.jpg` | Instagram Highlight | Capa do destaque "Canicross" |
| *(a baixar)* | [@adao.superacao Highlights](https://www.instagram.com/stories/highlights/18259015006201085/) | Fotos/vídeos do Adão e Alexandre |

## Posições-Chave para Modelagem 3D

### Cão (Adão — Malinois)
| Pose | Uso | Prioridade |
|------|-----|-----------|
| Standing perfil | Proporções base do modelo | 🔴 Alta |
| Running full stride | Animação de corrida | 🔴 Alta |
| Mid-jump | Animação de salto | 🟡 Média |
| Looking back | Animação de check-in | 🟡 Média |
| Head close-up | Expressão/máscara facial | 🟢 Baixa |
| Tail up / tail down | Rig da cauda | 🟢 Baixa |

### Corredor (Alexandre)
| Pose | Uso | Prioridade |
|------|-----|-----------|
| Running (perfil) | Proporções + animação | 🔴 Alta |
| Holding tether | Posição das mãos/corda | 🔴 Alta |
| Starting stance | Animação de largada | 🟡 Média |
| Braking/leaning back | Animação de freio | 🟡 Média |
| Full body standing | Referência de proporções | 🟢 Baixa |

## Notas de Animação
- O movimento do cão é mais "elástico" (saltitante) — Malinois são ágeis
- O corredor tem passada mais longa e constante
- A corda azul `#00BFFF` deve ter física simulada (LineRenderer + sway)
- Sincronização cão+corredor no salto: cão salta primeiro, corredor segue
