use day_12::{parse_input, run};

fn main() {
    let content = include_str!("../input.txt");
    let lines = parse_input(&content);
    println!("Part 1: {}", run(&lines, false));
    println!("Part 2: {}", run(&lines, true));
}