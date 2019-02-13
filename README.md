# Zstandard.net

A Zstandard wrapper for .Net

Zstandard is a real-time compression algorithm, providing high compression ratios. It offers a very wide range of compression / speed trade-off, while being backed by a very fast decoder. It also offers a special mode for small data, called dictionary compression, and can create dictionaries from any sample set. Zstandard library is provided as open source software using a BSD license.

http://facebook.github.io/zstd/

## Example

```csharp
byte[] input = GetTestData();
byte[] compressed = null;
byte[] output = null;

// load a dictionary that is trained for the data (optional).
var dictionary = new ZstandardDictionary("loremipsum.zdict");

// compress
using (var memoryStream = new MemoryStream())
using (var compressionStream = new ZstandardStream(memoryStream, CompressionMode.Compress))
{
	compressionStream.CompressionLevel = 11;               // optional!!
	compressionStream.CompressionDictionary = dictionary;  // optional!!
	compressionStream.Write(input, 0, input.Length);
	compressionStream.Close();
	compressed = memoryStream.ToArray();
}

// decompress
using (var memoryStream = new MemoryStream(compressed))
using (var compressionStream = new ZstandardStream(memoryStream, CompressionMode.Decompress))
using (var temp = new MemoryStream())
{
	compressionStream.CompressionDictionary = dictionary;  // optional!!
	compressionStream.CopyTo(temp);
	output = temp.ToArray();
}

// test output
if (output.SequenceEqual(input) == false)
{
	throw new Exception("Output is different from input!");
}
```

## NuGet

Download the library from NuGet:

https://www.nuget.org/packages/Zstandard.Net/



## Building ZStd binaries

It's usually best to create the binaries within a container or vm of the same type of OS, mapping the zstd (facebook repo) to the instance (or checking out within the image, but you'll need to map a local folder to copy the files out).

### For alpine

```dockerfile
FROM alpine:3.8

RUN apk --no-cache add make gcc libc-dev
COPY . /src
RUN mkdir /pkg && cd /src && make && make DESTDIR=/pkg install

```

* `docker build -t tmpzstd .`
* `docker run --rm -it -v I:\Zstandard.Net\zstd:/wrk tmpzstd `



### For Debian & Windows

The makefile can be ran from WSL (ubuntu) and will generate a debian compatible library.

For Windows you'll need VS installed; once installed you simply run the command that matches your version in `zstd\build\VS_scripts` the dll is deposited in the bin/release/x64 folder.



