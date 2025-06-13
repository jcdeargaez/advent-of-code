use crate::direction::{Direction, DIRECTIONS};

#[derive(Debug, PartialEq)]
pub enum Item {
    Wall,
    Box,
    Empty,
}

pub struct Map {
    pub robot: (usize, usize),
    pub items: Vec<Vec<Item>>,
}

impl Map {
    pub fn new(robot: (usize, usize), items: Vec<Vec<Item>>) -> Self {
        Self { robot, items }
    }

    fn find_empty_item(&self, x: usize, y: usize, dir: Direction) -> Option<(usize, usize)> {
        let (dy, dx) = DIRECTIONS[dir as usize];
        let mut x = x as isize;
        let mut y = y as isize;
        loop {
            if 0 <= y && y < self.items.len() as isize &&
               0 <= x && x < self.items[y as usize].len() as isize {
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
        if y >= 0 && x >= 0 {
            let x = x as usize;
            let y = y as usize;
            if y < self.items.len() && x < self.items[y].len() {
                match self.items[y][x] {
                    Item::Empty => self.robot = (x, y),
                    Item::Wall => (),
                    Item::Box =>
                        if let Some((tx, ty)) = self.find_empty_item(x, y, dir) {
                            self.robot = (x, y);
                            self.items[y][x] = Item::Empty;
                            self.items[ty][tx] = Item::Box;
                        }
                }
            }
        }
    }

    pub fn print(&self) {
        println!("{}", "#".repeat(self.items[0].len() + 2));
        self.items
            .iter()
            .enumerate()
            .for_each(|(y, row)| {
                print!("#");
                row
                    .iter()
                    .enumerate()
                    .for_each(|(x, item)|
                        if (x, y) == self.robot {
                            print!("@");
                        } else {
                            match item {
                                Item::Wall  => print!("#"),
                                Item::Box   => print!("O"),
                                Item::Empty => print!("."),
                            }
                        });
                println!("#");
            });
        println!("{}", "#".repeat(self.items[0].len() + 2));
    }

    pub fn gps_score(&self) -> usize {
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
