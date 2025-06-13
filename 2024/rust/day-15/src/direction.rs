#[derive(Clone, Copy, Debug, PartialEq)]
pub enum Direction {
    Up,
    Left,
    Down,
    Right,
}

pub const DIRECTIONS: [(isize, isize); 4] = [
    (-1,  0),
    ( 0, -1),
    ( 1,  0),
    ( 0,  1),
];

impl Direction {
    pub fn from(b: u8) -> Direction {
        match b {
            b'^' => Direction::Up,
            b'<' => Direction::Left,
            b'v' => Direction::Down,
            b'>' => Direction::Right,
            x => panic!("Invalid direction '{x}'")
        }
    }
}