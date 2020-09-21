export const flipOnIndex = (tiles, width) => {
    const row = Math.floor(tiles / width);
    const rightAlign = row % 2 === 0;
    return rightAlign;
}