import React from 'react';
import './Grid.css';

function Grid({ onMatch }) {
  const grid = [
    ['🌱', '☀️', '💧'],
    ['💧', '🌱', '☀️'],
    ['☀️', '💧', '🌱'],
  ];

  return (
    <div className="grid">
      {grid.map((row, rowIndex) =>
        row.map((cell, colIndex) => (
          <div
            key={`${rowIndex}-${colIndex}`}
            className="grid-cell"
            onClick={() => onMatch()}
          >
            {cell}
          </div>
        ))
      )}
    </div>
  );
}

export default Grid;
