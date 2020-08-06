import React from 'react';
export const Currency = ({ value }) => {
  return (
    <span>${value.toFixed(2)}</span>
  );
};
