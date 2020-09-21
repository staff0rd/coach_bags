import { flipOnIndex } from './flipOnIndex';

describe('flipOnIndex', () => {
    it ('should return true for first four', () => {
        let result = flipOnIndex(0, 4);
        expect(result).toBe(true);
        result = flipOnIndex(1, 4);
        expect(result).toBe(true);
        result = flipOnIndex(2, 4);
        expect(result).toBe(true);
        result = flipOnIndex(3, 4);
        expect(result).toBe(true);
    });
    it ('should return false for second four', () => {
        let result = flipOnIndex(4, 4);
        expect(result).toBe(false);
        result = flipOnIndex(5, 4);
        expect(result).toBe(false);
        result = flipOnIndex(6, 4);
        expect(result).toBe(false);
        result = flipOnIndex(7, 4);
        expect(result).toBe(false);
    });
});