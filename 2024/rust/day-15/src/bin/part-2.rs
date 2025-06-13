use day_15::prelude::{part2, parse_input};

fn main() {
    let content = include_str!("../../input.txt");
    let (mut map, dirs) = parse_input(content);
    println!("Part 2: {}", part2(&mut map, &dirs));
}