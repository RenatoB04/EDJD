import React, { useState } from 'react';
import LevelOne from './LevelOne';
import './styles/App.css';

function App() {
  const [startGame, setStartGame] = useState(false);

  return (
    <div className="app-container">
      {!startGame ? (
        <div className="start-screen">
          <h1>Guardian of Balance</h1>
          <button onClick={() => setStartGame(true)}>Começar o Jogo</button>
        </div>
      ) : (
        <LevelOne />
      )}
    </div>
  );
}

export default App;
