using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

using System.Text.RegularExpressions;

class Program
{
    static async Task Main(string[] args)
    {
        // Validate the input URL
       // var inp = "https://youtu.be/LMHbetTwXfw";
        if (args.Length == 0)
        {
            var  input = "https://youtu.be/LMHbetTwXfw";
            Console.WriteLine(input);
            return;
        }

        var youtube = new YoutubeClient();
        var videoUrl = args[0];

        try
        {
            // Get video ID from URL
            var videoId = ExtractVideoId(videoUrl);
            if (string.IsNullOrEmpty(videoId))
            {
                Console.WriteLine("Invalid YouTube URL.");
                return;
            }

            // Get video information
            var video = await youtube.Videos.GetAsync(videoId);
            Console.WriteLine($"Title: {video.Title}");
            Console.WriteLine($"Author: {video.Author}");
            Console.WriteLine($"Duration: {video.Duration}");

            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoId);

            // Select the highest quality muxed stream (both video and audio)
            var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

            if (streamInfo != null)
            {
                var filePath = $"{video.Title}.{streamInfo.Container}";

                // Download the video stream
                Console.WriteLine("Downloading...");
                await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath);
                Console.WriteLine($"Downloaded to {filePath}");
            }
            else
            {
                Console.WriteLine("No suitable streams found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static string ExtractVideoId(string url)
    {
        // Regular expression to extract the video ID from the URL
        var regex = new Regex(@"(?:youtube\.com\/.*v=|youtu\.be\/)([\w-]+)");
        var match = regex.Match(url);

        return match.Success ? match.Groups[1].Value : string.Empty;
    }
    
    
}
