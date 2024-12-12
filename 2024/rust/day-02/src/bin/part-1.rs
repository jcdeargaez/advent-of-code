use std::fs;

use day_02::part1;

fn main() {
    let content = fs::read_to_string("./input.txt").unwrap();
    println!("Part 1: {}", part1(&content));
}
