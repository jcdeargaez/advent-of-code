use itertools::Itertools;

fn parse_line(line: &str) -> Vec<isize> {
    line
        .split(' ')
        .map(|s| s.parse::<isize>().unwrap())
        .collect()
}

fn are_safe_reports(reports: &Vec<isize>) -> bool {
    let increasing = || {
        reports
            .iter()
            .tuple_windows()
            .all(|(a, b)| a < b && b - a <= 3)
    };
    let decreasing = || {
        reports
            .iter()
            .tuple_windows()
            .all(|(a, b)| a > b && a - b <= 3)
    };
    increasing() || decreasing()
}

pub fn part1(input: &str) -> usize {
    input
        .lines()
        .map(parse_line)
        .filter(are_safe_reports)
        .count()
}

fn is_problem_dampened(reports: &Vec<isize>) -> bool {
    let dampen = |i| {
        let mut dampened = reports.clone();
        dampened.remove(i);
        are_safe_reports(&dampened)
    };
    (0..reports.len())
        .any(dampen)
}

pub fn part2(input: &str) -> usize {
    input
        .lines()
        .map(parse_line)
        .filter(|reports| are_safe_reports(reports) || is_problem_dampened(reports))
        .count()
}

#[cfg(test)]
mod tests {
    use super::*;

    const INPUT: &str = "7 6 4 2 1
1 2 7 8 9
9 7 6 2 1
1 3 2 4 5
8 6 4 4 1
1 3 6 7 9
";

    #[test]
    fn test_part1() {
        let result = part1(INPUT);
        assert_eq!(result, 2);
    }

    #[test]
    fn test_part2() {
        let result = part2(INPUT);
        assert_eq!(result, 4);
    }
}
