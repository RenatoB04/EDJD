import React, { useState } from 'react';
import Grid from './components/Grid';
import Header from './components/Header';
import './styles/LevelOne.css';

function LevelOne() {
  const [score, setScore] = useState(0);

  const handleMatch = () => {
    setScore(score + 10);
  };

  return (
    <div className="level-container">
      <Header title="Nível 1: Restaurar a Vegetação" score={score} />
      <Grid onMatch={handleMatch} />
    </div>
  );
}

export default LevelOne;
