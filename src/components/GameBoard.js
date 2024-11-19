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
  const columns = 5; // Número de colunas
  const rows = 10; // Número máximo de itens em uma coluna
  const [grid, setGrid] = useState(Array.from({ length: columns }, () => []));
  const [score, setScore] = useState(0);
  const [timer, setTimer] = useState(60);
  const [speed, setSpeed] = useState(2000); // Velocidade inicial (ms)

  const bins = ["papel", "plástico", "vidro", "orgânico", "metal"];

  const generateWaste = () => {
    const randomColumn = Math.floor(Math.random() * columns);
    const randomWaste =
      WASTE_TYPES[Math.floor(Math.random() * WASTE_TYPES.length)];

    setGrid((prev) => {
      if (prev[randomColumn].length >= rows) {
        // Fim de jogo se uma coluna atingir o topo
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
      setTimer((prev) => prev + 3); // Aumenta o tempo
    } else {
      setTimer((prev) => Math.max(prev - 10, 0)); // Diminui o tempo
    }

    // Remove o lixo da coluna
    setGrid((prev) => {
      const newGrid = [...prev];
      newGrid[colIndex] = newGrid[colIndex].filter(
        (item, index) => index !== prev[colIndex].indexOf(waste)
      );
      return newGrid;
    });
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
      setSpeed((prev) => Math.max(prev - 200, 800)); // Aumenta a frequência dos resíduos
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
                key={rowIndex}
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