mod direction;
mod map;
mod parser;
mod point;
mod robot;
mod simulation;

pub mod prelude {
    pub use crate::parser::parse_input;
    pub use crate::simulation::{run, part1, part2};
}