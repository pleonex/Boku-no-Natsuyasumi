import struct

RANGES = [[0x8140, 11], [0x8151, 1], [0x8158, 1], [0x815B, 2],
          [0x815E, 1], [0x8160, 1], [0x8162, 2], [0x8165, 2],
          [0x8169, 2], [0x8175, 10], [0x8180, 2], [0x8183, 2],
          [0x8189, 2], [0x818F, 1], [0x8193, 5], [0x819B, 1],
          [0x81A0, 1], [0x81A2, 1], [0x81A8, 4], [0x824F, 10],
          [0x8260, 26], [0x8281, 26], [0x829F, 79], [0x82F0, 2],
          [0x8340, 63], [0x8380, 16], [0x8392, 5]]


def create_table(ranges):
    table = []
    for r in ranges:
        start = r[0]
        count = r[1]
        for code in range(start, start + count):
            table.append(code)
    return table


def decode(code):
    return struct.pack(">H", code).decode("shift_jis").encode("utf-8")

table = create_table(RANGES)
with open("table.txt", "w") as file:
    for i in range(len(table)):
        file.write("{0:04} = {1}\n".format(i, decode(table[i])))

# Create template for Kanji table (must be manually, sorry)
with open("kanji.txt", "w") as file:
    for i in range(279, 1024):
        if i % 32 == 0:
          file.write("#\n")
        file.write("{0:04} = \n".format(i))
