import { flipOnIndex } from './flipOnIndex';

describe('flipOnIndex', () => {
    it ('should return left for first two', () => {
        let result = flipOnIndex(0, true, false);
        expect(result).toBe(true);
        result = flipOnIndex(1, true, false);
        expect(result).toBe(true);
    });
    it ('should return right for second two', () => {
        let result = flipOnIndex(2, false, true);
        expect(result).toBe(true);
        result = flipOnIndex(3, false, true);
        expect(result).toBe(true);
    });
    it ('should return left for third two', () => {
        let result = flipOnIndex(4, true, false);
        expect(result).toBe(true);
        result = flipOnIndex(5, true, false);
        expect(result).toBe(true);
    });
    it ('should return right for fourth two', () => {
        let result = flipOnIndex(6, false, true);
        expect(result).toBe(true);
        result = flipOnIndex(7, false, true);
        expect(result).toBe(true);
    });
});