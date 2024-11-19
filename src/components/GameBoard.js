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
  const [fallingWaste, setFallingWaste] = useState([]);
  const [score, setScore] = useState(0);
  const [timer, setTimer] = useState(60);
  const [speed, setSpeed] = useState(2000);

  const bins = ["papel", "plástico", "vidro", "orgânico", "metal"];

  const generateWaste = () => {
    const randomWaste =
      WASTE_TYPES[Math.floor(Math.random() * WASTE_TYPES.length)];
    setFallingWaste((prev) => [
      ...prev,
      { ...randomWaste, id: Date.now(), position: 0 },
    ]);
  };

  const handleDrop = (waste, bin) => {
    if (waste.bin === bin) {
      setScore((prev) => prev + 10);
      setTimer((prev) => prev + 3);
    } else {
      setTimer((prev) => Math.max(prev - 10, 0));
    }
    setFallingWaste((prev) => prev.filter((item) => item.id !== waste.id));
  };

  useEffect(() => {
    if (timer <= 0) {
      alert(`Fim de jogo! Pontuação final: ${score}`);
      setTimer(60);
      setScore(0);
      setSpeed(2000);
      setFallingWaste([]);
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
      setFallingWaste((prev) =>
        prev.map((waste) => ({
          ...waste,
          position: waste.position + 20,
        }))
      );
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
        {fallingWaste.map((waste) => (
          <div
            key={waste.id}
            className="waste"
            style={{ top: `${waste.position}px` }}
            draggable
            onDragStart={(e) => e.dataTransfer.setData("waste", JSON.stringify(waste))}
            onAnimationEnd={() => handleDrop(waste, "nenhum")}
          >
            {waste.emoji}
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
              const droppedWaste = JSON.parse(e.dataTransfer.getData("waste"));
              handleDrop(droppedWaste, bin);
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