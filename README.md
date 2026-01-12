# Clash Arena - BGP-Studio

Projeto realizado por:
- Paulo Bastos 27945
- Bruno Mesquita 27947
- José Lima 27935

Realizado no âmbito da UC de Projeto Aplicado, lecionada e orientada pelo professor Duarte Duque.

## Descrição Geral

Este projeto é um FPS multiplayer desenvolvido em **Unity**, com suporte a jogadores humanos e bots inteligentes, integração de **Unity Netcode** para multiplayer seguro e sincronizado, e **Photon Chat** para comunicação textual global. O jogo combina combate com armas configuráveis, HUD completo, minimapa, indicadores de dano, menus intuitivos e música persistente.

O sistema é modular, permitindo adicionar armas, bots ou novos mapas sem alterar a arquitetura central. Todas as ações críticas são validadas no servidor para evitar inconsistências ou cheats, garantindo uma experiência multiplayer confiável.

---

## Funcionalidades Principais

* Multiplayer via **Unity Netcode** com sincronização de vida, kills, score, equipa, posição e ações do jogador.
* Inteligência Artificial de bots com detecção de inimigos, perseguição, disparo e recarregamento.
* Sistema de armas avançado com cadência de tiro, velocidade de projétil, magazine, recarregamento e efeitos visuais/sonoros.
* HUD dinâmico: munição, kills, score, crosshair dinâmico com kick, ADS e hitmarkers.
* Indicadores de dano direcionais e flash de dano.
* Sistema de respawn com contagem regressiva e teleport seguro para spawn.
* Minimap com opção de manter norte fixo e suavização de movimento.
* Menu principal, lobby, menus de configurações e créditos com persistência de PlayerPrefs.
* Música de menu persistente entre cenas.
* **Photon Chat** para comunicação textual global no lobby ou durante o jogo.

---

## Arquitetura e Sistemas

### Rede e Multiplayer

O sistema multiplayer baseia-se em **Unity Netcode**, usando:

* **NetworkVariables** para vida, mortes, kills, score e equipa. Variáveis replicadas automaticamente a todos os clientes.
* **ServerRpc** para enviar ações do cliente ao servidor (ex.: pedidos de respawn, troca de equipa, disparo).
* **ClientRpc** para feedback imediato do servidor para o cliente (ex.: teleporte, hitmarker, atualização de HUD).
* Validação de estado no servidor para prevenir cheats.
* Instanciação de projéteis e efeitos de armas centralizada no servidor.

O servidor controla tanto jogadores humanos quanto bots, garantindo que todas as ações estejam sincronizadas e que o score seja contabilizado corretamente.

### Inteligência Artificial (Bots)

* Bots possuem scripts próprios para detecção de inimigos, movimentação, disparo e recarga.
* Toda lógica de IA é processada no servidor.
* A posição, animações e ações dos bots são replicadas via **NetworkObject** para todos os clientes.
* O sistema permite adicionar novos tipos de bots com comportamento diferente sem alterar os scripts dos jogadores.

### Sistema de Jogador, Vida e Respawn

* O script `Health` gerencia vida e morte, disparando eventos quando o jogador morre.
* `PlayerDeathAndRespawn` controla respawn com delay configurável, teleport seguro, atualização de spawn points e verificação de colisões no solo.
* HUD é atualizado via eventos para refletir morte e respawn.
* Respawn pode ser forçado ignorando checks de vida quando necessário.
* Spawn points podem ser configurados manualmente ou descobertos automaticamente via tags.

### Armas e Combate

* `WeaponConfig` define estatísticas da arma: cadência, munição, velocidade de projétil, sons e efeitos visuais.
* `WeaponSwitchUIUpdater` detecta armas ativas e atualiza HUD de munição via reflexão (compatível com pacote Infima Games).
* Projéteis replicam dono e equipa para contabilizar dano corretamente.
* HUD atualiza dinamicamente munição, kills e score.
* Crosshair com kick, ADS e hitmarkers.

### HUD e UI

* HUD mostra score, kills, munição, crosshair, ADS, hitmarkers e damage indicators.
* `DamageIndicatorUI` mostra seta apontando a direção de ataque e flash na tela.
* `RespawnUIManager` mostra contagem regressiva e botão de respawn.
* `ScoreboardUI` exibe lista de jogadores com kills e score, atualizada periodicamente.
* `ScoreHUDBinder` vincula score e kills locais ao HUD do jogador.
* Menus incluem MainMenu, Lobby, Settings e Créditos com persistência de PlayerPrefs para resolução e sensibilidade do rato.
* `UIButtonHover` fornece feedback visual e sonoro ao interagir com botões.

### Photon Chat

* Integração com **Photon Chat** permite mensagens globais entre jogadores.
* Cada jogador autentica-se e subscreve-se a canais de chat.
* UI exibe mensagens com scroll automático e campo de escrita.

### Música e Som

* `MenuMusicPlayer` toca música de fundo em menus e persiste entre cenas.
* Sons de UI, disparo, recarregamento e efeitos de armas são reproduzidos localmente e replicados via networking se necessário.
* Hover e feedback de botão via `UIButtonHover`.

---

## Considerações Finais

O projeto combina múltiplos sistemas complexos com modularidade e replicação segura. A arquitetura garante que:

* Estado crítico é sincronizado e validado no servidor.
* Bots e jogadores humanos coexistem de forma consistente.
* HUD e UI refletem instantaneamente o estado do jogador.
* Configurações são persistentes e menus são intuitivos.
* Chat global permite comunicação independente do Netcode.

O código foi estruturado para facilitar **expansão futura**, incluindo novos modos de jogo, armas, mapas, tipos de bots e integração de novas funcionalidades de rede ou UI.
