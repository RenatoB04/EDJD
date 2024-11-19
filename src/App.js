import React, { useState } from "react";
import LoginForm from "./components/LoginForm";
import Grid from "./components/Grid"; // Jogo principal
import { signOut } from "firebase/auth";
import { auth } from "./firebase";

function App() {
  const [user, setUser] = useState(null);

  const handleLogin = (user) => {
    setUser(user);
  };

  const handleLogout = async () => {
    await signOut(auth);
    setUser(null);
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
          </header>
          <Grid />
        </div>
      )}
    </div>
  );
}

export default App;