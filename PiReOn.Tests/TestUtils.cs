using AuthLib;
using AuthLib.JSON;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using PiReOnLauncher.Code;
using PiReOnLauncher.Code.Updater;
using PiReOnLauncher.NewsBlock;

namespace PiReOn.Tests
{
    [TestClass]
    public class TestUtils
    {
        [TestMethod]
        public void GameVersionTests()
        {
            //TRUE

            GameVersion MAJOR1 = new GameVersion(1, 0, 0);
            GameVersion MAJOR2 = new GameVersion(2, 0, 0);

            GameVersion MINOR1 = new GameVersion(1, 1, 4);
            GameVersion MINOR2 = new GameVersion(0, 1, 4);

            GameVersion BUILD1 = new GameVersion(0, 0, 1);
            GameVersion BUILD2 = new GameVersion(0, 0, 2);


            //################################################
            //FALSE//


            GameVersion MAJOR3 = new GameVersion(2, 0, 0);
            GameVersion MAJOR4 = new GameVersion(1, 0, 0);

            GameVersion MINOR3 = new GameVersion(0, 2, 0);
            GameVersion MINOR4 = new GameVersion(0, 1, 0);

            GameVersion BUILD3 = new GameVersion(0, 0, 2);
            GameVersion BUILD4 = new GameVersion(0, 0, 1);

            Assert.AreEqual(MAJOR1 < MAJOR2, true);
            Assert.AreEqual(MAJOR3 > MAJOR4, true);
            Assert.AreEqual(MINOR1 > MINOR2, true);
            Assert.AreEqual(MINOR3 > MINOR4, true);
            Assert.AreEqual(BUILD1 < BUILD2, true);
            Assert.AreEqual(BUILD3 > BUILD4, true);
        }
        [TestMethod]
        public void AuthTest()
        {
            Authorization auth = new Authorization("Test", "Test123");
            Assert.AreEqual(auth.GetPacket().Status == AuthStatus.Success, true);

            Authorization auth2 = new Authorization("MyName", "TestPassword");
            Assert.AreEqual(auth2.GetPacket().Status == AuthStatus.Success, false);
        }
    }
}
