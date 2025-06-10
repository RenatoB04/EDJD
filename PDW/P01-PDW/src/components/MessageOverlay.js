import React from "react";
import "../styles/Game.css";

const messages = [
  "Sabias que reciclar uma tonelada de papel poupa cerca de 17 árvores?",
  "Reciclar alumínio consome 95% menos energia do que produzir alumínio novo.",
  "O vidro pode ser reciclado infinitamente sem perder qualidade.",
  "Cada tonelada de plástico reciclado poupa 7,4 metros cúbicos de espaço em aterros.",
  "Reciclar uma garrafa de plástico poupa energia suficiente para alimentar uma lâmpada durante 6 horas.",
  "Papel reciclado usa 70% menos energia do que papel novo.",
  "Um litro de óleo usado pode contaminar até um milhão de litros de água.",
  "Reciclar 50 kg de papel evita a emissão de cerca de 75 kg de CO2.",
  "As tampas de plástico devem ser separadas das garrafas antes da reciclagem.",
  "Reciclar uma lata de bebida pode poupar energia suficiente para uma televisão funcionar durante 3 horas.",
  "O plástico leva até 500 anos a decompor-se na natureza.",
  "Cada tonelada de vidro reciclado reduz a poluição do ar em 20%.",
  "Reciclar metais evita a extração de novos recursos naturais.",
  "Cerca de 80% das embalagens de papel são recicláveis.",
  "A reciclagem de eletrónicos previne a contaminação do solo com metais pesados.",
  "Reciclar um telemóvel evita a necessidade de extrair 75 kg de minérios.",
  "Pilhas devem ser recicladas em locais apropriados devido ao risco de contaminação.",
  "Em Portugal, cerca de 30% dos resíduos urbanos ainda não são reciclados.",
  "A reciclagem contribui para a redução das emissões de gases com efeito de estufa.",
  "Separar corretamente os resíduos facilita a reciclagem e reduz custos de processamento."
];

const MessageOverlay = ({ messageIndex }) => {
  return (
    <div className="toast">
      <p>{messages[messageIndex]}</p>
    </div>
  );
};

export default MessageOverlay;
