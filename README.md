<h1>Binary File Patcher</h1>
Simple tool to quickly replace a pattern of bytes in a file. Can replace one or multiple instances of the pattern, with a matching length of bytes.
Byte inputs are to be written in HEX code.

<h2>Command line usage:</h2>
BinaryFilePatcher.exe somefile.dll "42 4F 4F 42 53" "42 55 54 54 53" false

Argument 1: Target file.
Argument 2: HEX pattern to look for.
Argument 3: HEX pattern to replace with.
Argument 4 (optional): If set to true, all matching instances of the pattern will be replaced, instead of just the first.