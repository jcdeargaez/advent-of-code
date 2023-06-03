use std::collections::hash_set::{HashSet};
use std::fs;
use std::str::Lines;
use std::time::Instant;

use itertools::Itertools;

fn priority(item: u8) -> usize {
    (item - if item.is_ascii_uppercase() { b'@' - 26 } else { b'`' }) as usize
}

fn common_items_priority(containers: Vec<&str>) -> usize {
    containers.into_iter()
        .map(|container| container.bytes().collect::<HashSet<u8>>())
        .reduce(|acc, set| acc.intersection(&set).map(|item| *item).collect::<HashSet<u8>>())
        .map(|common_set| common_set.into_iter().map(priority).sum())
        .expect("Didn't find a common item")
}

fn part1(lines: Lines) -> usize {
    lines
        .map(|line| {
            let (c1, c2) = line.split_at(line.len() / 2);
            common_items_priority(vec![c1, c2])
        })
        .sum()
}

fn part2(lines: Lines) -> usize {
    lines
        .tuples()
        .map(|(line1, line2, line3)| common_items_priority(vec![line1, line2, line3]))
        .sum()
}

fn main() {
    let start = Instant::now();
    if let Ok(content) = fs::read_to_string("../input.txt") {
        println!("Part 1: {}", part1(content.lines()));
        println!("Part 2: {}", part2(content.lines()));
    }
    let duration = start.elapsed();
    println!("{:?}", duration);
}
