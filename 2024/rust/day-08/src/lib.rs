use std::{
    collections::HashMap,
    iter::successors
};

use itertools::Itertools;

#[derive(Clone, Copy, Eq, Hash, PartialEq)]
struct Antinode {
    x: isize,
    y: isize,
}

struct Antenna {
    x: usize,
    y: usize,
}

struct Map {
    antennas: HashMap<char, Vec<Antenna>>,
    height: usize,
    width: usize,
}

struct _AntinodeRay {
    map: Map,
    source: Antenna,
    target: Option<Antenna>,
}

fn parse_input(input: &str) -> Map {
    let lines: Vec<&str> = input.lines().collect();
    let mut antennas = HashMap::new();
    for (y, line) in lines.iter().enumerate() {
        for (x, ch) in line.char_indices() {
            if ch != '.' {
                antennas
                    .entry(ch)
                    .or_insert(Vec::new())
                    .push(Antenna {x, y});
            }
        }
    }
    Map {
        antennas,
        height: lines.len(),
        width: lines[0].len(),
    }
}

impl Antenna {
    fn antinode(&self, other: &Self) -> Antinode {
        let dx = other.x as isize - self.x as isize;
        let dy = other.y as isize - self.y as isize;
        Antinode {
            x: other.x as isize + dx,
            y: other.y as isize + dy,
        }
    }

    fn antinodes(&self, other: &Self, map: &Map) -> Vec<Antinode> {
        let dx = other.x as isize - self.x as isize;
        let dy = other.y as isize - self.y as isize;
        let first = Antinode {
            x: self.x as isize,
            y: self.y as isize,
        };
        successors(Some(first), |&antinode| {
            let a = Antinode {
                x: antinode.x + dx,
                y: antinode.y + dy,
            };
            if map.in_range(&a) { Some(a) } else { None }
        }).collect::<Vec<Antinode>>()
    }
}

impl Map {
    fn in_range(&self, a: &Antinode) -> bool {
        0 <= a.x && a.x < self.width as isize && 0 <= a.y && a.y < self.height as isize
    }

    fn antinodes(&self, antennas: &Vec<Antenna>, antinode_projection: bool) -> Vec<Antinode> {
        antennas
            .iter()
            .enumerate()
            .flat_map(|(i, a)|
                antennas
                    .iter()
                    .enumerate()
                    .filter(move |(j, _b)| i != *j)
                    .map(move |(_j, b)| (a, b)))
            .flat_map(|(a, b)|
                if antinode_projection {
                    a.antinodes(b, self)
                } else {
                    vec![a.antinode(b)]
                })
            .filter(|p| self.in_range(p))
            .collect()
    }
}

pub fn part1(input: &str) -> usize {
    let map = parse_input(input);
    map.antennas
        .values()
        .flat_map(|antennas| map.antinodes(antennas, false))
        .unique()
        .count()
}

pub fn part2(input: &str) -> usize {
    let map = parse_input(input);
    map.antennas
        .values()
        .flat_map(|antennas| map.antinodes(antennas, true))
        .unique()
        .count()
}

#[cfg(test)]
mod tests {
    use super::*;

    const INPUT: &str = "............
........0...
.....0......
.......0....
....0.......
......A.....
............
............
........A...
.........A..
............
............
";

    #[test]
    fn test_part1() {
        let result = part1(INPUT);
        assert_eq!(result, 14);
    }

    #[test]
    fn test_part2() {
        let result = part2(INPUT);
        assert_eq!(result, 34);
    }
}
