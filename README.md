# 🌍 Jogo Interativo - Guardião do Equilíbrio 🌱

Realizado por:
- Paulo Bastos 27945
- Bruno Mesquita 27947

## 📌 Descrição do Projeto
**"Guardião do Equilíbrio"** é um jogo interativo desenvolvido com **React** que visa sensibilizar os jogadores sobre a importância da **sustentabilidade ambiental e da reciclagem**.  
Inspirado pelo movimento **Games for Change**, o jogo combina diversão com educação ambiental, promovendo boas práticas de gestão de resíduos.  
O projeto aborda temas como **poluição, reciclagem e mudanças climáticas**, incentivando os jogadores a contribuir para um planeta mais saudável.  

---

## 🎯 Objetivo do Projeto
O objetivo principal é criar uma experiência interativa que:  
- **Educa e sensibiliza** sobre a importância da separação de resíduos.  
- Incentiva boas práticas de reciclagem de forma divertida e competitiva.  
- Promove a ideia de que **todos podemos contribuir para reduzir a poluição**.  

O jogador assume o papel de um **Guardião do Equilíbrio**, cuja missão é restaurar o planeta através da separação correta de resíduos.  

---

## 🛠️ Tecnologias Utilizadas  
- **HTML5, CSS3 e JavaScript (Fase Inicial).**  
- **React (Implementação Final).**  
- **Firebase (Autenticação, Base de Dados e Ranking).**  
- **GitHub Pages (Publicação do Projeto).**  

---

## 🚀 Publicação do Projeto  
O jogo está publicado e disponível em:  
👉 **[GitHub.io](https://renatob04.github.io/P01-PDW/)**  
O progresso do projeto pode ser acompanhado no repositório do GitHub:  
👉 **[GitHub - Repositório](https://github.com/RenatoB04/P01-PDW)**  

---

## 📂 Estrutura do Projeto  
```
📁 src  
│   ├── components  
│   │   ├── Game.js                # Motor principal do jogo  
│   │   ├── PollutionBar.js        # Barra de poluição que reflete progresso  
│   │   ├── LoginForm.js           # Formulário de login  
│   │   ├── MessageOverlay.js      # Mensagens educativas durante o jogo  
│   │   └── IntroModal.js          # Modal introdutório com instruções do jogo  
│   ├── styles                    # Ficheiros de estilo CSS  
│   ├── App.js                    # Componente principal da aplicação  
│   └── index.js                  # Ponto de entrada do React  
│  
└── public  
    └── index.html                # Estrutura HTML base  
```

---

## 🧩 Funcionalidades do Jogo  

### 🎮 Mecânicas de Jogo  
- **Separação de Resíduos:** O jogador arrasta diferentes tipos de resíduos (papel, vidro, plástico, etc.) para o contentor correto.  
- **Pontuação e Tempo:**  
   - **+10 pontos e +3 segundos** por cada resíduo reciclado corretamente.  
   - **-10 segundos** por erro.  
   - **A cada 5 acertos consecutivos,** a velocidade do jogo aumenta e a barra de poluição diminui em **1%**.  

- **Fim de Jogo:** O jogo termina se o tempo chegar a zero, se as colunas se encherem de resíduos (máx: 6) ou se a barra de poluição atingir o limite máximo.  
  

---

### 🏆 Ranking Global  
- **Pontuação em Tempo Real:** A pontuação dos jogadores é enviada para o Firebase, onde é atualizada no ranking global.  
- **Ranking de Poluição:** Os jogadores contribuem coletivamente para reduzir a barra de poluição global.  
- **Atualização Dinâmica:** O ranking é atualizado em tempo real, permitindo ver as contribuições de outros jogadores.  

---

## 🖥️ Interface e Design  
- **Modal Introdutório:** Ao entrar no jogo, é apresentado um modal com instruções claras e contexto sobre a missão do jogador.  
- **Feedback Visual:** A barra de poluição muda de cor (verde, amarelo, vermelho) para indicar o estado atual do planeta.  
- **Mensagens Educativas:** Durante o jogo, surgem mensagens educativas sobre reciclagem.  
- **Interface Simples e Intuitiva:** O design é responsivo e minimal.  

---

## 🧱 Componentes do Projeto  
### `App.js` (Componente Principal)  
- Gere a lógica de autenticação, início de jogo e logout.  
- Renderiza o `GameBoard` (tabuleiro de jogo) ou o `Ranking`, consoante o estado do jogo.  
- Exibe o `IntroModal` após o login para explicar o objetivo do jogo.  

### `Game.js` (Tabuleiro do Jogo)  
- Gere o fluxo do jogo, incluindo a geração de resíduos e a lógica de pontuação.  
- Atualiza a barra de poluição em tempo real através do Firebase.  
- Usa `useState` e `useEffect` para controlar o temporizador e a velocidade.  

### `PollutionBar.js` (Barra de Poluição)  
- Representa visualmente o nível de poluição.  
- A barra diminui com acertos e aumenta com erros ou inatividade.  
- Usa `onSnapshot` para atualizar em tempo real através do Firestore.  

### `IntroModal.js` (Instruções Iniciais)  
- Modal que aparece após o login para explicar as regras e a narrativa do jogo.  
- Inclui um botão de "Começar" para iniciar o jogo.  

---
## 🏁 Conclusão  

Desenvolver o projeto foi um desafio, especialmente devido à falta de experiência com **React** e **Firebase**. Apesar das dificuldades, conseguimos **cumprir todos os requisitos**, criando um jogo funcional, educativo e alinhado com o tema da sustentabilidade.  

O projeto inclui um **ranking global**, **pontuação em tempo real** e **mensagens educativas**, oferecendo uma experiência que sensibiliza para a importância da reciclagem. A publicação no **GitHub Pages** permite que o jogo esteja acessível a todos.  

Foi uma experiência enriquecedora que nos permitiu crescer e aprender, mostrando que, com dedicação, conseguimos alcançar os objetivos propostos.  
