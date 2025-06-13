mod direction;
mod map;
mod parser;
mod simulation;

pub mod prelude {
    pub use crate::parser::parse_input;
    pub use crate::simulation::{part1, part2};
}