use day_15::prelude::*;

fn main() {
    let content = include_str!("../../input.txt");
    let (_, map) = parse_input(&content).expect("Expected parsed input");
    println!("Part 1: {}", run(&map, false));
    println!("Part 2: {}", run(&map, true));
}