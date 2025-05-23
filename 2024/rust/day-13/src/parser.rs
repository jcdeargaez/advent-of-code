use nom::{
    bytes::complete::tag,
    character::complete::{digit1, newline},
    combinator::{eof, map, map_res},
    multi::separated_list1,
    IResult,
    Parser
};

fn number(input: &str) -> IResult<&str, isize> {
    map_res(digit1, str::parse).parse(input)
}

fn button_behavior(input: &str) -> IResult<&str, (isize, isize)> {
    map(
        (tag("X+"), number, tag(", Y+"), number),
        |(_, x, _, y)| (x, y)
    ).parse(input)
}

fn prize_coords(input: &str) -> IResult<&str, (isize, isize)> {
    map(
        (tag("X="), number, tag(", Y="), number),
        |(_, x, _, y)| (x, y)
    ).parse(input)
}

fn arcade(input: &str) -> IResult<&str, (isize, isize, isize, isize, isize, isize)> {
    let (input, (_, (a, d), _)) = (tag("Button A: "), button_behavior, newline).parse(input)?;
    let (input, (_, (b, e), _)) = (tag("Button B: "), button_behavior, newline).parse(input)?;
    let (input, (_, (c, f), _)) = (tag("Prize: "), prize_coords, newline).parse(input)?;
    Ok((input, (a, b, c, d, e, f)))
}

pub fn parse_input(input: &str) -> IResult<&str, Vec<(isize, isize, isize, isize, isize, isize)>> {
    let (input, arcades) = separated_list1(newline, arcade).parse(input)?;
    let (input, _) = eof(input)?;
    Ok((input, arcades))
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_parse_button_behavior() {
        let input = "X+94, Y+34";
        let (_, result) = button_behavior(input).expect("Expecting successful parsing");
        assert_eq!(result, (94, 34));
    }

    #[test]
    fn test_parse_prize_coords() {
        let input = "X=8400, Y=5400";
        let (_, result) = prize_coords(input).expect("Expecting successful parsing");
        assert_eq!(result, (8400, 5400));
    }

    #[test]
    fn test_parse_arcade() {
        let input = "Button A: X+94, Y+34
Button B: X+22, Y+67
Prize: X=8400, Y=5400
";
        let (_, result) = arcade(input).expect("Expecting successful parsing");
        assert_eq!(result, (94, 22, 8400, 34, 67, 5400));
    }

    #[test]
    fn test_parse_input() {
        let input = "Button A: X+94, Y+34
Button B: X+22, Y+67
Prize: X=8400, Y=5400

Button A: X+94, Y+34
Button B: X+22, Y+67
Prize: X=8400, Y=5400
";
        let (_, result) = parse_input(input).expect("Expecting successful parsing");
        assert_eq!(result, vec![
            (94, 22, 8400, 34, 67, 5400),
            (94, 22, 8400, 34, 67, 5400),
        ]);
    }
}
