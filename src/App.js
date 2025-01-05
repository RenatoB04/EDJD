import React, { useState, useEffect } from "react";
import LoginForm from "./components/LoginForm";
import GameBoard from "./components/Game";
import { signOut } from "firebase/auth";
import { auth } from "./firebase";
import { getRanking, getPollutionRanking } from "./components/Game";

function App() {
  const [user, setUser] = useState(null);
  const [isGameRunning, setIsGameRunning] = useState(false);
  const [generalRanking, setGeneralRanking] = useState([]);
  const [pollutionRanking, setPollutionRanking] = useState([]);

  const handleLogin = (user) => {
    setUser(user);
  };

  const handleLogout = async () => {
    await signOut(auth);
    setUser(null);
    setIsGameRunning(false);
  };

  const startGame = () => {
    setIsGameRunning(true);
  };

  const exitGame = () => {
    setIsGameRunning(false);
  };

  useEffect(() => {
    const fetchRankings = async () => {
      const generalRankingData = await getRanking();
      const pollutionRankingData = await getPollutionRanking();
      setGeneralRanking(generalRankingData);
      setPollutionRanking(pollutionRankingData);
    };

    fetchRankings();
  }, []);

  return (
    <div className="app-container">
      {!user ? (
        <LoginForm onLogin={handleLogin} />
      ) : (
        <div>
          <header>
            <h1>Bem-vindo, {user.displayName}!</h1>
            <button onClick={handleLogout}>Logout</button>
            {!isGameRunning ? (
              <button onClick={startGame}>Iniciar Jogo</button>
            ) : (
              <button onClick={exitGame}>Sair do Jogo</button>
            )}
          </header>
          {isGameRunning ? (
            <GameBoard />
          ) : (
            <div>
              <h2>Ranking Geral de Pontos</h2>
              <ul>
                {generalRanking.map((player, index) => (
                  <li key={player.id}>
                    {index + 1}. {player.name} - {player.highScore} pontos
                  </li>
                ))}
              </ul>

              <h2>Ranking - Contribuição Poluição</h2>
              <ul>
                {pollutionRanking.map((player, index) => (
                  <li key={player.id}>
                    {index + 1}. {player.name} - {player.contribution} pontos
                  </li>
                ))}
              </ul>
            </div>
          )}
        </div>
      )}
    </div>
  );
}

export default App;