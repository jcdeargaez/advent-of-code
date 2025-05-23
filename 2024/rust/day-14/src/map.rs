use crate::point::Point;
use crate::robot::Robot;

pub struct Map {
    pub width: usize,
    pub height: usize,
    pub robots: Vec<Robot>,
}

impl Map {
    fn new(width: usize, height: usize, robots: Vec<Robot>) -> Self {
        Self { width, height, robots }
    }

    pub fn small(robots: Vec<Robot>) -> Self {
        Self::new(11, 7, robots)
    }

    pub fn big(robots: Vec<Robot>) -> Self {
        Self::new(101, 103, robots)
    }

    pub fn safety_factor(&self, seconds: usize) -> usize {
        let mut quadrants = [0; 4];
        let hx = self.width / 2;
        let hy = self.height / 2;
        
        self.robots
            .iter()
            .map(|r| r.step_n(self.width, self.height, seconds))
            .filter_map(|p| p.quadrant(hx, hy))
            .for_each(|q| quadrants[q] += 1);

        quadrants
            .iter()
            .product()
    }

    pub fn xmas_tree(&self) -> usize {
        for second in 0..100_000 {
            let points: Vec<Point> = self.robots
                .iter()
                .map(|r| r.step_n(self.width, self.height, second))
                .collect();
    
            let mut frequency = vec![vec![0usize; self.width]; self.height];
            for p in points {
                frequency[p.y][p.x] += 1;
            }
    
            for y in 0..self.height-3 {
                for x in 0..self.width-3 {
                    let block = (y..y+3).all(|y| (x..x+3).all(|x| frequency[y][x] > 0));
                    if block {
                        self.print(&frequency);
                        return second
                    }
                }
            }
        }

        panic!("Christmas tree not found")
    }

    fn print(&self, frequency: &[Vec<usize>]) {
        for y in 0..self.height {
            for x in 0..self.width {
                match frequency[y][x] {
                    0 => print!(" "),
                    n => print!("{n}"),
                }
            }
            println!();
        }
    }
}