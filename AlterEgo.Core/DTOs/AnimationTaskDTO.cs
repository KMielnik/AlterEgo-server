﻿using AlterEgo.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AlterEgo.Core.DTOs
{
    public class AnimationTaskDTO
    {
        /// <summary>
        /// Unique identyficator of task.
        /// </summary>
        /// <example>82cfd7cf-73b1-4184-8cdd-c304357b0502</example>
        public Guid Id { get; init; }

        /// <summary>
        /// Login of owner of task.
        /// </summary>
        /// <example>login123</example>
        public string Owner { get; init; }

        /// <summary>
        /// MediaFileInfo of source video
        /// </summary>
        public MediaFileInfo SourceVideo { get; init; }

        /// <summary>
        /// MediaFileInfo of source image
        /// </summary>
        public MediaFileInfo SourceImage { get; init; }

        /// <summary>
        /// MediaFileInfo of result video
        /// </summary>
        public MediaFileInfo ResultAnimation { get; init; }

        /// <summary>
        /// Value indicating if audio retention was requested.
        /// </summary>
        /// <example>true</example>
        public bool RetainAudio { get; init; }

        /// <summary>
        /// Amount of image padding (scale 0.0-1.0) in animation requested.
        /// </summary>
        /// <example>0.2</example>
        public float ImagePadding { get; init; }

        /// <summary>
        /// Date of registering task in database.
        /// </summary>
        public DateTime CreatedAt { get; init; }

        /// <summary>
        /// Actual status of task.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AnimationTask.Statuses Status { get; init; }
    }
}
