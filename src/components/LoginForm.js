import React, { useState } from "react";
import { signInWithEmailAndPassword, createUserWithEmailAndPassword, updateProfile } from "firebase/auth";
import { auth } from "../firebase";
import "../styles/LoginForm.css";

function LoginForm({ onLogin }) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [username, setUsername] = useState("");
  const [error, setError] = useState("");
  const [isRegistering, setIsRegistering] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (isRegistering) {
        const userCredential = await createUserWithEmailAndPassword(auth, email, password);

        await updateProfile(userCredential.user, {
          displayName: username,
        });

        onLogin(userCredential.user);
      } else {
        const userCredential = await signInWithEmailAndPassword(auth, email, password);
        onLogin(userCredential.user);
      }
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div className="login-form">
      <h2>{isRegistering ? "Criar Conta" : "Login"}</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <label>Email:</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </div>
        <div>
          <label>Senha:</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        {isRegistering && (
          <div>
            <label>Nome:</label>
            <input
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
            />
          </div>
        )}
        {error && <p className="error">{error}</p>}
        <button type="submit">
          {isRegistering ? "Criar Conta" : "Entrar"}
        </button>
      </form>
      <p>
        {isRegistering ? "Já tem uma conta?" : "Não tem uma conta?"}{" "}
        <span
          style={{ color: "blue", cursor: "pointer" }}
          onClick={() => {
            setIsRegistering(!isRegistering);
            setError("");
          }}
        >
          {isRegistering ? "Fazer Login" : "Criar Conta"}
        </span>
      </p>
    </div>
  );
}

export default LoginForm;