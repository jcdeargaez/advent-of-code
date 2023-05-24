from functools import reduce
from datetime import datetime
import operator as op

DIVIDERS = [
    [[2]],
    [[6]]
]

def compare(a, b):
    match a, b:
        case int(), int(): return -1 if a < b else 1 if a > b else 0
        case int(), list(): return compare([a], b)
        case list(), int(): return compare(a, [b])
        case list(), list():
            for ai, bi in zip(a, b):
                if (c := compare(ai, bi)) != 0:
                    return c
            return compare(len(a), len(b))

def parse_packet(line):
    def parse_list(i, sb, packetSoFar):
        c = line[i]
        if c == ']':
            if len(sb) > 0:
                packetSoFar.append(int(sb))
            return i, packetSoFar
        
        if c == '[':
            i, np = parse_list(i + 1, "", [])
            packetSoFar.append(np)
        elif c == ',':
            if len(sb) > 0:
                packetSoFar.append(int(sb))
                sb = ""
        else:
            sb += c

        return parse_list(i + 1, sb, packetSoFar)
    
    return parse_list(1, "", [])[1]

def pairs(packets):
    left = None
    for packet in packets:
        if left is None:
            left = packet
        else:
            yield left, packet
            left = None

def part1(packets):
    return sum(i if compare(left, right) == -1 else 0
               for i, (left, right) in enumerate(pairs(packets), start=1))

def part2(packets):
    lessers = [1 for _ in DIVIDERS]
    for packet in packets + DIVIDERS:
        for i, p in enumerate(DIVIDERS):
            if compare(packet, p) == -1:
                lessers[i] += 1
    return reduce(op.mul, lessers)

def read_input(lines):
    for line in lines:
        line = line.strip()
        if len(line) > 0:
            yield parse_packet(line)

if __name__ == "__main__":
    for _ in range(3):
        t0 = datetime.now()

        with open("input.txt") as fin:
            packets = list(read_input(fin))
        t1 = datetime.now()

        p1 = part1(packets)
        t2 = datetime.now()

        p2 = part2(packets)
        t3 = datetime.now()

        print(f"Read input {(t1 - t0).total_seconds() * 1000} ms")
        print(f"Part 1 {p1} {(t2 - t1).total_seconds() * 1000} ms")
        print(f"Part 2 {p2} {(t3 - t2).total_seconds() * 1000} ms")
        print()
