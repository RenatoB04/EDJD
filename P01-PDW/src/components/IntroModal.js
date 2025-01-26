import React from "react";
import "../styles/IntroModal.css";

const IntroModal = ({ onClose }) => {
  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <h1>Bem-vindo Rusher!</h1>
        <p>
  O planeta enfrenta sérios riscos devido ao aumento da poluição e à má gestão de resíduos.  
  No Recycle Rush, a tua missão é travar o avanço do caos, separando corretamente os resíduos.  
  Ao reciclar, estarás a ajudar a reduzir a poluição e a restaurar o equilíbrio ambiental.  
</p>
<p>
  Cada erro diminui o tempo disponível. A barra de poluição reflete o esforço conjunto de TODOS os jogadores.  
  Dá o teu melhor para manter o planeta saudável!  
</p>
<p>
  Por cada resíduo colocado corretamente, ganhas 3 segundos e 10 pontos. No entanto, um erro retira 10 segundos.  
  A cada 5 acertos, a velocidade do jogo aumenta e a barra de poluição é reduzida em 1%.  
</p>
<p>
  No final do jogo, a tua pontuação será adicionada ao ranking geral e ao ranking de contribuições. Boa sorte!  
</p>
        <button onClick={onClose}>Começar</button>
      </div>
    </div>
  );
};

export default IntroModal;