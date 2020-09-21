export const flipOnIndex = (ix, left, right, selected, container) => {
    if (selected) return container;
    const mod = ix % 4;
    if (mod === 0 || mod === 1)
        return left;
    return right;
}