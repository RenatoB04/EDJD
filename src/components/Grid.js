import React, { useState, useEffect } from 'react';
import './Grid.css';

function Grid() {
  const symbols = ['💎', '🔔', '🍇', '🌟', '🍋', '🔥', '⚡', '🌈'];
  const gridRows = 6;
  const gridCols = 5;
  const [grid, setGrid] = useState([]);
  const [removingCells, setRemovingCells] = useState([]);
  const [draggedItem, setDraggedItem] = useState(null);

  const generateGrid = () => {
    return Array.from({ length: gridRows }, () =>
      Array.from({ length: gridCols }, () => getRandomElement())
    );
  };

  const getRandomElement = () => {
    return symbols[Math.floor(Math.random() * symbols.length)];
  };

  const findMatches = (row, col, visited = new Set()) => {
    if (!grid[row] || grid[row][col] === undefined) {
      return [];
    }

    const symbol = grid[row][col];
    const key = `${row},${col}`;

    if (
      row < 0 ||
      col < 0 ||
      row >= gridRows ||
      col >= gridCols ||
      visited.has(key) ||
      grid[row][col] !== symbol
    ) {
      return [];
    }

    visited.add(key);

    return [
      { row, col },
      ...findMatches(row - 1, col, visited),
      ...findMatches(row + 1, col, visited),
      ...findMatches(row, col - 1, visited),
      ...findMatches(row, col + 1, visited),
      ...findMatches(row - 1, col - 1, visited),
      ...findMatches(row - 1, col + 1, visited),
      ...findMatches(row + 1, col - 1, visited),
      ...findMatches(row + 1, col + 1, visited),
    ];
  };

  const handleMatch = (matches) => {
    const newGrid = [...grid];
    matches.forEach(({ row, col }) => {
      newGrid[row][col] = '';
    });

    setRemovingCells(matches);
    setTimeout(() => {
      replaceEmptyCells(newGrid, matches);
      setRemovingCells([]);
    }, 500);
  };

  const replaceEmptyCells = (grid, matches) => {
    matches.forEach(({ row, col }) => {
      grid[row][col] = getRandomElement();
    });
    setGrid(grid);
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

    const matches = findMatches(row, col);
    if (matches.length >= 8) {
      handleMatch(matches);
    } else {
      newGrid[row][col] = draggedValue;
      newGrid[draggedItem.row][draggedItem.col] = newGrid[row][col];
      setGrid(newGrid);
    }

    setDraggedItem(null);
  };

  useEffect(() => {
    setGrid(generateGrid());
  }, []);

  return (
    <div className="grid">
      {grid.map((row, rowIndex) =>
        row.map((cell, colIndex) => (
          <div
            key={`${rowIndex}-${colIndex}`}
            className={`grid-cell ${removingCells.some(
              (item) => item.row === rowIndex && item.col === colIndex
            ) ? 'removing' : ''}`}
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