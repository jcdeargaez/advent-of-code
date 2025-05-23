use crate::point::Point;

#[derive(Debug, PartialEq)]
struct Velocity {
    dx: isize,
    dy: isize,
}

#[derive(Debug, PartialEq)]
pub struct Robot {
    initial_position: Point,
    velocity: Velocity,
}

impl Robot {
    pub fn new(x: usize, y: usize, dx: isize, dy: isize) -> Self {
        Self {
            initial_position: Point::new(x, y),
            velocity: Velocity { dx, dy }
        }
    }    

    pub fn step_n(&self, width: usize, height: usize, seconds: usize) -> Point {
        let x = (self.velocity.dx * seconds as isize + self.initial_position.x as isize).rem_euclid(width as isize);
        let y = (self.velocity.dy * seconds as isize + self.initial_position.y as isize).rem_euclid(height as isize);
        Point::new(x as usize, y as usize)
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn straight_move() {
        let r = Robot::new(2, 4, 2, -3);
        let p = r.step_n(11, 7, 1);
        assert_eq!(p, Point { x: 4, y: 1 });
    }

    #[test]
    fn teleport_move() {
        let r = Robot::new(2, 4, 2, -3);
        let p = r.step_n(11, 7, 2);
        assert_eq!(p, Point { x: 6, y: 5 });
    }
}