import React, { useState, useEffect } from "react";
import "../styles/Game.css";
import { doc, getDoc, setDoc, updateDoc, collection, getDocs, query, orderBy, limit } from "firebase/firestore";
import { db } from "../firebase";
import { auth } from "../firebase";
import MessageOverlay from "./MessageOverlay";
import PollutionBar from "./PollutionBar";

const WASTE_TYPES = [
  { type: "papel", emoji: "📄", bin: "papel" },
  { type: "papel", emoji: "📖", bin: "papel" },
  { type: "papel", emoji: "📜", bin: "papel" },
  { type: "plástico", emoji: "🛍️", bin: "plástico" },
  { type: "plástico", emoji: "🧴", bin: "plástico" },
  { type: "plástico", emoji: "🥤", bin: "plástico" },
  { type: "vidro", emoji: "🍷", bin: "vidro" },
  { type: "vidro", emoji: "🥛", bin: "vidro" },
  { type: "vidro", emoji: "🍾", bin: "vidro" },
  { type: "orgânico", emoji: "🍎", bin: "orgânico" },
  { type: "orgânico", emoji: "🍌", bin: "orgânico" },
  { type: "orgânico", emoji: "🥬", bin: "orgânico" },
  { type: "metal", emoji: "🔧", bin: "metal" },
  { type: "metal", emoji: "⚙️", bin: "metal" },
  { type: "metal", emoji: "🔩", bin: "metal" },
];

export const getRanking = async () => {
  try {
    const rankingRef = collection(db, 'rankings');
    const q = query(rankingRef, orderBy('highScore', 'desc'), limit(10));
    const querySnapshot = await getDocs(q);

    const ranking = querySnapshot.docs.map((doc) => ({
      id: doc.id,
      ...doc.data(),
    }));

    return ranking;
  } catch (error) {
    console.error('Erro a encontrar o ranking geral:', error);
    return [];
  }
};

export const getPollutionRanking = async () => {
  try {
    const rankingRef = collection(db, 'ranking_pollution');
    const q = query(rankingRef, orderBy('contribution', 'desc'), limit(10));
    const querySnapshot = await getDocs(q);

    const ranking = querySnapshot.docs.map((doc) => ({
      id: doc.id,
      ...doc.data(),
    }));

    return ranking;
  } catch (error) {
    console.error('Erro a encontrar o ranking de poluição:', error);
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
  const [showMessage, setShowMessage] = useState(true);
  const [messageIndex, setMessageIndex] = useState(0);

  const bins = ["papel", "plástico", "vidro", "orgânico", "metal"];

  const generateWaste = () => {
    const randomColumn = Math.floor(Math.random() * columns);
    const randomWaste = WASTE_TYPES[Math.floor(Math.random() * WASTE_TYPES.length)];

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
    console.log('Nenhum utilizador autenticado.');
    return;
  }

  const userRef = doc(db, 'ranking_pollution', user.uid);
  const globalRef = doc(db, 'globalStats', 'pollutionBar');

  try {
    const userDoc = await getDoc(userRef);
    const globalDoc = await getDoc(globalRef);

    let globalScore = globalDoc.exists() ? globalDoc.data().score : 10000;
    console.log('Poluição antes do jogo:', globalScore);

    if (userDoc.exists()) {
      await updateDoc(userRef, {
        contribution: userDoc.data().contribution + finalScore,
        gamesPlayed: (userDoc.data().gamesPlayed || 0) + 1,
      });
      console.log(`Contribuição de ${finalScore} adicionada para:`, user.displayName);
    } else {
      await setDoc(userRef, {
        name: user.displayName,
        contribution: finalScore,
        gamesPlayed: 1,
      });
      console.log('Novo jogador adicionado:', user.displayName);
    }

    let newScore = globalScore;

    if (finalScore > 0) {
      newScore = Math.max(globalScore - finalScore, 0);
      console.log(`Pontuação de ${finalScore}. Poluição REDUZIDA para:`, newScore);
    } else {
      newScore = Math.min(globalScore + 100, 10000);
      console.log('Nenhum ponto feito. Poluição AUMENTADA para:', newScore);
    }

    await updateDoc(globalRef, { score: newScore });

  } catch (error) {
    console.error('Erro ao guardar score:', error);
  }
};

  
const handleDrop = async (waste, bin, colIndex) => {
  const binElement = document.querySelector(`[data-bin="${bin}"]`);

  if (waste.bin === bin) {
    setScore((prev) => prev + 10);
    setTimer((prev) => prev + 2);

    if ((score + 10) % 50 === 0) {
      setSpeed((prev) => Math.max(prev - 300, 500));
    }

    setGrid((prev) => {
      const newGrid = [...prev];
      newGrid[colIndex] = newGrid[colIndex].filter((_, index) => index !== 0);
      return newGrid;
    });

  } else {
    setTimer((prev) => Math.max(prev - 10, 0));
    binElement.classList.add('bin-error');
    setTimeout(() => {
      binElement.classList.remove('bin-error');
    }, 300);
  }
};

  const endGame = async () => {
    if (gameOver) return;
  
    setGameOver(true);
  
    const finalScore = await new Promise((resolve) => {
      setScore((prevScore) => {
        const newScore = prevScore;
        resolve(newScore);
        return newScore;
      });
    });
  
    console.log('Score final antes de salvar:', finalScore);
  
    await saveScoreToRanking(finalScore);
  };

  const resetGame = () => {
    setGameOver(false);
    setGrid(Array.from({ length: columns }, () => []));
    setScore(0);
    setTimer(60);
    setSpeed(2000);
  };

  useEffect(() => {
    const interval = setInterval(() => {
      setTimer((prev) => Math.max(prev - 1, 0));
    }, 1000);

    return () => clearInterval(interval);
  }, []);

  useEffect(() => {
    if (timer <= 0) {
      endGame();
    }
  }, [timer]);

  useEffect(() => {
    const wasteInterval = setInterval(() => {
      if (!gameOver) {
        generateWaste();
      }
    }, speed);

    return () => clearInterval(wasteInterval);
  }, [speed, gameOver]);

  useEffect(() => {
    const messageInterval = setInterval(() => {
      setShowMessage(false);
      setTimeout(() => {
        setShowMessage(true);
        setMessageIndex((prev) => (prev + 1) % 20);
      }, 100);
    }, 7000);

    return () => clearInterval(messageInterval);
  }, []);

  return (
    <div className="game-container">
      <PollutionBar />
      <div className="game-board">
        {showMessage && <MessageOverlay messageIndex={messageIndex} />}
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
                      e.dataTransfer.setData('waste', JSON.stringify({ waste, colIndex }))
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
                  const droppedData = e.dataTransfer.getData("waste");
                  if (!droppedData) return;
                  const parsedData = JSON.parse(droppedData);
                  handleDrop(parsedData.waste, bin, parsedData.colIndex);
                }}
              >
                {bin}
              </div>
            ))}
        </div>
      </div>
    </div>
  );
  
  
 };


export default GameBoard;