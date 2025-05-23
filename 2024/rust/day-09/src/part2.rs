#[derive(Clone, Copy)]
struct File {
    id: usize,
    length: usize,
}

#[derive(Clone, Copy)]
struct FreeSpace {
    length: usize
}

enum DiskItem {
    File(File),
    FreeSpace(FreeSpace),
}

struct Disk {
    items: Vec<DiskItem>
}

fn parse_input(input: &str) -> Disk {
    let items: Vec<DiskItem> =
        input
            .char_indices()
            .map(|(i, c)| {
                let d = c.to_digit(10).expect("Expected a digit") as usize;
                if i % 2 == 0 {
                    let file_id = i / 2;
                    DiskItem::File(File { id: file_id, length: d })
                } else {
                    DiskItem::FreeSpace(FreeSpace { length: d })
                }
            })
            .collect();
    Disk { items }
}

fn compact(disk: &mut Disk) {
    let mut right = disk.items.len() - 1;
    while right > 0 {
        match disk.items[right] {
            DiskItem::File(f) => {
                disk.items
                    .iter()
                    .enumerate()
                    .take_while(|&(i, _item)| i < right)
                    .position(|(_i, item)| match item {
                        DiskItem::FreeSpace(fs) if fs.length >= f.length => true,
                        _ => false
                    })
                    .inspect(|&i| {
                        if let DiskItem::FreeSpace(mut available_space) = disk.items[i] {
                            available_space.length -= f.length;
                            disk.items[i] = DiskItem::FreeSpace(available_space);
                            let file_item = disk.items.remove(right);
                            disk.items.insert(i, file_item);
                            disk.items.insert(right, DiskItem::FreeSpace(FreeSpace { length: f.length }));
                        }
                    });
            },
            _ => (),
        }
        right -= 1;
    }
}

fn checksum(disk: Disk) -> usize {
    disk.items
        .iter()
        .flat_map(|item| match item {
            DiskItem::File(f) => vec![f.id; f.length],
            DiskItem::FreeSpace(fs) => vec![0; fs.length]
        })
        .enumerate()
        .map(|(i, file_id)| i * file_id)
        .sum()
}

pub fn part2(input: &str) -> usize {
    let mut disk = parse_input(input);
    compact(&mut disk);
    checksum(disk)
}

mod tests {
    use super::*;

    const INPUT: &str = "2333133121414131402";

    #[test]
    fn test_part2() {
        let result = part2(INPUT);
        assert_eq!(result, 2858);
    }
}