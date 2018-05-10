using System;
using System.Collections.Generic;
using System.Text;

namespace appel
{
    /// <summary>
    /// Set of thumbnails for a video.
    /// </summary>
    public class ThumbnailSet
    {
        private readonly string _videoId;

        /// <summary>
        /// Low resolution thumbnail URL.
        /// </summary>
        public string LowResUrl => $"https://img.youtube.com/vi/{_videoId}/default.jpg";

        /// <summary>
        /// Medium resolution thumbnail URL.
        /// </summary>
        public string MediumResUrl => $"https://img.youtube.com/vi/{_videoId}/mqdefault.jpg";

        /// <summary>
        /// High resolution thumbnail URL.
        /// </summary>
        public string HighResUrl => $"https://img.youtube.com/vi/{_videoId}/hqdefault.jpg";

        /// <summary>
        /// Standard resolution thumbnail URL.
        /// Not always available.
        /// </summary>
        public string StandardResUrl => $"https://img.youtube.com/vi/{_videoId}/sddefault.jpg";

        /// <summary>
        /// Max resolution thumbnail URL.
        /// Not always available.
        /// </summary>
        public string MaxResUrl => $"https://img.youtube.com/vi/{_videoId}/maxresdefault.jpg";

        /// <summary />
        public ThumbnailSet(string videoId)
        {
            _videoId = videoId;
        }
    }

    /// <summary>
    /// User activity statistics.
    /// </summary>
    public class Statistics
    {
        /// <summary>
        /// View count.
        /// </summary>
        public long ViewCount { get; }

        /// <summary>
        /// Like count.
        /// </summary>
        public long LikeCount { get; }

        /// <summary>
        /// Dislike count.
        /// </summary>
        public long DislikeCount { get; }

        /// <summary>
        /// Average user rating in stars (1 star to 5 stars).
        /// </summary>
        public double AverageRating
        {
            get
            {
                if (LikeCount + DislikeCount == 0) return 0;
                return 1 + 4.0 * LikeCount / (LikeCount + DislikeCount);
            }
        }

        /// <summary />
        public Statistics(long viewCount, long likeCount, long dislikeCount)
        {
            ViewCount = viewCount;
            LikeCount = likeCount;
            DislikeCount = dislikeCount;
        }
    }

    /// <summary>
    /// Information about a YouTube video.
    /// </summary>
    public class oVideo
    {
        /// <summary>
        /// ID of this video.
        /// </summary> 
        public string Id { get; }

        /// <summary>
        /// Author of this video.
        /// </summary> 
        public string Author { get; }

        /// <summary>
        /// Upload date of this video.
        /// </summary>
        public DateTimeOffset UploadDate { get; }

        /// <summary>
        /// Title of this video.
        /// </summary> 
        public string Title { get; }

        /// <summary>
        /// Description of this video.
        /// </summary> 
        public string Description { get; }

        /// <summary>
        /// Thumbnails of this video.
        /// </summary> 
        public ThumbnailSet Thumbnails { get; }

        /// <summary>
        /// Duration of this video.
        /// </summary>
        public TimeSpan Duration { get; }

        /// <summary>
        /// Search keywords of this video.
        /// </summary> 
        public string[] Keywords { get; }

        /// <summary>
        /// Statistics of this video.
        /// </summary> 
        public Statistics Statistics { get; }

        /// <summary />
        public oVideo(string id, string author, DateTimeOffset uploadDate, string title, string description,
            ThumbnailSet thumbnails, TimeSpan duration, string[] keywords, Statistics statistics)
        {
            Id = id;
            Author = author;
            UploadDate = uploadDate;
            Title = title;
            Description = description;
            Thumbnails = thumbnails;
            Duration = duration;
            Keywords = keywords;
            Statistics = statistics;
        }

        /// <inheritdoc />
        public override string ToString() => Title;
    }
}
