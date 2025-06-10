import React, { useState, useEffect } from 'react';
import { db } from '../firebase';
import { doc, onSnapshot } from 'firebase/firestore';
import '../styles/PollutionBar.css';

const PollutionBar = () => {
  const [pollutionLevel, setPollutionLevel] = useState(100);

  useEffect(() => {
    const unsubscribe = onSnapshot(doc(db, 'globalStats', 'pollutionBar'), (doc) => {
      if (doc.exists()) {
        const globalScore = doc.data().score || 10000;
        const pollutionPercentage = (globalScore / 10000) * 100;
        setPollutionLevel(pollutionPercentage);
        console.log('Valor da barra de poluição atualizado:', pollutionPercentage, '%');
      }
    });

    return () => unsubscribe();
  }, []);

  const getBarColor = () => {
    if (pollutionLevel > 70) return 'red';
    if (pollutionLevel > 30) return 'yellow';
    return 'green';
  };

  return (
    <div className="pollution-bar-container">
      <div
        className="pollution-bar"
        style={{
          height: `${pollutionLevel}%`,
          backgroundColor: getBarColor(),
        }}
      >
        <span className="pollution-value">
          {Math.round(pollutionLevel)}%
        </span>
      </div>
    </div>
  );
};

export default PollutionBar;
