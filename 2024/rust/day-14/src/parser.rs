use nom::{
    bytes::complete::tag,
    character::complete::{char, isize, newline, usize},
    combinator::{eof, map},
    multi::many1, sequence::{preceded, separated_pair, terminated},
    IResult,
    Parser
};

use crate::robot::Robot;
use crate::map::Map;

fn velocity(input: &str) -> IResult<&str, (isize, isize)> {
    separated_pair(isize, char(','), isize).parse(input)
}

fn position(input: &str) -> IResult<&str, (usize, usize)> {
    separated_pair(usize, char(','), usize).parse(input)
}

fn robot(input: &str) -> IResult<&str, Robot> {
    map(
        terminated((preceded( tag("p="), position), preceded(tag(" v="), velocity)), newline),
        |((x, y), (dx, dy))| Robot::new(x, y, dx, dy)
    ).parse(input)
}

pub fn parse_input(input: &str) -> IResult<&str, Map> {
    let (input, robots) = terminated(many1(robot), eof).parse(input)?;
    let map = if cfg!(test) {
        Map::small(robots)
    } else {
        Map::big(robots)
    };
    Ok((input, map))
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn robot_line() {
        let (_, robot) = robot("p=6,3 v=-1,-3\n").expect("Expecting successful parsing");
        assert_eq!(robot, Robot::new(6, 3, -1, -3));
    }

    #[test]
    fn robot_lines() {
        let input = "\
p=0,4 v=3,-3
p=6,3 v=-1,-3
";
        let (_, map) = parse_input(input).expect("Expecting successful parsing");
        assert_eq!(map.robots, vec![
            Robot::new(0, 4, 3, -3),
            Robot::new(6, 3, -1, -3)
        ]);
    }
}
