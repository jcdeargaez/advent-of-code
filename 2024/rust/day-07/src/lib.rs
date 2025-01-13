use std::collections::VecDeque;

struct Equation {
    test_value: u64,
    values: Vec<u64>
}

mod parser {
    use nom::{
        bytes::complete::tag,
        character::complete::{char, digit1, newline},
        combinator::{eof, map_res},
        multi::{many1, separated_list1},
        sequence::{separated_pair, terminated}, IResult
    };

    use crate::Equation;
    
    fn parse_number(input: &str) -> IResult<&str, u64> {
        map_res(digit1, str::parse)(input)
    }

    fn parse_equation(input: &str) -> IResult<&str, Equation> {
        let (input, (test_value, values)) = separated_pair(parse_number, tag(": "), separated_list1(char(' '), parse_number))(input)?;
        Ok((input, Equation { test_value, values }))
    }

    pub fn parse_input(input: &str) -> Vec<Equation> {
        let pequation_nl = terminated(parse_equation, newline);
        let (_input, equations) = terminated(many1(pequation_nl), eof)(input).expect("Expected valid equations input");
        equations
    }
}

impl Equation {
    fn has_solution(&self, concat_op: bool) -> bool {
        let mut pending = VecDeque::new();
        pending.push_back((1, self.values[0]));
        while !pending.is_empty() {
            let (i, total_so_far) = pending.pop_front().expect("Expected item from queue");
            if i == self.values.len() {
                if total_so_far == self.test_value {
                    return true
                }
            } else if total_so_far <= self.test_value {
                let v = self.values[i];
                pending.push_back((i + 1, total_so_far * v));
                pending.push_back((i + 1, total_so_far + v));
                if concat_op {
                    pending.push_back((i + 1, format!("{}{}", total_so_far, v).parse::<u64>().expect("Expected a number to parse")));
                }
            }
        }
        false
    }
}

pub fn part1(input: &str) -> u64 {
    parser::parse_input(input)
        .iter()
        .filter(|e| e.has_solution(false))
        .map(|e| e.test_value)
        .sum()
}

pub fn part2(input: &str) -> u64 {
    parser::parse_input(input)
        .iter()
        .filter(|e| e.has_solution(true))
        .map(|e| e.test_value)
        .sum()
}

#[cfg(test)]
mod tests {
    use super::*;

    const INPUT: &str = "190: 10 19
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
        let result = part1(INPUT);
        assert_eq!(result, 3749);
    }

    #[test]
    fn test_part2() {
        let result = part2(INPUT);
        assert_eq!(result, 11387);
    }
}
