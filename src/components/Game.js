import React, { useState, useEffect } from "react";
import "../styles/Game.css";
import { doc, getDoc, setDoc, updateDoc, collection, getDocs, query, orderBy, limit } from "firebase/firestore";
import { db } from "../firebase";
import { auth } from "../firebase";

const WASTE_TYPES = [
  { type: "papel", emoji: "📄", bin: "papel" },
  { type: "papel", emoji: "📖", bin: "papel" },
  { type: "papel", emoji: "📜", bin: "papel" },
  { type: "plástico", emoji: "🛍️", bin: "plástico" },
  { type: "plástico", emoji: "🪣", bin: "plástico" },
  { type: "plástico", emoji: "🥤", bin: "plástico" },
  { type: "vidro", emoji: "🍷", bin: "vidro" },
  { type: "vidro", emoji: "🥛", bin: "vidro" },
  { type: "vidro", emoji: "🍾", bin: "vidro" },
  { type: "orgânico", emoji: "🍎", bin: "orgânico" },
  { type: "orgânico", emoji: "🍌", bin: "orgânico" },
  { type: "orgânico", emoji: "🥬", bin: "orgânico" },
  { type: "metal", emoji: "🪙", bin: "metal" },
  { type: "metal", emoji: "⚙️", bin: "metal" },
  { type: "metal", emoji: "🔩", bin: "metal" },
];

export const getRanking = async () => {
  try {
    const rankingRef = collection(db, "rankings");
    const q = query(rankingRef, orderBy("highScore", "desc"), limit(10));
    const querySnapshot = await getDocs(q);

    const ranking = querySnapshot.docs.map((doc) => ({
      id: doc.id,
      ...doc.data(),
    }));

    return ranking;
  } catch (error) {
    console.error("Erro a encontrar o ranking:", error);
    return [];
  }
};

const GameBoard = () => {
  const columns = 5;
  const rows = 6;
  const [grid, setGrid] = useState(Array.from({ length: columns }, () => []));
  const [score, setScore] = useState(0);
  const [timer, setTimer] = useState(60);
  const [speed, setSpeed] = useState(2000);
  const [gameOver, setGameOver] = useState(false);

  const bins = ["papel", "plástico", "vidro", "orgânico", "metal"];

  const generateWaste = () => {
    const randomColumn = Math.floor(Math.random() * columns);
    const randomWaste =
      WASTE_TYPES[Math.floor(Math.random() * WASTE_TYPES.length)];

    setGrid((prev) => {
      const newGrid = [...prev];
      if (newGrid[randomColumn].length >= rows) {
        endGame();
        return prev;
      }

      newGrid[randomColumn] = [...newGrid[randomColumn], randomWaste];
      return newGrid;
    });
  };

  const saveScoreToRanking = async (finalScore) => {
    const user = auth.currentUser;
    if (!user) {
      console.warn("Usuário não logado. A avançar save no Firebase.");
      return;
    }

    const userRef = doc(db, "rankings", user.uid);
    const userDoc = await getDoc(userRef);

    try {
      if (userDoc.exists()) {
        const currentHighScore = userDoc.data().highScore || 0;

        if (finalScore > currentHighScore) {
          await updateDoc(userRef, { highScore: finalScore });
          console.log("Ranking atualizado no Firebase!");
        }
      } else {
        await setDoc(userRef, { name: user.displayName, highScore: finalScore });
        console.log("Novo score guardado no Firebase!");
      }
    } catch (error) {
      console.error("Erro a guardar score no Firestore:", error);
    }
  };

  const handleDrop = (waste, bin, colIndex) => {
    const binElement = document.querySelector(`[data-bin="${bin}"]`);

    if (waste.bin === bin) {
      setScore((prev) => prev + 10);
      setTimer((prev) => prev + 3);

      setGrid((prev) => {
        const newGrid = [...prev];
        const updatedColumn = [...newGrid[colIndex]];
        updatedColumn.shift();
        newGrid[colIndex] = updatedColumn;
        return newGrid;
      });
    } else {
      setTimer((prev) => Math.max(prev - 10, 0));
      binElement.classList.add("bin-error");
      setTimeout(() => {
        binElement.classList.remove("bin-error");
      }, 300);
    }
  };

  const endGame = async () => {
    if (gameOver) return;

    setGameOver(true);

    setScore((prevScore) => {
      saveScoreToRanking(prevScore);
      return prevScore;
    });
  };

  const resetGame = () => {
    setGameOver(false);
    setGrid(Array.from({ length: columns }, () => []));
    setScore(0);
    setTimer(60);
    setSpeed(2000);
  };

  useEffect(() => {
    if (timer <= 0) {
      endGame();
    }
  }, [timer]);

  useEffect(() => {
    const interval = setInterval(() => {
      setTimer((prev) => prev - 1);
    }, 1000);

    return () => clearInterval(interval);
  }, []);

  useEffect(() => {
    const wasteInterval = setInterval(() => {
      generateWaste();
    }, speed);

    return () => clearInterval(wasteInterval);
  }, [speed]);

  useEffect(() => {
    if (score > 0 && score % 5 === 0) {
      setSpeed((prev) => Math.max(prev - 200, 800));
    }
  }, [score]);

  return (
    <div className="game-board">
      <div className="info">
        <p>Pontuação: {score}</p>
        <p>Tempo: {timer}s</p>
      </div>
      <div className="falling-area">
        {!gameOver ? (
          grid.map((col, colIndex) => (
            <div key={colIndex} className="column">
              {col.map((waste, rowIndex) => (
                <div
                  key={`${colIndex}-${rowIndex}`}
                  className="waste"
                  draggable
                  onDragStart={(e) =>
                    e.dataTransfer.setData(
                      "waste",
                      JSON.stringify({ waste, colIndex })
                    )
                  }
                >
                  {waste.emoji}
                </div>
              ))}
            </div>
          ))
        ) : (
          <div className="game-over">
            <h2>Fim de Jogo!</h2>
            <p>Sua pontuação final foi: {score}</p>
            <button onClick={resetGame}>Reiniciar Jogo</button>
          </div>
        )}
      </div>
      <div className="bins">
        {!gameOver &&
          bins.map((bin) => (
            <div
              key={bin}
              className="bin"
              data-bin={bin}
              onDragOver={(e) => e.preventDefault()}
              onDrop={(e) => {
                const droppedData = JSON.parse(e.dataTransfer.getData("waste"));
                handleDrop(droppedData.waste, bin, droppedData.colIndex);
              }}
            >
              {bin}
            </div>
          ))}
      </div>
    </div>
  );
};

export default GameBoard;