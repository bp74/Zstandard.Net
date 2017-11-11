# Zstandard.net

A Zstandard wrapper for .Net

Zstandard is a real-time compression algorithm, providing high compression ratios. It offers a very wide range of compression / speed trade-off, while being backed by a very fast decoder. It also offers a special mode for small data, called dictionary compression, and can create dictionaries from any sample set. Zstandard library is provided as open source software using a BSD license.

http://facebook.github.io/zstd/

## Example

	byte[] input = GetTestData();
	byte[] compressed = null;
	byte[] output = null;

	// compress
	using (var memoryStream = new MemoryStream())
	using (var compressionStream = new ZstandardStream(memoryStream, compressionLevel: 19))
	{
		//compressionStream.CompressionLevel = 6; // maxCompressionLevel;
		compressionStream.Write(input, 0, input.Length);
		compressionStream.Close();
		compressed = memoryStream.ToArray();
	}

	// decompress
	using (var memoryStream = new MemoryStream(compressed))
	using (var compressionStream = new ZstandardStream(memoryStream, CompressionMode.Decompress))
	using (var temp = new MemoryStream())
	{
		compressionStream.CopyTo(temp);
		output = temp.ToArray();
	}

	// test output
	if (output.SequenceEqual(input) == false)
	{
		throw new Exception("Output is different from input!");
	}
