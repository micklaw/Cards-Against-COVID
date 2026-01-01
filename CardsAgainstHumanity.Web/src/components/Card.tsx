import React from 'react';

export enum CardType {
  Prompt = 'prompt',
  Response = 'response'
}

interface CardProps {
  type: CardType;
  text: string;
}

const Card: React.FC<CardProps> = ({ type, text }) => {
  const className = type === CardType.Prompt 
    ? 'card text-white black-bg' 
    : 'card white-bg';

  return (
    <div className={`shadow ${className} mb-3`}>
      <div className="card-body text-left">
        <h5 className="card-title">{text}</h5>
      </div>
    </div>
  );
};

export default Card;
