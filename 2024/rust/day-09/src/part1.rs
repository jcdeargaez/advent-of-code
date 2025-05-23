type FileId = usize;

#[derive(Clone, Copy)]
enum Block {
    File(FileId),
    Free,
}

struct Disk {
    blocks: Vec<Block>
}

#[derive(Clone, Copy)]
struct DiskCompactingIterator<'a> {
    left: usize,
    right: isize,
    disk: &'a Disk,
}

fn parse_input(input: &str) -> Disk {
    let blocks =
        input
            .char_indices()
            .flat_map(|(i, c)| {
                let d = c.to_digit(10).expect("Expected a digit") as usize;
                if i % 2 == 0 {
                    let file_id = i / 2;
                    vec![Block::File(file_id); d]
                } else {
                    vec![Block::Free; d]
                }
            })
            .collect();
    Disk { blocks }
}

impl<'a> Iterator for DiskCompactingIterator<'a> {
    type Item = FileId;

    fn next(&mut self) -> Option<Self::Item> {
        if self.right < 0 || self.left > self.right as usize {
            None
        } else {
            match self.disk.blocks[self.left] {
                Block::File(file_id) => {
                    self.left += 1;
                    Some(file_id)
                },
                Block::Free => {
                    while let Block::Free = self.disk.blocks[self.right as usize] {
                        if self.right == 0 {
                            break
                        }
                        self.right -= 1;
                    }
                    if let Block::File(file_id) = self.disk.blocks[self.right as usize] {
                        self.left += 1;
                        self.right -= 1;
                        Some(file_id)
                    } else {
                        None
                    }
                }
            }
        }
    }
}

impl Disk {
    fn compact(&self) -> DiskCompactingIterator {
        DiskCompactingIterator {
            left: 0,
            right: self.blocks.len() as isize - 1,
            disk: self
        }
    }
}

impl<'a> DiskCompactingIterator<'a> {
    fn checksum(&self) -> usize {
        self
            .enumerate()
            .map(|(i, file_id)| i * file_id)
            .sum()
    }
}

pub fn part1(input: &str) -> usize {
    let disk = parse_input(input);
    disk
        .compact()
        .checksum()
}

#[cfg(test)]
mod tests {
    use super::*;

    const INPUT: &str = "2333133121414131402";

    #[test]
    fn test_part1() {
        let result = part1(INPUT);
        assert_eq!(result, 1928);
    }
}