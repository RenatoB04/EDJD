import React, { useState } from 'react';
import Grid from './components/Grid';

function App() {
  const [startGame, setStartGame] = useState(false);

  return (
    <div className="app-container">
      {!startGame ? (
        <div className="start-screen">
          <h1>Jogo: Conecte 6!</h1>
          <button onClick={() => setStartGame(true)}>Começar o Jogo</button>
        </div>
      ) : (
        <div className="game-screen">
          <h1>Jogo: Conecte 6!</h1>
          <Grid />
        </div>
      )}
    </div>
  );
}

export default App;