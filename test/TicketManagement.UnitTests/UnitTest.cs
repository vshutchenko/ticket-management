using NUnit.Framework;

namespace TicketManagement.UnitTests
{
    /// <summary>
    /// The purpose of this class is just a demonstration.
    /// Please refer to the documentation link for more information regarding NUnit framework.
    /// https://docs.nunit.org/articles/nunit/intro.html.
    /// </summary>
    public class Tests
    {
        /// <summary>
        /// This is setup method, please refer to the documentation link for more info
        /// https://docs.nunit.org/articles/nunit/writing-tests/attributes/setup.html.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // empty for demonstration purposes
        }

        /// <summary>
        /// This is a test method, please refer to the documentation link for more info
        /// https://docs.nunit.org/articles/nunit/writing-tests/attributes/test.html".
        /// </summary>
        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}