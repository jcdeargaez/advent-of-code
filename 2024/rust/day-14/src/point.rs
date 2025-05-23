#[derive(Eq, Debug, Hash, PartialEq)]
pub struct Point {
    pub x: usize,
    pub y: usize,
}

impl Point {
    pub fn new(x: usize, y: usize) -> Self {
        Self { x, y }
    }    

    pub fn quadrant(&self, hx: usize, hy: usize) -> Option<usize> {
        if self.x == hx || self.y == hy {
            None
        } else {
            let left_x = self.x < hx;
            let top_y = self.y < hy;
            match (left_x, top_y) {
                (true, true)   => Some(0),
                (false, true)  => Some(1),
                (true, false)  => Some(2),
                (false, false) => Some(3),
            }
        }
    }
}
