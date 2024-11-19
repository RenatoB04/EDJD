import { initializeApp } from "firebase/app";
import { getAuth } from "firebase/auth";

// Your web app's Firebase configuration
const firebaseConfig = {
  apiKey: "API_KEY",
  authDomain: "p01-pdw.firebaseapp.com",
  projectId: "p01-pdw",
  storageBucket: "p01-pdw.firebasestorage.app",
  messagingSenderId: "613218366398",
  appId: "1:613218366398:web:a394c61be0c7f458923fef"
};

// Inicializa o Firebase
const app = initializeApp(firebaseConfig);

// Exporta o serviço de autenticação
const auth = getAuth(app);
export { auth };