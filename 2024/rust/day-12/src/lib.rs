use std::collections::{HashSet, VecDeque};

struct Region {
    area: usize,
    perimeter: usize,
    sides: usize,
}

#[derive(Clone, Copy, Eq, Hash, PartialEq)]
struct Point {
    x: usize,
    y: usize,
}

const DIRS: [(isize, isize); 4] = [
    (-1,  0), // Up
    ( 0, -1), // Left
    ( 1,  0), // Down
    ( 0,  1), // Right
];

const BORDER: [(isize, isize); 8] = [
    (-1, -1), // Upper left
    (-1,  0), // Up
    (-1,  1), // Upper right
    ( 0,  1), // Right
    ( 1,  1), // Lower right
    ( 1,  0), // Down
    ( 1, -1), // Lower left
    ( 0, -1), // Left
];

impl Point {
    fn perimeter(neighbors: usize) -> usize {
        if neighbors > 4 {
            panic!("Unexpected neighbors {neighbors}")
        }
        4 - neighbors
    }

    fn corners(border: &[Option<Point>]) -> usize {
        let mut corners = 0;
        
        // when adjacents are different, the opposite does not matter
        if border[7].is_none() && border[1].is_none() {
            corners += 1;
        }
        if border[1].is_none() && border[3].is_none() {
            corners += 1;
        }
        if border[3].is_none() && border[5].is_none() {
            corners += 1;
        }
        if border[5].is_none() && border[7].is_none() {
            corners += 1;
        }

        // when adjacents are equal, the opposite must be different
        if border[7].is_some() && border[0].is_none() && border[1].is_some() {
            corners += 1;
        }
        if border[1].is_some() && border[2].is_none() && border[3].is_some() {
            corners += 1;
        }
        if border[3].is_some() && border[4].is_none() && border[5].is_some() {
            corners += 1;
        }
        if border[5].is_some() && border[6].is_none() && border[7].is_some() {
            corners += 1;
        }

        corners
    }

    fn neighbors(&self, lines: &[&[u8]]) -> Vec<Self> {
        let plant = lines[self.y][self.x];
        DIRS
            .iter()
            .filter_map(|&(dy, dx)| {
                let x = self.x as isize + dx;
                let y = self.y as isize + dy;
                if y >= 0 && x >= 0 {
                    let x = x as usize;
                    let y = y as usize;
                    if y < lines.len() && x < lines[0].len() && lines[y][x] == plant {
                        return Some(Point { x, y });
                    }
                }
                None
            }).collect()
    }

    fn border(&self, lines: &[&[u8]]) -> Vec<Option<Point>> {
        let plant = lines[self.y][self.x];
        BORDER
            .iter()
            .map(|&(dy, dx)| {
                let x = self.x as isize + dx;
                let y = self.y as isize + dy;
                if y < 0 || x < 0 || y >= lines.len() as isize || x >= lines[0].len() as isize {
                    None
                } else {
                    let x = x as usize;
                    let y = y as usize;
                    if lines[y][x] != plant {
                        None
                    } else {
                        Some(Point { x, y })
                    }
                }
            })
            .collect()
    }
}

impl Region {
    fn new(lines: &[&[u8]], initial_position: Point, visited: &mut HashSet<Point>) -> Region {
        let mut q = VecDeque::new();
        let mut perimeter = 0;
        let mut area = 0;
        let mut corners = 0;
        q.push_back(initial_position);
        while let Some(p) = q.pop_front() {
            if !visited.insert(p) {
                continue;
            }

            let neighbors = p.neighbors(lines);
            perimeter += Point::perimeter(neighbors.len());
            area += 1;
    
            let border = p.border(lines);
            corners += Point::corners(&border);

            neighbors
                .iter()
                .filter(|&n| !visited.contains(n))
                .for_each(|&n| q.push_back(n));
        }

        Region { area, perimeter, sides: corners }
    }

    fn price(&self, discount: bool) -> usize {
        let f = if discount { self.sides } else { self.perimeter };
        f * self.area
    }
}

pub fn parse_input(input: &str) -> Vec<&[u8]> {
    input
        .lines()
        .map(|line| line.as_bytes())
        .collect()
}

pub fn run(lines: &[&[u8]], discount: bool) -> usize {    
    let mut visited = HashSet::new();

    (0..lines.len())
        .flat_map(|y|
            (0..lines[y].len())
                .map(move |x| Point { x, y }))
        .filter_map(|p|
            if visited.contains(&p) {
                None
            } else {
                let r = Region::new(&lines, p, &mut visited);
                Some(r)
            })
        .map(|r| r.price(discount))
        .sum()
}

pub fn part1(input: &str) -> usize {
    let lines = parse_input(input);
    run(&lines, false)
}

pub fn part2(input: &str) -> usize {
    let lines = parse_input(input);
    run(&lines, true)
}

#[cfg(test)]
mod tests {
    use super::*;

    const INPUT: &str = "RRRRIICCFF
RRRRIICCCF
VVRRRCCFFF
VVRCCCJFFF
VVVVCJJCFE
VVIVCCJJEE
VVIIICJJEE
MIIIIIJJEE
MIIISIJEEE
MMMISSJEEE
";

    #[test]
    fn test_part1() {
        let result = part1(INPUT);
        assert_eq!(result, 1930);
    }

    #[test]
    fn test_part2() {
        let result = part2(INPUT);
        assert_eq!(result, 1206);
    }
}
