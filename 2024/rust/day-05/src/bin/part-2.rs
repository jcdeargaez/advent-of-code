use std::fs;

use day_05::part2;

fn main() {
    let content = fs::read_to_string("./input.txt").unwrap();
    println!("Part 2: {}", part2(&content));
}
