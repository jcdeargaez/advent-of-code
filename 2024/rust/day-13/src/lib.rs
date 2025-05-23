mod parser;

/// Solves x and y in the equations ax + by = c and dx + ey = f
fn solve(a: isize, b: isize, c: isize, d: isize, e: isize, f: isize) -> Option<(isize, isize)> {
    let y = (a*f - d*c) / (a*e - d*b);
    let x = (c - b*y) / a;
    if a*x + b*y == c && d*x + e*y == f {
        Some((x, y))
    } else {
        None
    }
}

pub fn run(input: &[(isize, isize, isize, isize, isize, isize)], prize_offset: bool) -> usize {
    input
        .iter()
        .filter_map(|&(a, b, c, d, e, f)|
            if prize_offset {
                solve(a, b, c + 10_000_000_000_000, d, e, f + 10_000_000_000_000)
            } else {
                solve(a, b, c, d, e, f)
            })
        .map(|(a, b)| a as usize * 3 + b as usize)
        .sum()
}

pub fn part1(input: &str) -> usize {
    let (_, arcades) = parser::parse_input(input).expect("Expecting parsed input");
    run(&arcades, false)
}

pub fn part2(input: &str) -> usize {
    let (_, arcades) = parser::parse_input(input).expect("Expecting parsed input");
    run(&arcades, true)
}

#[cfg(test)]
mod tests {
    use super::*;

    const INPUT: &str = "Button A: X+94, Y+34
Button B: X+22, Y+67
Prize: X=8400, Y=5400

Button A: X+26, Y+66
Button B: X+67, Y+21
Prize: X=12748, Y=12176

Button A: X+17, Y+86
Button B: X+84, Y+37
Prize: X=7870, Y=6450

Button A: X+69, Y+23
Button B: X+27, Y+71
Prize: X=18641, Y=10279
";

    #[test]
    fn test_part1() {
        let result = part1(INPUT);
        assert_eq!(result, 480);
    }
}
