# P02-TDV

Este trabalho foi desenvolvido no âmbito da unidade curricular de Técnicas de Desenvolvimento de Videojogos (lecionada por Daniel Nogueira).
O objetivo principal desta proposta é desenvolver um jogo linear de mundo aberto utilizando o Monogame. 

Este projeto foi desenvolvido por:
- Paulo Bastos - 27945
- Bruno Mesquita - 27947
- Bento Simões - 27914

Grande parte do source-code deste projeto foi obtido através da sample para jogos PlatFormer elaborado pelos desenvolvedores e contribuídores do MonoGame (ver referências). 

Nesta documentação iremos abordar as principais classes, recursos e implementações que tornaram o nosso projeto possível.
Iremos tratar dos seguintes tópicos, na ordem em que estão apresentados:
- Pasta Content
- Pasta GameObjects
- Pasta TextHandler
- State.cs
- Main.cs

# Pasta `Content`

A pasta `Content` do projeto contém vários ativos e recursos essenciais para o desenvolvimento do jogo. Num projeto MonoGame, esta pasta inclui todos os arquivos de conteúdo, como texturas, sons, fontes e outros recursos que o jogo utilizará. Abaixo está a estrutura e a descrição dos conteúdos principais desta pasta:

1. **Texturas e Sprites**: 
   - Imagens utilizadas para personagens do jogo, fundos e outros elementos visuais.
   - Formatos comuns: `.png`, `.jpg`.

2. **Arquivos de Áudio**: 
   - Efeitos sonoros e músicas.
   - Formatos comuns: `.wav`, `.mp3`.

3. **Fontes**: 
   - Fontes personalizadas usadas no jogo.
   - Formatos comuns: `.spritefont`, `.ttf`.

4. **Níveis**
   - Níveis processados pela engine e pela template.
   - Formatos comuns: `.txt`.  

5. **Configuração da Ferramenta de Pipeline**: 
   - Arquivo `Content.mgcb` utilizado pela ferramenta MonoGame Content Pipeline para processar e gerenciar os ativos do jogo.

Cada subpasta dentro de `Content` agrupa ativos semelhantes para manter a organização do projeto. Por exemplo:
- A pasta "Tutorials" contém todos os "slides" do tutorial.
- A pasta "Sounds" contém todos os arquivos de áudio.

Esta estrutura permite uma fácil navegação e gerenciamento dos recursos necessários para o desenvolvimento do jogo.


# Pasta `GameObjects`

A pasta `GameObjects` contém classes e componentes essenciais para os objetos do jogo. Abaixo está uma descrição detalhada dos conteúdos principais desta pasta:

## Subpastas

### Enemy
- **Propósito**: Contém classes e scripts relacionados aos inimigos do jogo.
- **Descrição**: Inclui a lógica de movimento, ataque e comportamento geral de todos os inimigos.

### Tiles
- **Propósito**: Contém classes e scripts para a gestão de tiles no jogo.
- **Descrição**: Inclui a configuração dos tiles, a renderização e interações (neste caso foi providenciado pela template oficial do MonoGame).

## Outras classes

### Bullet.cs
- **Propósito**: Define o comportamento dos projéteis no jogo.
- **Descrição**: Inclui propriedades como velocidade, direção e deteção de colisão dos projéteis.

### GameObject.cs
- **Propósito**: Classe base para todos os objetos do jogo.
- **Descrição**: Fornece propriedades e métodos comuns a todos os objetos do jogo, como posição, desenho e atualização.

### Player.cs
- **Propósito**: Define o comportamento do jogador.
- **Descrição**: Inclui lógica de movimento, controlo do jogador, interações com outros objetos e habilidades.

Cada subpasta e arquivo dentro de `GameObjects` é responsável por uma parte específica da lógica e dos componentes do jogo, garantindo uma estrutura organizada e modular.


# Pasta `TextHandler`

A pasta `TextHandler` contém classes responsáveis pela gestão e exibição de textos no jogo. Abaixo está uma descrição detalhada dos arquivos principais desta pasta:

## Outras Classes

### DialogueEntity.cs
- **Propósito**: Define uma entidade de diálogo.
- **Descrição**: Inclui propriedades e métodos para armazenar e manipular os dados de diálogos, como falas dos personagens.

### DialogueManager.cs
- **Propósito**: Gerencia os diálogos do jogo.
- **Descrição**: Controla a exibição de diálogos, transições entre falas e interação com o jogador.

### MessageLog.cs
- **Propósito**: Registra e exibe mensagens do jogo.
- **Descrição**: Armazena um histórico de mensagens e fornece funcionalidades para adicionar e renderizar essas mensagens na tela.

### TextButton.cs
- **Propósito**: Cria e gerencia botões de texto.
- **Descrição**: Define a aparência e o comportamento dos botões de texto, incluindo interações do usuário e eventos de clique.

Cada arquivo dentro de `TextHandler` é essencial para a manipulação e exibição de textos, diálogos e mensagens no jogo, garantindo uma interface de usuário interativa e informativa.
Na versão atual do jogo, ainda não existem diálogos entre personagens. Atualmente a pasta apenas está a ser utilizada para os botões presentes no menu principal.


# `State.cs`

O arquivo `State.cs` contém a definição de uma classe base abstrata para representar os diferentes estados do jogo. Abaixo está uma descrição detalhada dos componentes principais desta classe:

## Classe `State`

