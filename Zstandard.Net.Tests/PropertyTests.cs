using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Zstandard.Net.Tests
{
    [TestClass]
    public class PropertyTests
    {
        [TestMethod]
        public void ZstandardStream_PropertyVersion_IsValid()
        {
            var value = ZstandardStream.Version;
            Assert.AreEqual(new Version(1, 3, 8), value);
        }

        [TestMethod]
        public void ZstandardStream_PropertyMaxCompressionLevel_IsValid()
        {
            var value = ZstandardStream.MaxCompressionLevel;
            Assert.AreEqual(22, value);
        }
    }
}
