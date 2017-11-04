﻿using GiantBombApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiantBombApi.Services
{
    public class VideoPlaybackPositionAgent
    {
        /// <summary>
        /// Syncs the playback position of a video with Giant Bomb
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="videoId"></param>
        /// <param name="playbackPositionInSeconds"></param>
        /// <returns></returns>
        public static async Task<bool> SetPlaybackPositionAsync(string apiKey, string videoId, int playbackPositionInSeconds)
        {
            bool success = false;

            try
            {
                if ((!String.IsNullOrWhiteSpace(videoId)) && (!String.IsNullOrWhiteSpace(apiKey)) && (playbackPositionInSeconds > 0))
                {
                    var uri = new Uri("https://www.giantbomb.com/api/video/save-time/?format=json&video_id=" + videoId + "&time_to_save=" + playbackPositionInSeconds + "&api_key=" + apiKey);
                    var response = await Utilities.HttpRequestAgent.GetDeserializedResponseAsync<PlaybackPositionResponse>(uri);

                    success = true;
                }
            }
            catch (Exception e)
            {
                Serilog.Log.Error(e, "Error saving playback position");
            }

            return success;
        }


        /// <summary>
        /// Pulls the playback position (in seconds) of a given video
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="videoId"></param>
        /// <returns>The previous playback position in seconds. -1 if no value was found.</returns>
        public static async Task<int> GetPlaybackPositionAsync(string apiKey, string videoId)
        {
            int previousPosition = -1;

            try
            {
                if ((!String.IsNullOrWhiteSpace(videoId)) && (!String.IsNullOrWhiteSpace(apiKey)))
                {
                    var uri = new Uri("https://www.giantbomb.com/api/video/get-saved-time/?format=json&video_id=" + videoId + "&api_key=" + apiKey);
                    var response = await Utilities.HttpRequestAgent.GetDeserializedResponseAsync<PlaybackPositionResponse>(uri);

                    if ((response != null) && (response.SavedTime > 0))
                    {
                        previousPosition = (int)response.SavedTime;
                    }
                }
            }
            catch (Exception e)
            {
                Serilog.Log.Error(e, "Error retrieving playback position");
            }

            return previousPosition;
        }


        /// <summary>
        /// Get all the video saved times for the user
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, int>> GetAllPlaybackPositionsAsync(string apiKey)
        {
            var allPlaybackPositions = new Dictionary<string, int>();

            try
            {
                if (!String.IsNullOrWhiteSpace(apiKey))
                {
                    var uri = new Uri("https://www.giantbomb.com/api/video/get-all-saved-times/?format=json&api_key=" + apiKey);
                    var response = await Utilities.HttpRequestAgent.GetDeserializedResponseAsync<AllPlaybackPositionsResponse>(uri);

                    if ((response != null) && (response.SavedTimes != null) && (response.SavedTimes.Count() > 0))
                    {
                        foreach (var playbackPosition in response.SavedTimes)
                        {
                            allPlaybackPositions.Add(playbackPosition.Id, (int)playbackPosition.SavedTime);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Serilog.Log.Error(e, "Error retrieving all playback positions");
            }

            return allPlaybackPositions;
        }
    }
}