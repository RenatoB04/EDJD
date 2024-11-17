import React from 'react';
import './Header.css';

function Header({ title, score }) {
  return (
    <div className="header">
      <h2>{title}</h2>
      <p>Pontuação: {score}</p>
    </div>
  );
}

export default Header;
