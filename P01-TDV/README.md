# MonoSnake

Este trabalho foi realizado pelos seguintes elementos:
- Paulo Bastos / 27945
- Bruno Mesquita / 27947
- Bento Simões / 27914

Esta documentação é referente ao clássico jogo da cobrinha, desenvolvido na framework [MonoGame](https://monogame.net/).

Repositório original: [MonoSnake](https://github.com/iisevensii/MonoSnake)

Iremos documentar e abordar os principais ficheiros do jogo, na seguinte ordem:
1. Pasta Content
2. Pasta GameObjects
3. Pasta Infrastructure
4. Pasta UI
5. SnakeGame.cs
6. Program.cs

![snake00](https://user-images.githubusercontent.com/1765628/117053034-12058180-acde-11eb-995d-2f39d0680d79.gif)

## Pasta Content

A pasta "Content" no repositório MonoSnake contém todos os recursos que o jogo usa para funcionar, incluindo:

### Imagens:
- `apple.png`: Esta imagem é usada para representar a comida que a cobra come. É uma imagem de 42x42 pixeis que mostra uma maçã vermelha.
- `gear.png`: É usada para representar o símbolo das definições do jogo. Tem uma resolução de 57x57 pixeis.
- `ranking.png`: Esta imagem representa o símbolo do ranking do jogo. Também apresenta uma resolução de 57x57.
- `snakehead.png`: Esta imagem é usada para representar a cabeça da cobra. É uma imagem de 42x126 pixeis.
- `snakesegments.png`: Esta imagem representa o corpo da cobra em todas as direções possíveis. Tem uma resolução de 84x126.
- `UiBlueSheet.png`: Esta imagem contém todos os elementos da interface do utilizador. Tem uma resolução de 516x256.
### Sons:
- `eat.wav`: Este som é tocado quando a cobra come comida. É um som curto e agudo que indica que a cobra ganhou pontos.
- `sand_rattle.wav`: É um som curto tocado durante o jogo como ambiente. É o som típico de uma cobra.
- `hiss.wav`: Este som é usado da mesma forma do anterior, é outro som típico de uma cobra.
### Outras:
- `Arcade.ttf`: Esta fonte é usada para exibir o texto no jogo. Ela é uma fonte simples e fácil de ler. Existem vários outros ficheiros `.ttf` que são usados noutras partes da interface do jogo.

## Pasta GameObjects

### Apple (Maçã)
- `Apple` é uma classe que implementa a interface `IGameObject`.
- Representa a maçã no jogo, com propriedades para a ordem de desenho, o sprite, a posição e a rotação da maçã.
- Na função `Draw`, desenha a maçã na tela utilizando o sprite fornecido.

### IGameObject (Interface para Objetos do Jogo)
- `IGameObject` é uma interface que define as propriedades e funções básicas que todos os objetos do jogo devem implementar.
- Define propriedades como ordem de desenho, sprite, posição e rotação, e funções para atualizar e desenhar o objeto.

### SnakeHead (Cabeça da Cobra)
- `SnakeHead` é uma classe que implementa a interface `IGameObject`.
- Representa a cabeça da cobra no jogo.

## Pasta Infrastructure

### Gestão de Sprites e Animações:
As classes `AnimatedSprite` e `AnimatedSpriteFrame` lidam com a representação e animação de sprites no jogo. 
- `AnimatedSprite` representa um sprite animado, enquanto `AnimatedSpriteFrame` representa um único quadro dessa animação. 
- Ambas contêm lógica para atualizar e desenhar os sprites.

### Classe Sprite e Interface ISprite:
Ambas estão relacionadas à representação e manipulação de sprites no jogo. 
- `Sprite` é uma classe concreta que implementa a interface `ISprite`.
- `ISprite` define os atributos e funções que devem ser implementados por todas as classes que representam sprites no jogo.

### Controle de Entrada do Jogador:
As classes `InputController` e enumeração `SnakeDirection` lidam com o controle de entrada do jogador no jogo. 
- `InputController` captura e processa a entrada do jogador, enquanto `SnakeDirection` representa as direções possíveis para a cobra se mover no jogo.

### Gestão de Pontuações:
As classes `HighScores` e `ScoreEntry` estão envolvidas na gestão das pontuações do jogo. 
- `HighScores` mantém uma lista de pontuações altas e gere a adição de novas pontuações.
- `ScoreEntry` representa uma entrada de pontuação no jogo.

### Classe ScoreBoard e Enumeração ScoreBoardState:
Ambas estão relacionadas à gestão do placar de pontuações no jogo. 
- `ScoreBoard` gere o placar de pontuações, enquanto `ScoreBoardState` define os diferentes estados possíveis do placar de pontuações.

### Classe PositionedTexture2D e Enumeração UIState:
- `PositionedTexture2D` representa uma textura 2D posicionada no jogo.
- `UIState` define os diferentes estados possíveis da interface do usuário no jogo.

### TextEntry
A classe `TextEntry` é responsável por gerir a entrada de texto do utilizador num campo de texto.

#### Entrada de Texto Limitada:
O campo de texto possui um limite máximo de caracteres definido pela constante `INPUT_STRING_MAX_LENGTH`, que neste caso é definido como 15 caracteres. Isso garante que o utilizador não possa inserir mais caracteres do que o permitido.

#### Interatividade com Teclado:
A classe responde aos eventos de teclado, permitindo que o utilizador digite caracteres, exclua caracteres e navegue entre as opções de caracteres disponíveis. Os caracteres inseridos são exibidos dinamicamente no campo de texto.

#### Interface Gráfica:
A entrada de texto é exibida na tela do jogo, com um cursor piscante indicando a posição atual de inserção de texto. Os caracteres digitados são renderizados usando uma fonte de texto especificada.

#### Funcionalidade de Ciclagem:
O utilizador pode navegar entre diferentes opções de caracteres, permitindo a seleção de letras, números e outros caracteres especiais. Isso é feito através da tecla de seta para cima ou para baixo.

### Snake: Representação da Cobra
A classe `Snake` é responsável por gerir a cobra controlada pelo jogador durante o jogo.

#### Segmentação da Cobra:
A cobra é representada por uma cabeça e uma série de segmentos conectados, formando um corpo. Cada segmento possui uma posição, rotação e direção específicas.

#### Movimento e Atualização:
A cobra pode se mover em diferentes direções, respondendo às entradas do jogador. A cada atualização do jogo, a posição e a direção da cobra são ajustadas de acordo com as entradas do jogador e as regras do jogo.

#### Adição e Crescimento:
Quando a cobra come alimentos, ela cresce em comprimento adicionando novos segmentos ao seu corpo. Isso é feito através da função `AddSegment`, que cria um segmento com base na posição do último segmento.

#### Pontuação e Exibição:
A cobra acumula pontos ao comer alimentos, e a pontuação é exibida na tela do jogo. A cada segmento adicionado à cobra, a pontuação aumenta, proporcionando um feedback visual ao jogador sobre seu desempenho.

#### Interseção e Colisão:
A cobra pode colidir consigo mesma ou com os limites da tela, o que resulta no fim do jogo. A detecção de colisão é implementada para garantir que a cobra se comporte corretamente em diferentes situações.

## Pasta UI

### UIiObject

A classe `UIiObject` é fundamental para a criação de elementos de interface do utilizador no jogo. Ela representa objetos básicos da interface, como imagens, textos ou outros elementos gráficos.

Propriedades Essenciais: `DrawOrder`, `Sprite`, `Position`, `Rotation` e `IsMouseOver` são propriedades essenciais que definem o comportamento e a aparência do objeto de interface do utilizador.

Atualização e Renderização: A função `Update(GameTime gameTime)` é utilizada para atualizar o estado do objeto de interface do utilizador a cada quadro do jogo, embora esteja vazio nesta implementação. Por outro lado, a função `Draw(SpriteBatch spriteBatch, GameTime gameTime)` é responsável por renderizar o objeto na tela utilizando um `SpriteBatch`.

### UiFrame
Estrutura para Organização de Elementos de Interface do utilizador

A classe `UiFrame` é responsável por criar e gerir estruturas de quadros de interface do utilizador.

Organização Espacial: Utilizando objetos de interface do utilizador como blocos de construção, o `UiFrame` calcula e desenha uma estrutura de moldura na tela com base nos sprites fornecidos.

Layout Flexível: Os objetos de interface do utilizador são organizados em linhas e colunas para formar a estrutura do quadro. Isso permite uma flexibilidade significativa na disposição dos elementos de UI.

Atualização e Renderização: Assim como a classe `UiObject`, o `UiFrame` também possui funções `Update` e `Draw` para atualizar e renderizar o quadro na tela.

## UiButton

A classe `UiButton` é responsável por representar botões interativos na interface do utilizador.

### Estados de Interatividade:
A classe `UiButton` possui estados como Normal, Hover e MouseDown, que permitem uma resposta visual dinâmica às interações do jogador.

### Verificação de Interatividade:
A função `Update` verifica se o rato está sobre o botão e se foi clicado, alterando seu estado de acordo com a interação do utilizador.

### Renderização Responsiva:
A função `Draw` desenha o botão na tela de acordo com o estado atual, fornecendo feedback visual imediato ao jogador.

## Toggle UI Button

A classe `ToggleUiButton` é uma extensão de `UiButton` que permite a criação de botões de alternância na interface do utilizador.

### Estado de Alternância:
Além dos estados normais e de foco, o `ToggleUiButton` possui um estado adicional para quando está ativado ou desativado, oferecendo mais opções de interação.

### Alternância Dinâmica:
Quando clicado, o `ToggleUiButton` alterna entre dois estados, refletindo visualmente sua ativação ou desativação.

### Renderização Personalizada:
A renderização do `ToggleUiButton` é ajustada para refletir seu estado de alternância atual, proporcionando uma experiência de utilizador intuitiva e consistente.

## CenteredUIFrame

A classe `CenteredUiFrame` é uma extensão da classe `UiFrame` que permite a criação de quadros de interface do utilizador centralizados na tela do jogo.

### Posicionamento Centralizado:
A principal característica do `CenteredUiFrame` é que ele posiciona automaticamente o quadro de interface do utilizador no centro da tela do jogo, com base nas dimensões fornecidas pelo parâmetro parentWidth e parentHeight.

### Dimensionamento Flexível:
Assim como a classe `UiFrame`, o `CenteredUiFrame` calcula dinamicamente as dimensões e posições dos objetos de interface do utilizador que o compõem, permitindo uma adaptação flexível a diferentes tamanhos de tela.

### Inicialização Aprimorada:
A função `Initialize` é chamada automaticamente durante a construção do quadro, garantindo que todos os objetos e propriedades necessários sejam configurados corretamente.

## CenteredUIDialog

A classe `CenteredUiDialog` é responsável por criar e gerir diálogos centralizados na interface do utilizador.

### Gestão de Diálogo:
O `CenteredUiDialog` encapsula todas as funcionalidades necessárias para exibir um diálogo na tela, incluindo o título, mensagem, botões de confirmação e cancelamento, e a manipulação de eventos associados.

### Estilo Personalizado:
O diálogo pode ser personalizado com diferentes fontes, cores e estilos visuais, permitindo uma adaptação flexível ao tema e ao design geral do jogo.

### Interatividade do utilizador:
Os botões de confirmação e cancelamento podem ser clicados pelo utilizador, ativando eventos associados que podem ser utilizados para realizar ações específicas ou responder às escolhas do jogador.

## SnakeGame

### GenerateGrid():
Esta função é responsável por gerar uma grade de células que representa o espaço de jogo. Ela também determina quais células estão ocupadas pelo corpo da cobra e quais estão livres.

### GenerateApple():
Aqui é gerada uma maçã numa posição aleatória dentro das células não ocupadas pela cobra. A posição da maçã é armazenada e é verificado se a maçã foi consumida pela cobra.

### EndGameAndRecordScore():
Esta função é chamada quando o jogo termina (quando a cobra atinge a borda do espaço de jogo ou se colide consigo mesma). Ela atualiza o estado da interface do utilizador (UI) para exibir a tela de fim de jogo e verifica se o jogador alcançou uma pontuação alta para entrar no placar de líderes.

### Update(GameTime gameTime):
Aqui são processadas as entradas do jogador, atualizados os objetos do jogo (como a cobra e a maçã), verificadas as condições de fim de jogo e atualizado o estado da interface do utilizador.

### Draw(GameTime gameTime):
Esta função desenha os elementos do jogo na tela, incluindo a grade de jogo, a cobra, a maçã e os elementos da interface do utilizador, dependendo do estado atual do jogo.

Além das funções mencionadas, o código também inclui:

#### Funções de Desenho da Interface do utilizador (UI):
Existem funções específicas para desenhar elementos da interface do utilizador, como textos, botões e frames, tanto para a tela inicial (start screen) quanto para a tela de pontuação alta (high scores screen).

#### Lógica de Atualização do Jogo:
A função `Update(GameTime gameTime)` é responsável por processar as entradas do jogador, atualizar a posição dos objetos do jogo (cobra e maçã), verificar colisões e condições de fim de jogo, além de atualizar a interface do utilizador conforme necessário.

#### Lógica de Desenho do Jogo:
A função `Draw(GameTime gameTime)` desenha os elementos do jogo na tela, como a grade de jogo, a cobra, a maçã e elementos da interface do utilizador, dependendo do estado atual do jogo.

#### Geração de Elementos do Jogo:
As funções `GenerateGrid()` e `GenerateApple()` lidam com a geração da grade de jogo e da maçã, respectivamente, garantindo que sejam gerados de forma aleatória e válida.

#### Fim de Jogo e Registo de Pontuação:
A função `EndGameAndRecordScore()` é chamada quando o jogo termina e trata da atualização da interface do utilizador para exibir a tela de fim de jogo, bem como verifica se o jogador alcançou uma pontuação alta para entrar no placar de líderes.

No geral, o código encapsula a lógica central do jogo Snake, desde a geração dos elementos do jogo até a interação com o jogador e a atualização da interface do utilizador.

## Program

### Função Main:
A função `Main()` serve como ponto de entrada do programa. Ela inicializa uma instância da classe `SnakeGame`, que representa o jogo Snake, e a executa chamando a função `Run()`.

### Atributo STAThread:
O atributo `[STAThread]` indica que a thread de aplicativo COM será apartada para processamento de mensagens. Isso é comum em aplicativos GUI do Windows para garantir a compatibilidade com certas APIs do Windows.

### Usando a Declaração:
O uso da declaração `using` garante que o objeto `game` seja descartado corretamente após a execução do bloco de código, garantindo a liberação de recursos.

Este código é responsável por iniciar o jogo Snake e controlar o fluxo de execução do programa.

### Melhorias Futuras

O projeto MonoSnake apresenta uma base sólida para um jogo da cobrinha, mas ainda há várias áreas que podem ser aprimoradas para melhorar a experiência do jogador e adicionar novos recursos. Algumas possíveis melhorias futuras incluem:

1. **Melhorias na Interface do Usuário (UI)**:
   - Implementação de uma interface mais intuitiva e visualmente atraente, com elementos de UI mais dinâmicos e animações suaves.
   - Adição de opções de personalização da interface do jogador, como temas de cores, skins de cobra e configurações de fonte.

2. **Novos Modos de Jogo**:
   - Introdução de diferentes modos de jogo, como modo de sobrevivência, modo de velocidade crescente ou modo multiplayer.
   - Adição de desafios e objetivos específicos para cada modo de jogo, aumentando a variedade e a longevidade da experiência de jogo.

3. **Recursos Adicionais**:
   - Incorporação de novos elementos de jogabilidade, como power-ups especiais, obstáculos variados ou itens de personalização da cobra.
   - Expansão da biblioteca de sons e músicas do jogo para oferecer uma experiência auditiva mais rica e imersiva.

4. **Otimização de Desempenho**:
   - Identificação e correção de possíveis bugs de desempenho para garantir uma jogabilidade suave e responsiva numa variedade de dispositivos.
   - Implementação de técnicas de otimização de renderização e atualização de objetos para melhorar a eficiência do jogo.

5. **Aprimoramentos de IA**:
   - Desenvolvimento de uma IA para controlar a cobra em modos de jogo single-player, oferecendo desafios adicionais e uma experiência mais dinâmica.
   - Adição de opções de dificuldade ajustável para a IA, permitindo que os jogadores personalizem o nível de desafio conforme sua habilidade.

6. **Suporte Multiplataforma**:
   - Portabilidade do jogo para outras plataformas, como dispositivos móveis ou consolas, expandindo assim sua base de utilizadores e alcance.
   - Adaptação da interface do usuário e dos controlos para diferentes dispositivos e métodos de entrada, garantindo uma experiência consistente em todas as plataformas.

## Conclusão

Com base em toda a documentação apresentada sobre o projeto MonoSnake, podemos concluir que este é um esforço colaborativo de desenvolvimento de um clássico jogo da cobrinha usando a estrutura do MonoGame. A equipa responsável por este projeto, demonstrou habilidade na implementação de diversos componentes essenciais para o funcionamento do jogo.

Ao longo da documentação, foram abordados detalhes sobre a estrutura do código, os recursos utilizados, como imagens e sons, e a lógica por trás dos principais elementos do jogo, como a cobra, a maçã e a interface do usuário. Além disso, foram apresentadas informações sobre a organização dos arquivos nas diferentes pastas do projeto, fornecendo uma visão geral da arquitetura do MonoSnake.

Destacam-se também as classes e funções específicas responsáveis por aspectos cruciais do jogo, como a geração de elementos, o controle de entrada do jogador, a gestão de pontuações e a atualização da interface do usuário. Esses componentes foram cuidadosamente desenvolvidos para proporcionar uma experiência de jogo envolvente e satisfatória.

Em resumo, o projeto MonoSnake representa um exemplo bem-sucedido de colaboração e desenvolvimento de um jogo clássico usando a framework MonoGame, com atenção aos detalhes e à qualidade do código. O repositório original do projeto pode ser encontrado em [MonoSnake](https://github.com/iisevensii/MonoSnake), onde está disponível para contribuições e para aqueles que desejam explorar mais a fundo seu código-fonte.
