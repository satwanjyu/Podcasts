using System.Xml.Linq;

namespace Podcasts.Tests.MSTest;

// https://docs.microsoft.com/visualstudio/test/getting-started-with-unit-testing
// https://docs.microsoft.com/visualstudio/test/using-microsoft-visualstudio-testtools-unittesting-members-in-unit-tests
// https://docs.microsoft.com/visualstudio/test/run-unit-tests-with-test-explorer

[TestClass]
public class SyndicationTests
{
    [TestMethod]
    public void ReadFromRawXML()
    {
        var root = XElement.Load("Assets/feed.xml");
        var title =
            from el in root.Elements()
            where el.Name.LocalName == "channel"
            from el2 in el.Elements()
            where el2.Name.LocalName == "title"
            select el2.Value;
        Assert.AreEqual(title.First(), "Lorem ipsum feed for an interval of 1 minutes with 10 item(s)");
    }

    [TestMethod]
    public async Task ReadWithWindowsWebSyndication()
    {
        var client = new Windows.Web.Syndication.SyndicationClient();
        var feed = await client.RetrieveFeedAsync(new Uri("https://lorem-rss.herokuapp.com/feed"));
        Assert.AreEqual(feed.Title.Text, "Lorem ipsum feed for an interval of 1 minutes with 10 item(s)");
        Assert.IsTrue(feed.Items[0].Title.Text.Contains("Lorem ipsum"));
    }
}
