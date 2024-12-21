use std::collections::HashSet;

const DIRECTIONS: [(isize, isize); 4] = [
    ( 0, -1), // Up
    ( 1,  0), // Right
    ( 0,  1), // Down
    (-1,  0), // Left
];

fn parse_input(input: &str) -> (Vec<Vec<char>>, (usize, usize)) {
    let map =
        input
            .lines()
            .map(|line| line.chars().collect::<Vec<char>>())
            .collect::<Vec<Vec<char>>>();

    let mut guard = (0, 0);
    for row in 0..map.len() {
        for col in 0..map[0].len() {
            if map[row][col] == '^' {
                guard = (row, col);
            }
        }
    }

    (map, guard)
}

fn next_point(map: &Vec<Vec<char>>, (row, col): (usize, usize), dir: usize) -> Option<(usize, usize)> {
    let (dx, dy) = DIRECTIONS[dir];
    let height = map.len() as isize;
    let width = map[0].len() as isize;
    let next_row = row as isize + dy;
    let next_col = col as isize + dx;
    if 0 <= next_row && next_row < height && 0 <= next_col && next_col < width {
        Some((next_row as usize, next_col as usize))
    } else {
        None
    }
}

fn simulate_path(map: &Vec<Vec<char>>, initial_position: (usize, usize)) -> HashSet<(usize, usize)> {
    let mut path = HashSet::new();
    let mut guard = initial_position;
    let mut dir = 0;
    path.insert(initial_position);
    while let Some(p) = next_point(map, guard, dir) {
        if map[p.0][p.1] == '#' {
            dir = (dir + 1) % 4;
        } else {
            path.insert(p);
            guard = p;
        }
    }
    path
}

pub fn part1(input: &str) -> usize {
    let (map, guard) = parse_input(input);
    let path = simulate_path(&map, guard);
    path.len()
}

fn has_loop(map: &Vec<Vec<char>>, obstruction: (usize, usize), initial_position: (usize, usize)) -> bool {
    let mut guard = initial_position;
    let mut dir = 0;
    let mut visited = HashSet::new();
    visited.insert((initial_position, dir));
    while let Some(p) = next_point(map, guard, dir) {
        if map[p.0][p.1] == '#' || p == obstruction {
            dir = (dir + 1) % 4;
        } else {
            if !visited.insert((p, dir)) {
                return true;
            }
            guard = p;
        }
    }
    false
}

pub fn part2(input: &str) -> usize {
    let (map, guard) = parse_input(input);
    let mut path = simulate_path(&map, guard);
    path.remove(&guard);
    path
        .iter()
        .filter(|&&p| has_loop(&map, p, guard))
        .count()
}

#[cfg(test)]
mod tests {
    use super::*;

    const INPUT: &str = "....#.....
.........#
..........
..#.......
.......#..
..........
.#..^.....
........#.
#.........
......#...
";

    #[test]
    fn test_part1() {
        let result = part1(INPUT);
        assert_eq!(result, 41);
    }

    #[test]
    fn test_part2() {
        let result = part2(INPUT);
        assert_eq!(result, 6);
    }
}
