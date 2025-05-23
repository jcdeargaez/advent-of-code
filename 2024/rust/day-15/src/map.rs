use crate::direction::{Direction, DIRECTIONS};

pub enum Item {
    Wall,
    Box,
    Empty,
}

pub struct Map {
    robot: (usize, usize),
    items: Vec<Vec<Item>>
}

impl Map {
    pub fn new(robot: (usize, usize), items: Vec<Vec<Item>>) -> Self {
        Self { robot, items }
    }

    fn find_empty(&self, x: usize, y: usize, dir: Direction) -> Option<(usize, usize)> {
        let (dy, dx) = DIRECTIONS[dir as usize];
        let mut x = x as isize;
        let mut y = y as isize;
        loop {
            if 1 <= y && y < (self.items.len() - 1) as isize &&
               1 <= x && x < (self.items[y as usize].len() - 1) as isize {
                match self.items[y as usize][x as usize] {
                    Item::Empty => return Some((x as usize, y as usize)),
                    Item::Wall => return None,
                    Item::Box => {
                        x += dx;
                        y += dy;
                    }
                }
            } else {
                return None;
            }
        }
    }

    pub fn attempt_move(&mut self, dir: Direction) {
        let (dy, dx) = DIRECTIONS[dir as usize];
        let x = self.robot.0 as isize + dx;
        let y = self.robot.1 as isize + dy;
        if y >= 1 && x >= 1 {
            let x = x as usize;
            let y = y as usize;
            if y < self.items.len() - 1 && x < self.items[y].len() - 1 {
                match self.items[y][x] {
                    Item::Empty => self.robot = (x, y),
                    Item::Wall => (),
                    Item::Box =>
                        if let Some((tx, ty)) = self.find_empty(x, y, dir) {
                            self.robot = (x, y);
                            self.items[y][x] = Item::Empty;
                            self.items[ty][tx] = Item::Box;
                        }
                }
            }
        }
    }

    pub fn gps(&self) -> usize {
        self.items
            .iter()
            .enumerate()
            .map(|(y, row)|
                row
                .iter()
                .enumerate()
                .map(|(x, item)|
                    match item {
                        Item::Box => (y + 1) * 100 + x + 1,
                        _ => 0
                    })
                .sum::<usize>())
            .sum()
    }
}
