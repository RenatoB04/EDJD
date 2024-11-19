import React, { useState } from "react";
import LoginForm from "./components/LoginForm";
import GameBoard from "./components/Game";
import { signOut } from "firebase/auth";
import { auth } from "./firebase";

function App() {
  const [user, setUser] = useState(null);
  const [isGameRunning, setIsGameRunning] = useState(false);

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

  return (
    <div className="app-container">
      {!user ? (
        <LoginForm onLogin={handleLogin} />
      ) : (
        <div>
          <header>
            <h1>Bem-vindo, {user.email}!</h1>
            <button onClick={handleLogout}>Logout</button>
            {!isGameRunning ? (
              <button onClick={startGame}>Iniciar Jogo</button>
            ) : (
              <button onClick={exitGame}>Sair do Jogo</button>
            )}
          </header>
          {isGameRunning && <GameBoard />}
        </div>
      )}
    </div>
  );
}

export default App;