import React, { useState, useEffect } from "react";
import "../styles/Game.css";

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

const GameBoard = () => {
  const columns = 5;
  const rows = 6;
  const [grid, setGrid] = useState(Array.from({ length: columns }, () => []));
  const [score, setScore] = useState(0);
  const [timer, setTimer] = useState(60);
  const [speed, setSpeed] = useState(2000);
  const [draggedWaste, setDraggedWaste] = useState(null);

  const bins = ["papel", "plástico", "vidro", "orgânico", "metal"];

  const generateWaste = () => {
    const randomColumn = Math.floor(Math.random() * columns);
    const randomWaste =
      WASTE_TYPES[Math.floor(Math.random() * WASTE_TYPES.length)];

    setGrid((prev) => {
      const newGrid = [...prev];
      if (newGrid[randomColumn].length >= rows) {
        alert(`Fim de jogo! Pontuação final: ${score}`);
        resetGame();
        return prev;
      }

      newGrid[randomColumn] = [...newGrid[randomColumn], randomWaste];
      return newGrid;
    });
  };

  const handleDragStart = (waste, colIndex) => {
    setDraggedWaste({ waste, colIndex });
  };

  const handleDrop = (bin) => {
    const binElement = document.querySelector(`[data-bin="${bin}"]`);

    if (draggedWaste) {
      const { waste, colIndex } = draggedWaste;
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
      setDraggedWaste(null);
    }
  };

  const resetGame = () => {
    setGrid(Array.from({ length: columns }, () => []));
    setScore(0);
    setTimer(60);
    setSpeed(2000);
    setDraggedWaste(null);
  };

  useEffect(() => {
    if (timer <= 0) {
      alert(`Fim de jogo! Pontuação final: ${score}`);
      resetGame();
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
        {grid.map((col, colIndex) => (
          <div key={colIndex} className="column">
            {col.map((waste, rowIndex) => (
              <div
                key={`${colIndex}-${rowIndex}`}
                className="waste"
                draggable
                onDragStart={() => handleDragStart(waste, colIndex)}
                onTouchStart={() => handleDragStart(waste, colIndex)}
              >
                {waste.emoji}
              </div>
            ))}
          </div>
        ))}
      </div>
      <div className="bins">
        {bins.map((bin) => (
          <div
            key={bin}
            className="bin"
            data-bin={bin}
            onDragOver={(e) => e.preventDefault()}
            onDrop={(e) => {
              e.preventDefault();
              handleDrop(bin);
            }}
            onTouchEnd={() => handleDrop(bin)}
          >
            {bin}
          </div>
        ))}
      </div>
    </div>
  );
};

export default GameBoard;