import React from 'react';

export const CardType = {
  Prompt: 'prompt',
  Response: 'response'
} as const;

export type CardType = typeof CardType[keyof typeof CardType];

interface CardProps {
  type: CardType;
  text: string;
}

const Card: React.FC<CardProps> = ({ type, text }) => {
  const isPrompt = type === CardType.Prompt;
  
  return (
    <div className={`game-card shadow-lg mb-3 ${isPrompt ? 'black-bg' : 'white-bg'}`}>
      <div className="card-body">
        <h5 className="card-title">{text}</h5>
      </div>
    </div>
  );
};

export default Card;
