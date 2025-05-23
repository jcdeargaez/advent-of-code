use std::{cmp::min, collections::HashMap};

fn subtree_index() -> HashMap<usize, Vec<Vec<usize>>> {
    let sub_trees = vec![
        (0, vec![
            vec![1],
            vec![2024],
            vec![20, 24],
            vec![2, 0, 2, 4],
        ]),
        (1, vec![
            vec![2024],
            vec![20, 24],
            vec![2, 0, 2, 4],
        ]),
        (2, vec![
            vec![4048],
            vec![40, 48],
            vec![4, 0, 4, 8],
        ]),
        (3, vec![
            vec![6072],
            vec![60, 72],
            vec![6, 0, 7, 2],
        ]),
        (4, vec![
            vec![8096],
            vec![80, 96],
            vec![8, 0, 9, 6],
        ]),
        (5, vec![
            vec![10120],
            vec![20482880],
            vec![2048, 2880],
            vec![20, 48, 28, 80],
            vec![2, 0, 4, 8, 2, 8, 8, 0],
        ]),
        (6, vec![
            vec![12144],
            vec![24579456],
            vec![2457, 9456],
            vec![24, 57, 94, 56],
            vec![2, 4, 5, 7, 9, 4, 5, 6],
        ]),
        (7, vec![
            vec![14168],
            vec![28676032],
            vec![2867, 6032],
            vec![28, 67, 60, 32],
            vec![2, 8, 6, 7, 6, 0, 3, 2],
        ]),
        (8, vec![
            vec![16192],
            vec![32772608],
            vec![3277, 2608],
            vec![32, 77, 26, 8],
            vec![3, 2, 7, 7, 2, 6, 16192],
        ]),
        (9, vec![
            vec![18216],
            vec![36869184],
            vec![3686, 9184],
            vec![36, 86, 91, 84],
            vec![3, 6, 8, 6, 9, 1, 8, 4],
        ]),
        (16192, vec![
            vec![32772608],
            vec![3277, 2608],
            vec![32, 77, 26, 8],
            vec![3, 2, 7, 7, 2, 6, 16192],
        ]),
    ];
    HashMap::from_iter(sub_trees)
}

fn blink(v: usize) -> Vec<usize> {
    if v == 0 {
        vec![1]
    } else {
        let s = v.to_string();
        if s.len() % 2 == 0 {
            vec![
                s[..s.len() / 2].parse().expect("Expecting digits"),
                s[s.len() / 2..].parse().expect("Expecting digits")
            ]
        } else {
            vec![v * 2024]
        }
    }
}

fn blink_bfs(input: &Vec<usize>) -> Vec<usize> {
    input
        .iter()
        .flat_map(|&v| blink(v))
        .collect()
}

pub fn part1(input: &str) -> usize {
    let initial = input
        .split(' ')
        .map(|s| s.parse().expect("Expecting digits"))
        .collect();
    (0..25)
        .fold(initial, |stones_so_far, _i| blink_bfs(&stones_so_far))
        .len()
}

fn blink_dfs(values: &Vec<usize>, level_so_far: usize, max_level: usize, subtree_index: &HashMap<usize, Vec<Vec<usize>>>, cache: &mut HashMap<(usize, usize), usize>) -> usize {
    if level_so_far == max_level {
        values.len()
    } else {
        let mut total = 0;
        for &v in values {
            let key = (v, level_so_far);
            if let Some(result) = cache.get(&key) {
                total += result;
            } else {
                let (next_level, next_values) =
                    if let Some(levels) = subtree_index.get(&v) {
                        let jump = min(max_level - level_so_far, levels.len());
                        (level_so_far + jump, &levels[jump - 1])
                    } else {
                        (level_so_far + 1, &blink(v))
                    };
                let result = blink_dfs(&next_values, next_level, max_level, subtree_index, cache);
                total += result;
                cache.insert(key, result);
            }
        }
        total
    }
}

pub fn part2(input: &str) -> usize {
    let initial = input
        .split(' ')
        .map(|s| s.parse().expect("Expecting digits"))
        .collect();
    let subtree_index = subtree_index();
    let mut cache = HashMap::new();
    blink_dfs(&initial, 0, 75, &subtree_index, &mut cache)
}

#[cfg(test)]
mod tests {
    use super::*;

    const INPUT: &str = "125 17";

    #[test]
    fn test_part1() {
        let result = part1(INPUT);
        assert_eq!(result, 55312);
    }

    #[test]
    fn test_part2() {
        let result = part2(INPUT);
        assert_eq!(result, 65601038650482);
    }
}
