use day_07::prelude::{parse_input, part1, part2};

fn main() {
    let content = include_str!("../../input.txt");
    let input = parse_input(content);
    println!("Part 1: {}", part1(&input));
    println!("Part 2: {}", part2(&input));
}