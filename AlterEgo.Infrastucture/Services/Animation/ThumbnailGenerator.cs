using AlterEgo.Core.Interfaces.Animation;
using AlterEgo.Core.Settings;
using AlterEgo.Infrastructure.Exceptions;
using FFmpeg.NET;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace AlterEgo.Infrastructure.Services.Animation
{
    public class ThumbnailGenerator : IThumbnailGenerator
    {
        private readonly ILogger<ThumbnailGenerator> _logger;
        private readonly FFmpegSettings _ffmpegSettings;
        private Engine _ffmpeg;

        public ThumbnailGenerator(
            ILogger<ThumbnailGenerator> logger, 
            IOptions<FFmpegSettings> ffmpegSettings)
        {
            _logger = logger;
            _ffmpegSettings = ffmpegSettings.Value;

            InitializeFFmpeg();
        }

        private void InitializeFFmpeg()
        {
            try
            {
                _ffmpeg = new Engine(_ffmpegSettings.FFmpegLocation);
            }
            catch (ArgumentException ex)
            {
                _logger.LogCritical(ex, "Couldn't find ffmpeg executable, make sure you inserted proper one in settings");

                throw new MissingConfigurationSetting(nameof(_ffmpegSettings.FFmpegLocation), nameof(FFmpegSettings));
            }
        }

        private (int targetWidth, int targetHeight) GetThumbnailResolution(int originalWidth, int originalHeight)
        {
            var ratio =  ((double)originalWidth) / originalHeight;

            var baseSize = Math.Min(256, Math.Max(originalWidth, originalHeight));

            return ratio < 1 ?
                ((int)(baseSize * ratio), baseSize) :
                (baseSize, (int)(baseSize / ratio));
        }

        public async Task<byte[]> GetThumbnailAsync(string filepath)
        => Path.GetExtension(filepath) switch
        {
            ".jpg" or ".jpeg" or ".png" => await GenerateThumbnailFromImage(filepath),
            ".mp4" => await GenerateThumbnailFromVideo(filepath),
            _ => throw new UnsupportedMediaTypeException(filepath)
        };

        private Task<byte[]> GenerateThumbnailFromImage(string filepath)
        {
            using var ms = new MemoryStream();

            var image = Image.FromFile(filepath);
            var originalDimensions = image.Size;

            var thumbnailDimensions = GetThumbnailResolution(originalDimensions.Width, originalDimensions.Height);

            _logger.LogDebug("Generating thumbnail for {FilePath}, from resolution {@OriginalResolution} to {@TargetResolution}", filepath, originalDimensions, thumbnailDimensions);

            using var thumbnail = image.GetThumbnailImage(
                thumbnailDimensions.targetWidth, 
                thumbnailDimensions.targetHeight, 
                null, 
                new IntPtr());

            thumbnail.Save(ms, ImageFormat.Jpeg);

            return Task.FromResult(ms.ToArray());
        }

        private async Task<byte[]> GenerateThumbnailFromVideo(string filepath)
        {
            var input = new MediaFile(filepath);

            var outputPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()) + ".jpg";
            var output = new MediaFile(outputPath);

            var originalSize = (await _ffmpeg.GetMetaDataAsync(input))
                .VideoData
                .FrameSize
                .Split('x')
                .Select(x => int.Parse(x))
                .ToList();

            var targetSize = GetThumbnailResolution(originalSize[1], originalSize[0]);

            _logger.LogDebug("Original video size: {@OriginalSize},  target thumbnail size {TargetSize}", originalSize, targetSize);

            _logger.LogDebug("Starting generation of thumbnail in {ThumbnailPath} for video {VideoPath}", outputPath, filepath);

            await _ffmpeg.GetThumbnailAsync(input, 
                output,
                new ConversionOptions 
                { 
                    Seek = TimeSpan.Zero,
                    VideoSize = FFmpeg.NET.Enums.VideoSize.Custom,
                    CustomWidth = targetSize.targetWidth,
                    CustomHeight = targetSize.targetHeight
                });

            _logger.LogDebug("Thumbnail generated");

            byte[] thumbnailByteArray;

            using (var thumbnailFile = File.OpenRead(outputPath))
            {
                thumbnailByteArray = new byte[thumbnailFile.Length];
                await thumbnailFile.ReadAsync(thumbnailByteArray, 0, thumbnailByteArray.Length);
            }

            File.Delete(outputPath);
            _logger.LogDebug("Transferred thumbnail to byte array and deleted the temp file.");

            return thumbnailByteArray;
        }
    }
}
