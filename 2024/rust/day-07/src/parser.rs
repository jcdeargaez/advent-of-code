use nom::{
    bytes::complete::tag,
    character::complete::{char, line_ending, usize},
    combinator::eof,
    multi::{many1, separated_list1},
    sequence::{separated_pair, terminated},
    IResult,
    Parser
};

use crate::equation::Equation;

fn parse_equation(input: &str) -> IResult<&str, Equation> {
    let (input, (test_value, values)) = separated_pair(usize, tag(": "), separated_list1(char(' '), usize)).parse(input)?;
    Ok((input, Equation::new(test_value, values)))
}

pub fn parse_input(input: &str) -> Vec<Equation> {
    let pequation_nl = terminated(parse_equation, line_ending);
    let (_input, equations) = terminated(many1(pequation_nl), eof).parse(input).expect("Expected valid equations input");
    equations
}