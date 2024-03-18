using Microsoft.Data.Sqlite;
using Windows.Storage;
using Windows.Web.Syndication;

namespace Podcasts.Services;
public class SqliteDataService
{
    public SqliteConnection? connection;

    public async Task<SqliteConnection> GetOpenConnectionAsync()
    {
        var localFolder = ApplicationData.Current.LocalFolder;
        var feedsDbFile = await localFolder.CreateFileAsync("feeds.db", CreationCollisionOption.OpenIfExists);
        var feedsDbPath = feedsDbFile.Path;

        var connection = new SqliteConnection($"Filename={feedsDbPath}");
        await connection.OpenAsync();

        return connection;
    }

    public async Task CreateFeedsTable(SqliteConnection connection)
    {
        var createFeedsTableCommand = connection.CreateCommand();
        createFeedsTableCommand.CommandText = @"
            CREATE TABLE IF NOT EXISTS Feeds (
                Id TEXT PRIMARY KEY,
                Title TEXT NOT NULL,
                Subtitle TEXT NOT NULL,
                ImageUrl TEXT NOT NULL,
                FeedUrl TEXT NOT NULL,
                LastUpdated TEXT NOT NULL
            );
        ";
        await createFeedsTableCommand.ExecuteNonQueryAsync();
    }

    public async Task CreateItemsTable(SqliteConnection connection)
    {
        var createItemsTableCommand = connection.CreateCommand();
        createItemsTableCommand.CommandText = @"
            CREATE TABLE IF NOT EXISTS Items (
                FeedId TEXT NOT NULL,
                Title TEXT NOT NULL,
                AudioUrl TEXT NOT NULL,
                Published TEXT NOT NULL,
                Summary TEXT NOT NULL,
                Progress REAL NOT NULL
            );
        ";
        await createItemsTableCommand.ExecuteNonQueryAsync();
    }

    public async Task InsertFeed(SqliteConnection connection, SyndicationFeed feed, string feedUrl)
    {
        using var transaction = await connection.BeginTransactionAsync();
        var insertFeedCommand = connection.CreateCommand();
        insertFeedCommand.CommandText = @"
            INSERT INTO Feeds (
                Id,
                Title,
                Subtitle,
                ImageUrl,
                FeedUrl,
                LastUpdated
            ) VALUES (
                @Id,
                @Title,
                @Subtitle,
                @ImageUrl,
                @FeedUrl,
                @LastUpdated
            );
        ";

        var feedId = Guid.NewGuid().ToString();
        insertFeedCommand.Parameters.AddWithValue("@Id", feedId);
        insertFeedCommand.Parameters.AddWithValue("@Title", feed.Title.Text);
        insertFeedCommand.Parameters.AddWithValue("@Subtitle", feed.Subtitle.Text);
        insertFeedCommand.Parameters.AddWithValue("@ImageUrl", feed.ImageUri?.ToString());
        insertFeedCommand.Parameters.AddWithValue("@FeedUrl", feedUrl);
        insertFeedCommand.Parameters.AddWithValue("@LastUpdated", feed.LastUpdatedTime.ToString());
        await insertFeedCommand.ExecuteNonQueryAsync();

        var insertItemCommand = connection.CreateCommand();
        insertItemCommand.CommandText = @"
            INSERT INTO Items (
                FeedId,
                Title,
                AudioUrl,
                Published,
                Summary,
                Progress
            ) VALUES (
                @FeedId,
                @Title,
                @AudioUrl,
                @Published,
                @Summary,
                @Progress
            );
        ";
        insertItemCommand.Parameters.AddWithValue("@FeedId", feedId);
        var titleParameter = insertItemCommand.CreateParameter();
        titleParameter.ParameterName = "@Title";
        insertItemCommand.Parameters.Add(titleParameter);
        var audioUrlParameter = insertItemCommand.CreateParameter();
        audioUrlParameter.ParameterName = "@AudioUrl";
        insertItemCommand.Parameters.Add(audioUrlParameter);
        var publishedParameter = insertItemCommand.CreateParameter();
        publishedParameter.ParameterName = "@Published";
        insertItemCommand.Parameters.Add(publishedParameter);
        var summaryParameter = insertItemCommand.CreateParameter();
        summaryParameter.ParameterName = "@Summary";
        insertItemCommand.Parameters.Add(summaryParameter);
        insertItemCommand.Parameters.AddWithValue("@Progress", 0.0);

        foreach (var item in feed.Items)
        {
            var audioLinks = from link in item.Links
                             where link.MediaType == "audio/mpeg"
                             select link;
            if (audioLinks.Count() != 1)
            {
                continue;
            }
            var audioLink = audioLinks.First();

            titleParameter.Value = item.Title.Text;
            audioUrlParameter.Value = audioLink.Uri.ToString();
            publishedParameter.Value = item.PublishedDate.ToString();
            summaryParameter.Value = item.Summary.Text;

            await insertItemCommand.ExecuteNonQueryAsync();
        }

        await transaction.CommitAsync();
    }

    public async Task InsertFeeds(SqliteConnection connection, IEnumerable<SyndicationFeed> feeds, string feedUrl)
    {
        foreach (var feed in feeds)
        {
            await InsertFeed(connection, feed, feedUrl);
        }
    }

    private async Task<List<SyndicationItem>> GetItems(SqliteConnection connection, string feedId)
    {
        var selectItemsCommand = connection.CreateCommand();
        selectItemsCommand.CommandText = @"
            SELECT
                Title,
                AudioUrl,
                Published,
                Summary,
                Progress
            FROM Items
            WHERE FeedId = @FeedId;
        ";
        selectItemsCommand.Parameters.AddWithValue("@FeedId", feedId);
        var items = new List<SyndicationItem>();
        using (var reader = await selectItemsCommand.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var item = new SyndicationItem()
                {
                    Title = new SyndicationText(reader.GetString(0)),
                    Links = { new SyndicationLink(new Uri(reader.GetString(1))) { MediaType = "audio/mpeg" } },
                    PublishedDate = DateTime.Parse(reader.GetString(2)),
                    Summary = new SyndicationText(reader.GetString(3)),
                };
                items.Add(item);
            }
        }
        return items;

    }

    public async Task<List<SyndicationFeed>> GetFeeds(SqliteConnection connection)
    {
        var selectFeedsCommand = connection.CreateCommand();
        selectFeedsCommand.CommandText = @"
            SELECT
                Id,
                Title,
                Subtitle,
                ImageUrl,
                FeedUrl,
                LastUpdated
            FROM Feeds;
        ";
        var feeds = new List<SyndicationFeed>();
        using (var reader = await selectFeedsCommand.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var feed = new SyndicationFeed()
                {
                    Id = reader.GetString(0),
                    Title = new SyndicationText(reader.GetString(1)),
                    Subtitle = new SyndicationText(reader.GetString(2)),
                    ImageUri = new Uri(reader.GetString(3)),
                    LastUpdatedTime = DateTime.Parse(reader.GetString(5)),
                };
                var items = await GetItems(connection, feed.Id);
                foreach (var item in items)
                {
                    feed.Items.Add(item);
                }
                feeds.Add(feed);
            }
        }
        return feeds;
    }
}
