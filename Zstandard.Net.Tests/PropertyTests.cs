using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Zstandard.Net.Tests
{
    [TestClass]
    public class PropertyTests
    {
        [TestMethod]
        public void ZstandardStream_PropertyVersion_IsValid()
        {
            var value = ZstandardStream.Version;
            Assert.AreEqual(new Version(1, 3, 2), value);
        }

        [TestMethod]
        public void ZstandardStream_PropertyMaxCompressionLevel_IsValid()
        {
            var value = ZstandardStream.MaxCompressionLevel;
            Assert.AreEqual(22, value);
        }
    }
}
