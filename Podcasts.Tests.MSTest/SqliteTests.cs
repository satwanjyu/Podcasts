using Microsoft.Data.Sqlite;
using Podcasts.Services;
using Windows.Web.Syndication;

namespace Podcasts.Tests.MSTest;

[TestClass]
public class SqliteTests
{
    [TestMethod]
    public async Task InsertAndRetrieveFeed()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var service = new SqliteDataService();
        await service.CreateFeedsTable(connection);
        await service.CreateItemsTable(connection);

        var feed = new SyndicationFeed()
        {
            Title = new SyndicationText("Test Feed Title"),
            Subtitle = new SyndicationText("Test Feed Subtitle"),
            ImageUri = new Uri("https://example.com/test-feed-image.png"),
            LastUpdatedTime = new DateTime(2000, 1, 1),
        };

        var audioLink = new SyndicationLink(new Uri("https://example.com/test-item.mp3"))
        {
            MediaType = "audio/mpeg"
        };
        var item = new SyndicationItem()
        {
            Title = new SyndicationText("Test Item Title"),
            Links = { audioLink },
            PublishedDate = new DateTime(2000, 1, 1),
            Summary = new SyndicationText("Test Item Summary"),
        };
        var item2 = new SyndicationItem()
        {
            Title = new SyndicationText("Test Item Title 2"),
            Links = { audioLink },
            PublishedDate = new DateTime(2000, 1, 1),
            Summary = new SyndicationText("Test Item Summary 2"),
        };
        feed.Items.Add(item);
        feed.Items.Add(item2);

        await service.InsertFeed(connection, feed, "https://example.com/test-feed.xml");

        var retrievedFeeds = await service.GetFeeds(connection);
        var retrievedFeed = retrievedFeeds.First();

        Assert.AreEqual(feed.Title.Text, retrievedFeed.Title.Text);
        Assert.AreEqual(feed.Subtitle.Text, retrievedFeed.Subtitle.Text);
        Assert.AreEqual(feed.ImageUri, retrievedFeed.ImageUri);
        Assert.AreEqual(feed.LastUpdatedTime, retrievedFeed.LastUpdatedTime);

        var retrievedItem = retrievedFeed.Items[0];
        Assert.AreEqual(item.Title.Text, retrievedItem.Title.Text);
        Assert.AreEqual(item.Links[0].Uri, retrievedItem.Links[0].Uri);
        Assert.AreEqual(item.Links[0].MediaType, retrievedItem.Links[0].MediaType);
        Assert.AreEqual(item.PublishedDate, retrievedItem.PublishedDate);
        Assert.AreEqual(item.Summary.Text, retrievedItem.Summary.Text);
    }
}
