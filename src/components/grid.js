import React, { useState, useEffect } from 'react';
import './Grid.css';

function Grid({ onMatch }) {
  const [grid, setGrid] = useState([
    ['🌱', '☀️', '💧'],
    ['💧', '🌱', '☀️'],
    ['☀️', '💧', '🌱'],
  ]);
  const [draggedItem, setDraggedItem] = useState(null);

  const checkMatches = () => {
    let newGrid = [...grid];
    let matched = false;

    for (let row = 0; row < grid.length; row++) {
      for (let col = 0; col < grid[row].length - 2; col++) {
        if (
          grid[row][col] === grid[row][col + 1] &&
          grid[row][col] === grid[row][col + 2]
        ) {
          matched = true;
          newGrid[row][col] = '';
          newGrid[row][col + 1] = '';
          newGrid[row][col + 2] = '';
        }
      }
    }

    for (let col = 0; col < grid[0].length; col++) {
      for (let row = 0; row < grid.length - 2; row++) {
        if (
          grid[row][col] === grid[row + 1][col] &&
          grid[row][col] === grid[row + 2][col]
        ) {
          matched = true;
          newGrid[row][col] = '';
          newGrid[row + 1][col] = '';
          newGrid[row + 2][col] = '';
        }
      }
    }

    if (matched) {
      setGrid(newGrid);
      onMatch();
    }
  };

  const fillEmptyCells = () => {
    let newGrid = [...grid];
    for (let row = 0; row < newGrid.length; row++) {
      for (let col = 0; col < newGrid[row].length; col++) {
        if (newGrid[row][col] === '') {
          newGrid[row][col] = getRandomElement();
        }
      }
    }
    setGrid(newGrid);
  };

  const getRandomElement = () => {
    const elements = ['🌱', '☀️', '💧'];
    return elements[Math.floor(Math.random() * elements.length)];
  };

  const handleDragStart = (row, col) => {
    setDraggedItem({ row, col });
  };

  const handleDrop = (row, col) => {
    if (!draggedItem) return;

    const newGrid = [...grid];
    const draggedValue = newGrid[draggedItem.row][draggedItem.col];
    newGrid[draggedItem.row][draggedItem.col] = newGrid[row][col];
    newGrid[row][col] = draggedValue;

    setGrid(newGrid);
    setDraggedItem(null);
    checkMatches();
  };

  useEffect(() => {
    fillEmptyCells();
  }, [grid]);

  return (
    <div className="grid">
      {grid.map((row, rowIndex) =>
        row.map((cell, colIndex) => (
          <div
            key={`${rowIndex}-${colIndex}`}
            className="grid-cell"
            draggable
            onDragStart={() => handleDragStart(rowIndex, colIndex)}
            onDragOver={(e) => e.preventDefault()}
            onDrop={() => handleDrop(rowIndex, colIndex)}
          >
            {cell}
          </div>
        ))
      )}
    </div>
  );
}

export default Grid;