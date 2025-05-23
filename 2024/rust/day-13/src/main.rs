use day_13::run;

mod parser;

fn main() {
    let content = include_str!("../input.txt");
    let (_, arcades) = parser::parse_input(&content).expect("Expected parsed input");
    println!("Part 1: {}", run(&arcades, false));
    println!("Part 2: {}", run(&arcades, true));
}