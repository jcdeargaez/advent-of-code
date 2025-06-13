use crate::equation::Equation;

pub fn part1(input: &[Equation]) -> usize {
    input
        .iter()
        .filter(|e| e.has_solution(false))
        .map(|e| e.test_value)
        .sum()
}

pub fn part2(input: &[Equation]) -> usize {
    input
        .iter()
        .filter(|e| e.has_solution(true))
        .map(|e| e.test_value)
        .sum()
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::parser;

    const INPUT: &str = "\
190: 10 19
3267: 81 40 27
83: 17 5
156: 15 6
7290: 6 8 6 15
161011: 16 10 13
192: 17 8 14
21037: 9 7 18 13
292: 11 6 16 20
";

    #[test]
    fn test_part1() {
        let result = part1(&parser::parse_input(INPUT));
        assert_eq!(result, 3749);
    }

    #[test]
    fn test_part2() {
        let result = part2(&parser::parse_input(INPUT));
        assert_eq!(result, 11387);
    }
}