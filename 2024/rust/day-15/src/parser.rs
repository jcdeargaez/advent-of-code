use crate::{direction::Direction, map::{Item, Map}};

fn parse_directions(input: &str) -> Vec<Direction> {
    let input: String = input
        .lines()
        .skip_while(|&line| line.len() > 0)
        .skip(1)
        .take_while(|_| true)
        .collect();

    input
        .as_bytes()
        .iter()
        .map(|&b| Direction::from(b))
        .collect()
}

fn parse_map(input: &str) -> Map {
    let input: String = input
        .lines()
        .take_while(|&line| line.len() > 0)
        .collect();

    let lines: Vec<&str> = input
        .lines()
        .collect();

    let mut robot = (0, 0);
    let items: Vec<Vec<Item>> = (1..lines.len() - 1)
        .map(|y|
            lines[y]
                .as_bytes()
                .iter()
                .enumerate()
                .map(|(x, &b)|
                    match b {
                        b'.' => Item::Empty,
                        b'#' => Item::Wall,
                        b'O' => Item::Box,
                        b'@' => {
                            robot = (x, y);
                            Item::Empty
                        },
                        x => panic!("Invalid character in map '{x}'")
                    })
                .collect())
        .collect();

    Map::new(robot, items)
}

pub fn parse_input(input: &str) -> (Map, Vec<Direction>) {    
    let map = parse_map(input);
    let dirs = parse_directions(input);
    (map, dirs)
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn 
}