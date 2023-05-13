import os
from typing import Iterable, Mapping

TOTAL_SIZE = 70_000_000
REQUIRED_FREE_SPACE = 30_000_000

def read_input(lines: Iterable[str]) -> Mapping[str, int]:
    dir_size = {}

    def update_directories(path: str, size: int):
        dir_size[path] = dir_size.get(path, 0) + size
        if path != '/':
            update_directories(os.path.dirname(path), size)
    
    path = ""
    for line in lines:
        line = line.strip()
        if line.startswith('$ cd'):
            match line[5:]:
                case '..':
                    path = os.path.dirname(path)
                case dir:
                    path = os.path.join(path, dir)
        elif len(line) != 0 and line[0].isdecimal():
            file_size = int(line.split(maxsplit=1)[0])
            update_directories(path, file_size)
    
    return dir_size

def part1(dirs: Mapping[str, int]) -> int:
    return sum(v for v in dirs.values() if v <= 100_000)

def part2(dirs: Mapping[str, int]) -> int:
    free_space = TOTAL_SIZE - dirs['/']
    space_needed = REQUIRED_FREE_SPACE - free_space
    return min(v for v in dirs.values() if v >= space_needed)

if __name__ == '__main__':
    with open('input.txt') as fin:
        dirs = read_input(fin)
    print('Part 1:', part1(dirs))
    print('Part 2:', part2(dirs))
