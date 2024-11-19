import React, { useState, useEffect } from "react";
import "./Game.css";

const WASTE_TYPES = [
  { type: "papel", emoji: "📄", bin: "papel" },
  { type: "plástico", emoji: "🛍️", bin: "plástico" },
  { type: "vidro", emoji: "🍾", bin: "vidro" },
  { type: "orgânico", emoji: "🍎", bin: "orgânico" },
  { type: "metal", emoji: "🪙", bin: "metal" },
];

const GameBoard = () => {
  const columns = 5;
  const rows = 10;
  const [grid, setGrid] = useState(Array.from({ length: columns }, () => []));
  const [score, setScore] = useState(0);
  const [timer, setTimer] = useState(60);
  const [speed, setSpeed] = useState(2000);

  const bins = ["papel", "plástico", "vidro", "orgânico", "metal"];

  const generateWaste = () => {
    const randomColumn = Math.floor(Math.random() * columns);
    const randomWaste =
      WASTE_TYPES[Math.floor(Math.random() * WASTE_TYPES.length)];

    setGrid((prev) => {
      if (prev[randomColumn].length >= rows) {
        alert(`Fim de jogo! Pontuação final: ${score}`);
        resetGame();
        return prev;
      }

      const newGrid = [...prev];
      newGrid[randomColumn] = [...newGrid[randomColumn], randomWaste];
      return newGrid;
    });
  };

  const handleDrop = (waste, bin, colIndex) => {
    if (waste.bin === bin) {
      setScore((prev) => prev + 10);
      setTimer((prev) => prev + 3);

      setGrid((prev) => {
        const newGrid = [...prev];
        const updatedColumn = [...newGrid[colIndex]];
        const startIndex = updatedColumn.findIndex(
          (item) => item.emoji === waste.emoji && item.type === waste.type
        );

        if (startIndex !== -1) {
          for (let i = startIndex; i < updatedColumn.length; i++) {
            if (
              updatedColumn[i]?.emoji === waste.emoji &&
              updatedColumn[i]?.type === waste.type
            ) {
              updatedColumn[i] = null;
            } else {
              break;
            }
          }
          newGrid[colIndex] = updatedColumn.filter((item) => item !== null);
        }
        return newGrid;
      });
    } else {
      setTimer((prev) => Math.max(prev - 10, 0));
    }
  };

  const resetGame = () => {
    setGrid(Array.from({ length: columns }, () => []));
    setScore(0);
    setTimer(60);
    setSpeed(2000);
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