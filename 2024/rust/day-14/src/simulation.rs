use crate::map::Map;
use crate::parser;

pub fn run(map: &Map, easter_egg: bool) -> usize {
    if easter_egg {
        map.xmas_tree()
    } else {
        map.safety_factor(100)
    }
}

pub fn part1(input: &str) -> usize {
    let (_, map) = parser::parse_input(input).expect("Expecting parsed input");
    run(&map, false)
}

pub fn part2(input: &str) -> usize {
    let (_, map) = parser::parse_input(input).expect("Expecting parsed input");
    run(&map, true)
}

#[cfg(test)]
mod tests {
    use super::*;

    const INPUT: &str = "\
p=0,4 v=3,-3
p=6,3 v=-1,-3
p=10,3 v=-1,2
p=2,0 v=2,-1
p=0,0 v=1,3
p=3,0 v=-2,-2
p=7,6 v=-1,-3
p=3,0 v=-1,-2
p=9,3 v=2,3
p=7,3 v=-1,2
p=2,4 v=2,-3
p=9,5 v=-3,-3
";

    #[test]
    fn test_part1() {
        let result = part1(INPUT);
        assert_eq!(result, 12);
    }
}