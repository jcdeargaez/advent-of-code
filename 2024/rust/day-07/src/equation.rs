use std::collections::VecDeque;

pub struct Equation {
    pub test_value: usize,
    pub values: Vec<usize>
}

impl Equation {
    pub fn new(test_value: usize, values: Vec<usize>) -> Self {
        Self { test_value, values }
    }

    pub fn has_solution(&self, concat_op: bool) -> bool {
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
                    pending.push_back((i + 1, format!("{}{}", total_so_far, v).parse::<usize>().expect("Expected a number to parse")));
                }
            }
        }
        false
    }
}