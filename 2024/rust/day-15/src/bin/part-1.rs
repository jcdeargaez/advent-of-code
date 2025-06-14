use day_15::prelude::{part1, parse_input};

fn main() {
    let content = include_str!("../../input.txt");
    let (mut map, dirs) = parse_input(content);
    println!("Part 1: {}", part1(&mut map, &dirs));
}