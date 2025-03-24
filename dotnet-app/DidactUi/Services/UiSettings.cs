﻿using System.Text.Json.Serialization;

namespace DidactUi.Services
{
    public class UiSettings
    {
        [JsonPropertyName("didactEngineBaseUrl")]
        public required string DidactEngineBaseUrl { get; set; }
    }
}