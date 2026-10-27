# sarchive
The simplest and probably worst archiving tool.

Loosely based on tar format:
All blocks 512 bytes

Header block format:
- 256 for path/name
- 8 for size
- 248 padding/null

Data block:
- as many 512b blocks as it takes, final one padded to meet 512

EOF:
- 512 all zeroes/null
