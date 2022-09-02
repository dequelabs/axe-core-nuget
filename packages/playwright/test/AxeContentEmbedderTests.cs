#nullable enable

using Deque.AxeCore.Playwright.AxeContent;
using Microsoft.Playwright;
using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deque.AxeCore.Playwright.Test
{
    [TestFixture]
    [Category("Unit")]
    public sealed class AxeContentEmbedderTests
    {
        [Test]
        [TestCase(false, 1, 0)]
        [TestCase(true, 0, 1)]
        [TestCase(null, 0, 1)]
        public async Task EmbedAxeCoreIntoPage_NoIFrames_EmbedsIntoPage(bool? embedIntoFrames, int expectedPageEvalCount, int expectedIframeEvalCount)
        {
            const string axeContent = "() => void";
            string axeFunctionExpression = $"() => {axeContent}";

            Mock<IFrame> frameOne = new();
            Mock<IFrame> frameTwo = new();
            frameOne.Setup(mock => mock.EvaluateAsync(axeFunctionExpression, null));
            frameTwo.Setup(mock => mock.EvaluateAsync(axeFunctionExpression, null));

            IReadOnlyList<IFrame> frames = new List<IFrame>() 
            { 
                frameOne.Object, 
                frameTwo.Object 
            };

            Mock<IAxeContentProvider> contentProviderMock = new();
            contentProviderMock.Setup(mock => mock.GetAxeCoreScriptContent())
                .Returns(axeContent);

            DefaultAxeContentEmbedder contentEmbedder = new(contentProviderMock.Object);

            Mock<IPage> pageMock = new();
            pageMock.Setup(mock => mock.EvaluateAsync(axeFunctionExpression, null));
            pageMock.Setup(mock => mock.Frames).Returns(frames);

            await contentEmbedder.EmbedAxeCoreIntoPage(pageMock.Object, embedIntoFrames);

            pageMock.Verify(mock => mock.EvaluateAsync(axeFunctionExpression, null), Times.Exactly(expectedPageEvalCount));
            frameOne.Verify(mock => mock.EvaluateAsync(axeFunctionExpression, null), Times.Exactly(expectedIframeEvalCount));
            frameTwo.Verify(mock => mock.EvaluateAsync(axeFunctionExpression, null), Times.Exactly(expectedIframeEvalCount));
        }
    }
}