### Propósito
- Servir como classe base para todos os estados do jogo, como menus, estados de jogo ativo, pausas, etc.

### Funções Principais
- **`Enter()`**: Definir o comportamento ao entrar em um estado.
- **`Exit()`**: Definir o comportamento ao sair de um estado.
- **`Update(GameTime gameTime)`**: Atualizar a lógica do estado a cada frame.
- **`Draw(GameTime gameTime, SpriteBatch spriteBatch)`**: Desenhar e representar visualmente o estado na tela.

### Propriedades
- Propriedades e funções comuns que serão herdadas e implementadas por classes derivadas específicas para cada estado do jogo.

Esta classe fornece uma estrutura organizada e padronizada para gerir os diferentes estados do jogo, facilitando a transição e manutenção dos mesmos.


# `Main.cs`

O arquivo `Main.cs` contém a implementação das funções necessárias para desenhar os diferentes estados do jogo e dar reset ao jogo e inimigos. Abaixo está uma descrição detalhada dos componentes principais desta função:

## Função Principal Draw

### Propósito
- Gerir a renderização dos diferentes estados do jogo e desenhar os elementos gráficos correspondentes a cada estado.

### Funções Principais
- **Tutorial**: Renderiza os tutoriais.
- **GameWin**: Renderiza a tela de vitória.
- **GameOver**: Renderiza uma sobreposição escura e mensagens de game over.
- **TitleScreen**: Renderiza a tela inicial e seus elementos interativos (botões).

### Propriedades
- `spriteBatch`: Usado para desenhar texturas e textos na tela.
- `tutorials`, `endgame`, `overlay`, `titlescreen`: Texturas usadas para desenhar os estados específicos.
- `font`: Fonte usada para desenhar textos na tela.
- `playButton`, `tutorialButton`, `quitButton`: Botões interativos na tela inicial.

## Função Principal Reset

### Propósito
- Reiniciar o jogo e os inimigos ao começar um novo jogo ou ao mudar de nível.

### Funções Principais
- **Reset()**: Reseta o estado do jogo e reposiciona o jogador e inimigos.
- **ResetEnemies()**: Reseta e posiciona os inimigos conforme o nível atual.
- **LoadNextLevel()**: Carrega o próximo nível do jogo.
- **ReloadCurrentLevel()**: Recarrega o nível atual.
- **LoadLevel(int levelDestination)**: Carrega um nível específico.

### Propriedades
- `gameObjects`: Lista de objetos do jogo, incluindo o jogador e inimigos.
- `gameSprite`: Textura usada para os sprites do jogo.
- `shotSound`: Efeito sonoro do tiro do jogador.
- `levelIndex`: Índice do nível atual.
- `numberOfLevels`: Número total de níveis no jogo.

Esta função principal fornece uma estrutura organizada e eficiente para desenhar e gerir os diferentes estados do jogo, assim como para resetar o jogo e os inimigos conforme necessário.

## Função Principal Update

### Propósito
- Atualizar a lógica do jogo a cada frame, garantindo a continuidade e responsividade da experiência de jogo.

### Funções Principais
- **Update(GameTime gameTime)**: Atualiza a lógica do jogo baseada no estado atual, entradas do jogador, e atualiza os objetos do jogo.

### Componentes
- **UpdateGameObjects(GameTime gameTime)**: Atualiza todos os objetos do jogo.
- **HandleCollisions()**: Detecta e gerencia colisões entre objetos do jogo.

### Propriedades
- `gameObjects`: Lista de objetos do jogo, incluindo jogador, inimigos e outros elementos.
- `keyboardState`: Estado atual do teclado, utilizado para capturar entradas do jogador.
- `gamePadState`: Estado atual do gamepad, utilizado para capturar entradas do jogador.
- `State.Instance.CurrentGameState`: Estado atual do jogo, que determina qual lógica deve ser executada (por exemplo, tela de título, jogando, game over).

Esta função principal garante que a lógica do jogo seja atualizada corretamente a cada frame, proporcionando uma experiência de jogo contínua e responsiva.

## Funções de Inicialização
- **Initialize()**: Inicializa os componentes do jogo, configurações e variáveis.
- **LoadContent()**: Carrega todo o conteúdo do jogo, incluindo texturas, fontes e sons.
- **UnloadContent()**: Descarrega o conteúdo não mais necessário.
- **Constructor (Main)**: Configurações iniciais do jogo e instancia objetos principais.

# Melhorias Futuras
O Jogo está longe de estar perfeito ou finalizado, existem muitos bugs e partes incompletas.
Reunimos abaixo uma lista de melhorias que poderiam ser implementadas no futuro, entre elas:
- Configurações (volume, resolução, sensibilidade etc.)..
- Leaderboard
- Mais níveis e mais complexos
- Novas habilidades
- Boss mais desafiante
- Sistema de diálogo

  
# Referências:
- [Monogame](https://monogame.net/)
- [Código Base](https://github.com/MonoGame/MonoGame.Samples/tree/3.8.1/Platformer2D)
- [1-Bit Pack](https://kenney-assets.itch.io/1-bit-pack)
- [Background Music](https://youtu.be/mRN_T6JkH-c?si=2VRXJ6iuM0hwj-us)
- [Efeitos Sonoros](https://opengameart.org/)
- [Super Dialogue Audio Pack](https://dillonbecker.itch.io/sdap)
- [16-Bit Sound Pack](https://jdwasabi.itch.io/8-bit-16-bit-sound-effects-pack)
