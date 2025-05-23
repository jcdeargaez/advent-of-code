mod parser {
    use nom::branch::alt;
    use nom::bytes::complete::tag;
    use nom::character::complete::{char, usize};
    use nom::combinator::map;
    use nom::sequence::delimited;
    use nom::{IResult, Parser};

    fn operands_pair(input: &str) -> IResult<&str, (usize, usize)> {
        let pnumbers = (usize, tag(","), usize);
        map(pnumbers, |(lhs, _, rhs)| (lhs, rhs)).parse(input)
    }

    pub fn mul(input: &str) -> IResult<&str, usize> {
        let pmul = delimited(tag("mul("), operands_pair, char(')'));
        map(pmul, |(a, b)| a * b).parse(input)
    }

    pub fn do_dont(input: &str) -> IResult<&str, bool> {
        let pdodont = alt((tag("don't"), tag("do")));
        map(pdodont, |dodont| dodont != "don't").parse(input)
    }
}

fn multiplications(s: &str, total: usize) -> usize {
    if s.is_empty() { total }
    else if let Ok((remaining, v)) = parser::mul(s) { multiplications(remaining, total + v) }
    else { multiplications(&s[1..], total) }
}

pub fn part1(input: &str) -> usize {
    multiplications(input, 0)
}

fn multiplications_with_do_dont(s: &str, total: usize, apply: bool) -> usize {
    if s.is_empty() { total }
    else if let Ok((remaining, new_apply)) = parser::do_dont(s) { multiplications_with_do_dont(remaining, total, new_apply) }
    else if let Ok((remaining, v)) = parser::mul(s) { multiplications_with_do_dont(remaining, total + if apply { v } else { 0 }, apply) }
    else { multiplications_with_do_dont(&s[1..], total, apply) }
}

pub fn part2(input: &str) -> usize {
    multiplications_with_do_dont(input, 0, true)
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part1() {
        let result = part1("xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))");
        assert_eq!(result, 161);
    }

    #[test]
    fn test_part2() {
        let result = part2("xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))");
        assert_eq!(result, 48);
    }
}
