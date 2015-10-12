using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject.MockingKernel.Moq;
using Ninject;

namespace StockMarketSharedLibrary.Tests
{
    [TestClass]
    public class HashHelperTests : BaseTest
    {
        public HashHelperTests()
        {
            _kernel.Bind<IHashHelper>().To<HashHelper>();
        }

        [TestMethod]
        public void ComputeHash_ValidMessageAndKey_GenerateHash()
        {
            // prepare
            string message = "hello";
            string key = "1234";

            // act
            var hashHelper = _kernel.Get<IHashHelper>();
            string computedHash = hashHelper.ComputeHash(message, key);

            // assert
            Assert.IsNotNull(computedHash);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ComputeHash_NullMessage_Exception()
        {
            // prepare
            string message = null;
            string key = "1234";

            // act
            var hashHelper = _kernel.Get<IHashHelper>();
            string computedHash = hashHelper.ComputeHash(message, key);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ComputeHash_NullKey_Exception()
        {
            // prepare
            string message = "hello";
            string key = null;

            // act
            var hashHelper = _kernel.Get<IHashHelper>();
            string computedHash = hashHelper.ComputeHash(message, key);
        }
    }
}
