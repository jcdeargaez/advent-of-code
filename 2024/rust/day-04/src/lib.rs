/// (x, y) directions
const DIRECTIONS: [(isize, isize); 8] = [
    ( 0, -1), // Top
    (-1, -1), // Top left
    (-1,  0), // Left
    (-1,  1), // Bottom left
    ( 0,  1), // Bottom
    ( 1,  1), // Bottom right
    ( 1,  0), // Right
    ( 1, -1), // Top right
];

fn parse_input(input: &str) -> Vec<Vec<char>> {
    input
        .lines()
        .map(|line| line.chars().collect())
        .collect()
}

fn count_xmas(chars: &Vec<Vec<char>>, x: usize, y: usize) -> usize {
    let in_bounds = |(dx, dy): &&(isize, isize)| {
        let end_x = x as isize + 3 * dx;
        let end_y = y as isize + 3 * dy;
        0 <= end_x && end_x < chars[0].len() as isize && 0 <= end_y && end_y < chars.len() as isize
    };

    let has_xmas = |(dx, dy): &&(isize, isize)| {
        "XMAS"
            .char_indices()
            .all(|(i, c)| {
                let col = (x as isize + i as isize * dx) as usize;
                let row = (y as isize + i as isize * dy) as usize;
                c == chars[row][col]
            })
    };

    DIRECTIONS
        .iter()
        .filter(in_bounds)
        .filter(has_xmas)
        .count()
}

pub fn part1(input: &str) -> usize {
    let chars = parse_input(input);
    let height = chars.len();
    let width = chars[0].len();
    (0..height)
        .flat_map(|y| (0..width).map(|x| (x, y)).collect::<Vec<(usize, usize)>>())
        .filter(|(x, y)| chars[*y][*x] == 'X')
        .map(|(x, y)| count_xmas(&chars, x, y))
        .sum()
}

fn has_x_mas(chars: &Vec<Vec<char>>, x: usize, y: usize) -> bool {
    if chars[y][x] != 'A' { false } else {
        let tl = chars[y - 1][x - 1];
        let tr = chars[y - 1][x + 1];
        let bl = chars[y + 1][x - 1];
        let br = chars[y + 1][x + 1];
        ((tl == 'M' && br == 'S') || (tl == 'S' && br == 'M')) && ((tr == 'M' && bl == 'S') || (tr == 'S' && bl == 'M'))
    }
}

pub fn part2(input: &str) -> usize {
    let chars = parse_input(input);
    let height = chars.len();
    let width = chars[0].len();
    (1..height - 1)
        .flat_map(|y| (1..width - 1).map(|x| (x, y)).collect::<Vec<(usize, usize)>>())
        .filter(|(x, y)| has_x_mas(&chars, *x, *y))
        .count()
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part1() {
        let input = "....XXMAS.
.SAMXMS...
...S..A...
..A.A.MS.X
XMASAMX.MM
X.....XA.A
S.S.S.S.SS
.A.A.A.A.A
..M.M.M.MM
.X.X.XMASX
";
        let result = part1(input);
        assert_eq!(result, 18);
    }

    #[test]
    fn test_part2() {
        let input = ".M.S......
..A..MSMS.
.M.S.MAA..
..A.ASMSM.
.M.S.M....
..........
S.S.S.S.S.
.A.A.A.A..
M.M.M.M.M.
..........
";
        let result = part2(input);
        assert_eq!(result, 9);
    }
}
