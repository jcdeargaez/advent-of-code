use itertools::{Either, Itertools};

fn parse_input(input: &str) -> (Vec<usize>, Vec<usize>) {
    input
        .lines()
        .flat_map(|line|
            line
                .split_whitespace()
                .map(|p| p.parse::<usize>().unwrap())
                .collect::<Vec<usize>>())
        .enumerate()
        .partition_map(|(i, v)| if i % 2 == 0 { Either::Left(v) } else { Either::Right(v) })
}

pub fn part1(input: &str) -> usize {
    let (mut lhs, mut rhs) = parse_input(input);
    lhs.sort();
    rhs.sort();
    let distance =
        lhs
            .iter()
            .zip(rhs.iter())
            .map(|(a, b)| a.abs_diff(*b))
            .sum();
    distance
}

pub fn part2(input: &str) -> usize {
    let (lhs, rhs) = parse_input(input);
    let f = rhs.iter().counts();
    let score  =
        lhs
            .iter()
            .map(|a| a * f.get(a).unwrap_or(&0))
            .sum();
    score
}

#[cfg(test)]
mod tests {
    use super::*;
    
    const INPUT: &str = "3   4
4   3
2   5
1   3
3   9
3   3
";

    #[test]
    fn test_part1() {
        let result = part1(INPUT);
        assert_eq!(result, 11);
    }

    #[test]
    fn test_part2() {
        let result = part2(INPUT);
        assert_eq!(result, 31);
    }
}
