﻿namespace SentenceMining.Common
{
    public static class BlobHelper
    {
        public static string GetBlobAudioPath(string fileName)
        {
            var blobPath = Environment.GetEnvironmentVariable("BLOB_AUDIO_PATH") ?? throw new InvalidOperationException("BlobAudioPath environment variable is not set.");
            return $"{blobPath}{fileName}";
        }
    }
}
