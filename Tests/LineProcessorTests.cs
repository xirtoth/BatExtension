using NUnit.Framework;
using System.Windows.Documents;
using BatExtension;
using NUnit.Framework.Legacy;

namespace BatExtension.Tests
{
    [TestFixture]
    public class LineProcessorTests
    {
        [SetUp]
        public void Setup()
        {
            LineProcessor.Initialize(inCombat => { /* No-op for tests */ });
        }

        [Test]
        public void ProcessLine_MissPattern_ShouldReturnYouMissed()
        {
            // Arrange
            string input = "You miss.";

            // Act
            var (paragraph, inCombat) = LineProcessor.ProcessLine(input);

            // Assert
            ClassicAssert.AreEqual("YOU MISSED", new TextRange(paragraph.ContentStart, paragraph.ContentEnd).Text);
            ClassicAssert.IsFalse(inCombat);
        }

        [Test]
        public void ProcessLine_HitPattern_ShouldReturnYouScrapeAsterisk()
        {
            // Arrange
            string input = "You cut Full-grown badger.";

            // Act
            var (paragraph, inCombat) = LineProcessor.ProcessLine(input);

            // Assert
            ClassicAssert.AreEqual("YOU CUT Full-grown badger", new TextRange(paragraph.ContentStart, paragraph.ContentEnd).Text);
            ClassicAssert.IsFalse(inCombat);
        }
        [Test]
        public void HitMessage_ShouldReturnYouHitAsterisk()
        {
            // Arrange
            string input = "You hit Full-grown badger.";

            // Act
            var (paragraph, inCombat) = LineProcessor.ProcessLine(input);

            // Assert
            ClassicAssert.AreEqual("YOU HIT *", new TextRange(paragraph.ContentStart, paragraph.ContentEnd).Text);
            ClassicAssert.IsFalse(inCombat);
        }
        [Test]
        public void ProcessLine_RoundPattern_ShouldReturnCombatStarted()
        {
            // Arrange
            string input = "********************** Round 1 **********************";

            // Act
            var (paragraph, inCombat) = LineProcessor.ProcessLine(input);

            // Assert
            ClassicAssert.AreEqual("COMBAT STARTED", new TextRange(paragraph.ContentStart, paragraph.ContentEnd).Text);
            ClassicAssert.IsTrue(inCombat);
        }
    }
}