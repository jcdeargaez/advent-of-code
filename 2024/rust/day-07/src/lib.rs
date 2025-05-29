mod equation;
mod parser;
mod solver;

pub mod prelude {
    pub use crate::parser::parse_input;
    pub use crate::solver::{part1, part2};
}