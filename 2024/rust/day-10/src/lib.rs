use std::collections::{HashSet, VecDeque};

const DIRS: [(isize, isize); 4] = [
    (-1,  0), // Up
    ( 0, -1), // Left
    ( 1,  0), // Down
    ( 0,  1), // Right
];

struct Map(Vec<Vec<usize>>);

impl Map {
    fn value(&self, row: usize, col: usize) -> usize {
        self.0[row][col]
    }

    fn height(&self) -> usize {
        self.0.len()
    }

    fn width(&self) -> usize {
        self.0[0].len()
    }

    fn score(&self, row: usize, col: usize) -> usize {
        let mut score = 0;
        let mut visited = HashSet::new();
        let mut pending = VecDeque::new();
        pending.push_back((row, col));
        while let Some(coord) = pending.pop_front() {
            if visited.contains(&coord) {
                continue;
            }
            let (y, x) = coord;
            let h = self.value(y, x);
            if h == 9 {
                score += 1;
            } else {
                let next_h = h + 1;
                for (dy, dx) in DIRS {
                    let next_y = y as isize + dy;
                    let next_x = x as isize + dx;
                    if next_y >= 0 && next_x >= 0 {
                        let next_y = next_y as usize;
                        let next_x = next_x as usize;
                        if next_y < self.height() && next_x < self.width() && self.value(next_y, next_x) == next_h {
                            pending.push_back((next_y, next_x));
                        }
                    }
                }
            }
            visited.insert(coord);
        }
        score
    }
    
    fn rating(&self, row: usize, col: usize) -> usize {
        let mut paths: Vec<Vec<usize>> = (0..self.height())
            .map(|_| vec![0; self.width()])
            .collect();
        let mut peaks = HashSet::new();
        let mut pending = VecDeque::new();
        pending.push_back((row, col));
        while let Some(coord) = pending.pop_front() {
            let (y, x) = coord;
            let h = self.value(y, x);
            paths[y][x] += 1;
            if h == 9 {
                peaks.insert(coord);
            } else {
                let next_h = h + 1;
                for (dy, dx) in DIRS {
                    let next_y = y as isize + dy;
                    let next_x = x as isize + dx;
                    if next_y >= 0 && next_x >= 0 {
                        let next_y = next_y as usize;
                        let next_x = next_x as usize;
                        if next_y < self.height() && next_x < self.width() && self.value(next_y, next_x) == next_h {
                            pending.push_back((next_y, next_x));
                        }
                    }
                }
            }
        }
        let rating = peaks
            .iter()
            .map(|&(y, x)| paths[y][x])
            .sum();
        rating
    }

    fn parse(input: &str) -> Self {
        let values = input
            .lines()
            .map(|line| line.as_bytes()
                .iter()
                .map(|&b| (b - b'0') as usize)
                .collect::<Vec<usize>>())
            .collect();
        Self(values)
    }
}

pub fn part1(input: &str) -> usize {
    let map = Map::parse(input);
    (0..map.height())
        .map(|row| (0..map.width())
            .filter(|&col| map.value(row, col) == 0)
            .map(|col| map.score(row, col))
            .sum::<usize>())
        .sum()
}

pub fn part2(input: &str) -> usize {
    let map = Map::parse(input);
    (0..map.height())
        .map(|row| (0..map.width())
            .filter(|&col| map.value(row, col) == 0)
            .map(|col| map.rating(row, col))
            .sum::<usize>())
        .sum()
}

#[cfg(test)]
mod tests {
    use super::*;

    const INPUT: &str = "89010123
78121874
87430965
96549874
45678903
32019012
01329801
10456732
";

    #[test]
    fn test_part1() {
        let result = part1(INPUT);
        assert_eq!(result, 36);
    }

    #[test]
    fn test_part2() {
        let result = part2(INPUT);
        assert_eq!(result, 81);
    }
}
