use std::collections::{HashMap, HashSet};

mod parser {
    use nom::{
        character::complete::{char, digit1, newline},
        combinator::{eof, map_res},
        multi::{many1, separated_list1},
        sequence::{separated_pair, terminated}, IResult
    };

    fn parse_page_number(input: &str) -> IResult<&str, usize> {
        map_res(digit1, str::parse)(input)
    }

    fn parse_rule(input: &str) -> IResult<&str, (usize, usize)> {
        separated_pair(parse_page_number, char('|'), parse_page_number)(input)
    }

    fn parse_rules(input: &str) -> IResult<&str, Vec<(usize, usize)>> {
        let prule_nl = terminated(parse_rule, newline);
        terminated(many1(prule_nl), newline)(input)
    }

    fn parse_update(input: &str) -> IResult<&str, Vec<usize>> {
        separated_list1(char(','), parse_page_number)(input)
    }

    fn parse_updates(input: &str) -> IResult<&str, Vec<Vec<usize>>> {
        let pupdate_nl = terminated(parse_update, newline);
        many1(pupdate_nl)(input)
    }

    pub fn parse_input(input: &str) -> IResult<&str, (Vec<(usize, usize)>, Vec<Vec<usize>>)> {
        let (input, rules) = parse_rules(input)?;
        let (input, updates) = parse_updates(input)?;
        eof(input)?;
        Ok((input, (rules, updates)))
    }
}

fn process_rules(rules: &Vec<(usize, usize)>) -> (HashMap<usize, HashSet<usize>>, HashMap<usize, HashSet<usize>>) {
    let mut pages_before: HashMap<usize, HashSet<usize>> = HashMap::new();
    let mut pages_after: HashMap<usize, HashSet<usize>> = HashMap::new();
    for (a, b) in rules {
        pages_before
            .entry(*b)
            .or_insert(HashSet::new())
            .insert(*a);
        pages_after
            .entry(*a)
            .or_insert(HashSet::new())
            .insert(*b);
    }
    (pages_before, pages_after)
}

fn check_pages_before(pages_before: &HashMap<usize, HashSet<usize>>, update: &Vec<usize>, i: usize) -> bool {
    let p = update[i];
    pages_before
        .get(&p)
        .map_or(true, |before| {
            if i > 0 {
                update[..i]
                    .iter()
                    .all(|a| before.contains(a))
            } else { true }
        })
}

fn check_pages_after(pages_after: &HashMap<usize, HashSet<usize>>, update: &Vec<usize>, i: usize) -> bool {
    let p = update[i];
    pages_after
        .get(&p)
        .map_or(true, |after| {
            if i < update.len() - 1 {
                update[(i+1)..]
                    .iter()
                    .all(|b| after.contains(b))
            } else { true }
        })
}

fn check_ordering(pages_before: &HashMap<usize, HashSet<usize>>, pages_after: &HashMap<usize, HashSet<usize>>, update: &Vec<usize>) -> bool {
    (0..update.len())
        .all(|i| check_pages_before(&pages_before, &update, i) && check_pages_after(&pages_after, &update, i))
}

fn mid(v: &Vec<usize>) -> usize {
    v[(v.len() / 2) as usize]
}

pub fn part1(input: &str) -> usize {
    let (_, (rules, updates)) = parser::parse_input(input).unwrap();
    let (pages_before, pages_after) = process_rules(&rules);
    
    updates
        .iter()
        .filter(|update| check_ordering(&pages_before, &pages_after, update))
        .map(mid)
        .sum()
}

fn swap(v: &mut Vec<usize>, i: usize, j: usize) {
    let tmp = v[i];
    v[i] = v[j];
    v[j] = tmp;
}

fn fix_update(pages_before: &HashMap<usize, HashSet<usize>>, pages_after: &HashMap<usize, HashSet<usize>>, update: &Vec<usize>) -> Vec<usize> {
    let mut fixed = update.clone();
    for i in 0..fixed.len() {
        let mut j = i;
        while j < fixed.len() - 1 && !check_pages_after(pages_after, &fixed, j) {
            swap(&mut fixed, j, j + 1);
            j += 1;
        }
    
        let mut j = i;
        while j > 0 && !check_pages_before(pages_before, &fixed, j) {
            swap(&mut fixed, j - 1, j);
            j -= 1;
        }
    }
    fixed
}

pub fn part2(input: &str) -> usize {
    let (_, (rules, updates)) = parser::parse_input(input).unwrap();
    let (pages_before, pages_after) = process_rules(&rules);
    
    updates
        .iter()
        .filter(|update| !check_ordering(&pages_before, &pages_after, update))
        .map(|update| fix_update(&pages_before, &pages_after, update))
        .map(|update| mid(&update))
        .sum()
}

#[cfg(test)]
mod tests {
    use super::*;

    const INPUT: &str = "47|53
97|13
97|61
97|47
75|29
61|13
75|53
29|13
97|29
53|29
61|53
97|53
61|29
47|13
75|47
97|75
47|61
75|61
47|29
75|13
53|13

75,47,61,53,29
97,61,53,29,13
75,29,13
75,97,47,61,53
61,13,29
97,13,75,29,47
";

    #[test]
    fn test_part1() {
        let result = part1(INPUT);
        assert_eq!(result, 143);
    }

    #[test]
    fn test_part2() {
        let result = part2(INPUT);
        assert_eq!(result, 123);
    }
}
