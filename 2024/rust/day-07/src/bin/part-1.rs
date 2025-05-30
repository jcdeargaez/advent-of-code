use day_07::prelude::{parse_input, part1};

fn main() {
    let content = include_str!("../../input.txt");
    let input = parse_input(content);
    println!("Part 1: {}", part1(&input));
}