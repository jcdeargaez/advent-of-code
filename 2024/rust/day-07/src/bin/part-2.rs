use day_07::prelude::{parse_input, part2};

fn main() {
    let content = include_str!("../../input.txt");
    let input = parse_input(content);
    println!("Part 2: {}", part2(&input));
}