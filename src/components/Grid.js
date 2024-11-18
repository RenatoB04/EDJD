import React, { useState, useEffect } from 'react';
import './Grid.css';

function Grid() {
  const symbols = ['🌱', '💧', '🌍', '🌞', '🍃', '♻️', '🌳', '🌾', '🐝', '⚡'];
  const gridRows = 6;
  const gridCols = 5;
  const [grid, setGrid] = useState([]);
  const [draggedItem, setDraggedItem] = useState(null);
  const [removingCells, setRemovingCells] = useState([]);

  const generateGrid = () => {
    let newGrid;
    do {
      newGrid = Array.from({ length: gridRows }, () =>
        Array.from({ length: gridCols }, () => getRandomElement())
      );
    } while (hasInitialMatches(newGrid));
    return newGrid;
  };

  const getRandomElement = () => {
    return symbols[Math.floor(Math.random() * symbols.length)];
  };

  const hasInitialMatches = (grid) => {
    for (let row = 0; row < gridRows; row++) {
      for (let col = 0; col < gridCols; col++) {
        const matches = findMatchesInDirection(grid, row, col, 'horizontal', 4);
        const matchesVert = findMatchesInDirection(grid, row, col, 'vertical', 4);
        if (matches.length >= 4 || matchesVert.length >= 4) {
          return true;
        }
      }
    }
    return false;
  };

  const findMatches = () => {
    const matches = [];

    for (let row = 0; row < gridRows; row++) {
      for (let col = 0; col < gridCols; col++) {
        const horizontalMatch = findMatchesInDirection(grid, row, col, 'horizontal', 4);
        const verticalMatch = findMatchesInDirection(grid, row, col, 'vertical', 4);

        if (horizontalMatch.length >= 4) {
          matches.push(...horizontalMatch);
        }

        if (verticalMatch.length >= 4) {
          matches.push(...verticalMatch);
        }
      }
    }
    return matches;
  };

  const findMatchesInDirection = (grid, row, col, direction, minLength) => {
    const symbol = grid[row][col];
    const cells = [];
    let r = row;
    let c = col;

    while (
      r >= 0 &&
      r < gridRows &&
      c >= 0 &&
      c < gridCols &&
      grid[r][c] === symbol
    ) {
      cells.push({ row: r, col: c });
      if (direction === 'horizontal') c++;
      if (direction === 'vertical') r++;
    }

    return cells.length >= minLength ? cells : [];
  };

  const handleSwap = (sourceRow, sourceCol, targetRow, targetCol) => {
    const newGrid = [...grid];
    const temp = newGrid[sourceRow][sourceCol];
    newGrid[sourceRow][sourceCol] = newGrid[targetRow][targetCol];
    newGrid[targetRow][targetCol] = temp;

    setGrid(newGrid);

    const matches = findMatches();

    if (matches.length > 0) {
      handleMatch(matches);
    } else {
      setGrid(newGrid);
    }
  };

  const handleMatch = (matches) => {
    const newGrid = [...grid];

    matches.forEach(({ row, col }) => {
      newGrid[row][col] = '';
    });

    setRemovingCells(matches);
    setTimeout(() => {
      dropSymbols(newGrid);
      setRemovingCells([]);
    }, 500);
  };

  const dropSymbols = (grid) => {
    for (let col = 0; col < gridCols; col++) {
      let emptySpaces = 0;
      for (let row = gridRows - 1; row >= 0; row--) {
        if (grid[row][col] === '') {
          emptySpaces++;
        } else if (emptySpaces > 0) {
          grid[row + emptySpaces][col] = grid[row][col];
          grid[row][col] = '';
        }
      }

      for (let row = 0; row < emptySpaces; row++) {
        grid[row][col] = getRandomElement();
      }
    }
    setGrid(grid);
  };

  const handleDragStart = (row, col) => {
    setDraggedItem({ row, col });
  };

  const handleDragOver = (e) => {
    e.preventDefault();
  };

  const handleDrop = (row, col) => {
    if (!draggedItem) return;

    const { row: sourceRow, col: sourceCol } = draggedItem;

    if (
      (Math.abs(sourceRow - row) === 1 && sourceCol === col) ||
      (Math.abs(sourceCol - col) === 1 && sourceRow === row)
    ) {
      handleSwap(sourceRow, sourceCol, row, col);
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
            onDragOver={handleDragOver}
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