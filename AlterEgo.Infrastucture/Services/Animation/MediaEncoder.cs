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
    public class MediaEncoder : IMediaEncoder
    {
        private readonly ILogger<MediaEncoder> _logger;
        private readonly FFmpegSettings _ffmpegSettings;
        private Engine _ffmpeg;

        public MediaEncoder(
            ILogger<MediaEncoder> logger,
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
            var ratio = ((double)originalWidth) / originalHeight;

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

            const int ExifOrientationId = 0x112;
            if (Array.IndexOf(image.PropertyIdList, ExifOrientationId) > -1)
            {
                int orientation;

                orientation = image.GetPropertyItem(ExifOrientationId).Value.Last();

                if (orientation >= 1 && orientation <= 8)
                {
                    switch (orientation)
                    {
                        case 2:
                            image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            break;
                        case 3:
                            image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case 4:
                            image.RotateFlip(RotateFlipType.Rotate180FlipX);
                            break;
                        case 5:
                            image.RotateFlip(RotateFlipType.Rotate90FlipX);
                            break;
                        case 6:
                            image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        case 7:
                            image.RotateFlip(RotateFlipType.Rotate270FlipX);
                            break;
                        case 8:
                            image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                    }

                    image.RemovePropertyItem(ExifOrientationId);
                }
            }

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

            _logger.LogCritical("{@Size}", originalSize);

            var targetSize = GetThumbnailResolution(originalSize[0], originalSize[1]);

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

        public async Task<string> ReencodeMedia(string filePath)
        {
            var tempFilePath = Path.Join(Path.GetTempPath(), Path.GetFileName(filePath));

            await _ffmpeg.ExecuteAsync($" -i {filePath} {tempFilePath}");

            File.Move(tempFilePath, filePath, true);

            return filePath;
        }
    }
}
