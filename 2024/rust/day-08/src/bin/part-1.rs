use std::fs;

use day_08::part1;

fn main() {
    let content = fs::read_to_string("./input.txt").expect("Expecting input file.");
    println!("Part 1: {}", part1(&content));
}
