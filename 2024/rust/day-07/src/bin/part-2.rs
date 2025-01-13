use std::fs;

use day_07::part2;

fn main() {
    let content = fs::read_to_string("./input.txt").expect("Expecting input file");
    println!("Part 2: {}", part2(&content));
}
