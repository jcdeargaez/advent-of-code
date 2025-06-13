use crate::{direction::Direction, map::{Item, Map}};

fn parse_directions(input: &str) -> Vec<Direction> {
    input
        .lines()
        .skip_while(|&line| line.len() > 0)
        .skip(1)
        .map(|line| line
            .as_bytes()
            .iter()
            .map(|&b| Direction::from(b))
            .collect::<Vec<Direction>>())
        .flatten()
        .collect()
}

fn parse_map(input: &str) -> Map {
    let lines: Vec<&str> = input
        .lines()
        .take_while(|&line| line.len() > 0)
        .collect();

    let mut robot = (0, 0);
    let items: Vec<Vec<Item>> = (1..lines.len()-1)
        .map(|y| {
            let line = lines[y].as_bytes();
            (1..line.len()-1)
                .map(|x|
                    match line[x] {
                        b'.' => Item::Empty,
                        b'#' => Item::Wall,
                        b'O' => Item::Box,
                        b'@' => {
                            robot = (x - 1, y - 1);
                            Item::Empty
                        },
                        x => panic!("Invalid character in map '{x}'")
                    })
                .collect()
        })
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
    fn test_parse_input() {
        let input = "\
####
#.O#
#@.#
####

^>
v<";
        let (map, dirs) = parse_input(input);
        
        let expected_map_robot = (0, 1);
        let expected_map_items = vec![
            vec![Item::Empty, Item::Box],
            vec![Item::Empty, Item::Empty],
        ];
        let expected_dirs = vec![Direction::Up, Direction::Right, Direction::Down, Direction::Left];

        assert_eq!(map.robot, expected_map_robot, "Robot position does not match expected");
        assert_eq!(map.items, expected_map_items, "Map items do not match expected");
        assert_eq!(dirs, expected_dirs, "Directions do not match expected");
    }
}