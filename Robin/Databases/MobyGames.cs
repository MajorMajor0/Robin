using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace Robin;

internal class MobyGames : IDB
{
	private const string baseUrl = @"https://api.mobygames.com/v1/";
	private readonly string endUrl = $"api_key={Keys.MobyGames}";
	public LocalDB DB => throw new NotImplementedException();

	public string Title => throw new NotImplementedException();

	public IEnumerable<IDbPlatform> Platforms => throw new NotImplementedException();

	public IEnumerable<IDbRelease> Releases => throw new NotImplementedException();

	public bool HasRegions => throw new NotImplementedException();

	public void CachePlatformData(Platform platform)
	{
		MessageBox.Show("MobyGames does not provide platform data.", "Cannot Cache Data", MessageBoxButton.OK);
	}

	public async void CachePlatformGamesAsync(Platform platform)
	{
		if (platform.ID_MB == null)
		{
			return;
		}

		using HttpClient client = new();
		long platformId = (long)platform.ID_MB;
		int offset = 0;
		bool keepGoing = true;

		JsonElement gamesElement;
		JsonElement gameElement = new();
		string url = "";
		string key = "";

		while (keepGoing)
		{
			try
			{
				// https://api.mobygames.com/v1/games?format=brief&api_key=4NKaKoCp6BGhL/tkqlnnxQ==
				url = $"{baseUrl}games?format=normal&platform={platformId}&offset={offset}&{endUrl}";
				//url = HttpUtility.UrlEncode(url);

				var response = await client.GetAsync(url);
				var pageContents = await response?.Content?.ReadAsStreamAsync();

				// Get the array of games from the json document
				gamesElement = JsonDocument
					.Parse(pageContents)
					.RootElement
					.GetProperty("games");

				// Create an array of Mbgames
				var elements = gamesElement.EnumerateArray();

				// This is at least second to last time
				if (elements.Count() < 100)
				{
					keepGoing = false;
				}

				// This is the last time
				if (!elements.Any())
				{
					break;
				}

				// Get an MBGame out of the element
				foreach (var element in elements)
				{
					// Store off for exception handling
					gameElement = element;

					// Get the ID
					key = "game_id";
					int gameId = element.GetProperty(key).GetInt32();

					// Get the game out of the database
					Mbgame mbGame = R.Data.Mbgames
						.Local
						.FirstOrDefault(x => x.Id == gameId);

					// If the game isn't found, create a new one and add it
					if (mbGame is null)
					{
						key = "game_id, title, or description";
						mbGame = new()
						{
							Id = element.GetProperty("game_id").GetInt32(),
							Title = element.GetProperty("title").GetString(),
							Overview = element.GetProperty("description").GetString(),
							MbplatformId = platformId,
						};

						R.Data.Mbgames.Add(mbGame);
					}

					// Get genres
					key = "genres";
					var genreElements = element
						.GetProperty(key)
						.EnumerateArray();

					foreach (var genreElement in genreElements)
					{
						key = "genre_id";
						int genreId = genreElement.GetProperty(key).GetInt32();
						var genre = R.Data.Mbgenres.Local.FirstOrDefault(x => x.Id == genreId);

						if (genre is not null)
						{
							mbGame.Mbgenres.Add(genre);
						}
					}
					Debug.WriteLine(mbGame.Id);

					if (43384 == gameId)
					{

					}

					// Get the release date for this platform
					key = "platforms, platform_id, or first_release_date";
					mbGame.Date = element
						.GetProperty("platforms")
						.EnumerateArray()
						.FirstOrDefault(x => x.GetProperty("platform_id").GetInt32() == platformId)
						.GetProperty("first_release_date")
						.GetMbDate();
				}

				R.Save();
				offset += 100;
			}

			catch (InvalidOperationException ex)
			{
				// The request failed due to an underlying issue such as network connectivity, DNS
				// failure, server certificate validation or timeout.

				// This value's System.Text.Json.JsonElement.ValueKind is not System.Text.Json.JsonValueKind.Object.

				string caption = ex.GetType().ToString();
				string text = $"In {nameof(CachePlatforms)}\n\n {ex.Message}\n\nURL: {url}\n\nFailed Json Element:\n{gameElement.GetRawText()}";
				MessageBox.Show(text, caption, MessageBoxButton.OK);
			}
			catch (HttpRequestException ex)
			{
				// The request failed due to an underlying issue such as network connectivity, DNS
				// failure, server certificate validation or timeout.
				string caption = ex.GetType().ToString();
				string text = $"In {nameof(CachePlatforms)}\n\n {ex.Message}\n\nURL: {url}";
				MessageBox.Show(text, caption, MessageBoxButton.OK);
			}
			catch (JsonException ex)
			{
				// utf8Json does not represent a valid single JSON value.
				string caption = ex.GetType().ToString();
				string text = $"In {nameof(CachePlatforms)}\n\n {ex.Message}\n\nURL: {url}\n\nFailed Json Element:\n{gameElement.GetRawText()}";
				MessageBox.Show(text, caption, MessageBoxButton.OK);
			}
			catch (KeyNotFoundException ex)
			{
				string caption = ex.GetType().ToString();
				string text = $"In {nameof(CachePlatforms)}\n\n {ex.Message}\n\nKey: {key}\n\nURL: {url}\n\nFailed Json Element:\n{gameElement.GetRawText()}";
				MessageBox.Show(text, caption, MessageBoxButton.OK);
			}
			catch (FormatException ex)
			{
				string caption = ex.GetType().ToString();
				string text = $"In {nameof(CachePlatforms)}\n\n {ex.Message}\n\nURL: {url}\n\nFailed Json Element:\n{gameElement.GetRawText()}";
				MessageBox.Show(text, caption, MessageBoxButton.OK);
			}
			catch (Exception ex)
			{
				string caption = ex.GetType().ToString();
				string text = $"In {nameof(CachePlatforms)}\n\n {ex.Message}\n\nURL: {url}\n\nFailed Json Element:\n{gameElement.GetRawText()}";
				MessageBox.Show(text, caption, MessageBoxButton.OK);
			}
		}
	}

	public async void CachePlatformReleasesAsync(Platform platform)
	{
		if (platform.ID_MB == null)
		{
			return;
		}

		using HttpClient client = new();
		long platformId = (long)platform.ID_MB;
		int offset = 0;
		bool keepGoing = true;

		JsonElement gamesElement;
		JsonElement gameElement = new();
		string url = "";
		string key = "";

		while (keepGoing)
		{
			try
			{
				// https://api.mobygames.com/v1/games?format=brief&api_key=4NKaKoCp6BGhL/tkqlnnxQ==
				url = $"{baseUrl}games?format=normal&platform={platformId}&offset={offset}&{endUrl}";
				//url = HttpUtility.UrlEncode(url);

				var response = await client.GetAsync(url);
				var pageContents = await response?.Content?.ReadAsStreamAsync();

				// Get the array of games from the json document
				gamesElement = JsonDocument
					.Parse(pageContents)
					.RootElement
					.GetProperty("games");

				// Create an array of Mbgames
				var elements = gamesElement.EnumerateArray();

				// This is at least second to last time
				if (elements.Count() < 100)
				{
					keepGoing = false;
				}

				// This is the last time
				if (!elements.Any())
				{
					break;
				}

				// Get an MBGame out of the element
				foreach (var element in elements)
				{
					// Store off for exception handling
					gameElement = element;

					// Get the ID
					key = "game_id";
					int gameId = element.GetProperty(key).GetInt32();

					// Get the game out of the database
					Mbgame mbGame = R.Data.Mbgames
						.Local
						.FirstOrDefault(x => x.Id == gameId);

					// If the game isn't found, create a new one and add it
					if (mbGame is null)
					{
						key = "game_id, title, or description";
						mbGame = new()
						{
							Id = element.GetProperty("game_id").GetInt32(),
							Title = element.GetProperty("title").GetString(),
							Overview = element.GetProperty("description").GetString(),
							MbplatformId = platformId,
						};

						R.Data.Mbgames.Add(mbGame);
					}

					// Get genres
					key = "genres";
					var genreElements = element
						.GetProperty(key)
						.EnumerateArray();

					foreach (var genreElement in genreElements)
					{
						key = "genre_id";
						int genreId = genreElement.GetProperty(key).GetInt32();
						var genre = R.Data.Mbgenres.Local.FirstOrDefault(x => x.Id == genreId);

						if (genre is not null)
						{
							mbGame.Mbgenres.Add(genre);
						}
					}
					Debug.WriteLine(mbGame.Id);

					if (43384 == gameId)
					{

					}

					// Get the release date for this platform
					key = "platforms, platform_id, or first_release_date";
					mbGame.Date = element
						.GetProperty("platforms")
						.EnumerateArray()
						.FirstOrDefault(x => x.GetProperty("platform_id").GetInt32() == platformId)
						.GetProperty("first_release_date")
						.GetMbDate();
				}

				R.Save();
				offset += 100;
			}

			catch (InvalidOperationException ex)
			{
				// The request failed due to an underlying issue such as network connectivity, DNS
				// failure, server certificate validation or timeout.

				// This value's System.Text.Json.JsonElement.ValueKind is not System.Text.Json.JsonValueKind.Object.

				string caption = ex.GetType().ToString();
				string text = $"In {nameof(CachePlatforms)}\n\n {ex.Message}\n\nURL: {url}\n\nFailed Json Element:\n{gameElement.GetRawText()}";
				MessageBox.Show(text, caption, MessageBoxButton.OK);
			}
			catch (HttpRequestException ex)
			{
				// The request failed due to an underlying issue such as network connectivity, DNS
				// failure, server certificate validation or timeout.
				string caption = ex.GetType().ToString();
				string text = $"In {nameof(CachePlatforms)}\n\n {ex.Message}\n\nURL: {url}";
				MessageBox.Show(text, caption, MessageBoxButton.OK);
			}
			catch (JsonException ex)
			{
				// utf8Json does not represent a valid single JSON value.
				string caption = ex.GetType().ToString();
				string text = $"In {nameof(CachePlatforms)}\n\n {ex.Message}\n\nURL: {url}\n\nFailed Json Element:\n{gameElement.GetRawText()}";
				MessageBox.Show(text, caption, MessageBoxButton.OK);
			}
			catch (KeyNotFoundException ex)
			{
				string caption = ex.GetType().ToString();
				string text = $"In {nameof(CachePlatforms)}\n\n {ex.Message}\n\nKey: {key}\n\nURL: {url}\n\nFailed Json Element:\n{gameElement.GetRawText()}";
				MessageBox.Show(text, caption, MessageBoxButton.OK);
			}
			catch (FormatException ex)
			{
				string caption = ex.GetType().ToString();
				string text = $"In {nameof(CachePlatforms)}\n\n {ex.Message}\n\nURL: {url}\n\nFailed Json Element:\n{gameElement.GetRawText()}";
				MessageBox.Show(text, caption, MessageBoxButton.OK);
			}
			catch (Exception ex)
			{
				string caption = ex.GetType().ToString();
				string text = $"In {nameof(CachePlatforms)}\n\n {ex.Message}\n\nURL: {url}\n\nFailed Json Element:\n{gameElement.GetRawText()}";
				MessageBox.Show(text, caption, MessageBoxButton.OK);
			}
		}
	}


	public async void CachePlatforms()
	{
		using HttpClient client = new();

		string url = $"{baseUrl}platforms?{endUrl}";
		//url = HttpUtility.UrlEncode(url);

		try
		{
			var response = await client.GetAsync(url);
			var pageContents = await response?.Content?.ReadAsStreamAsync();

			// Get the array of platforms from the json document
			JsonElement platforms = JsonDocument
				.Parse(pageContents)
				.RootElement
				.GetProperty("platforms");

			// Create an array of MbPlatforms
			var platformsList = platforms
				.EnumerateArray()
				.Select(x => new Mbplatform
				{
					Id = x.GetProperty("platform_id").GetInt32(),
					Title = x.GetProperty("platform_name").GetString(),
					CacheDate = new DateTime(1900, 1, 1)
				});

			// Remove any platforms that are not already in the database
			platformsList = platformsList
				.Where(x => !R.Data.Mbplatforms.Local.Any(y => y.Id == x.Id));

			R.Data.Mbplatforms.AddRange(platformsList);
			R.Save();
		}

		catch (InvalidOperationException ex)
		{
			// The request failed due to an underlying issue such as network connectivity, DNS
			// failure, server certificate validation or timeout.
			string caption = nameof(ex);
			string text = $"In {nameof(CachePlatforms)}\n\n {ex.Message}";
			MessageBox.Show(text, caption, MessageBoxButton.OK);
		}
		catch (HttpRequestException ex)
		{
			// The request failed due to an underlying issue such as network connectivity, DNS
			// failure, server certificate validation or timeout.
			string caption = nameof(ex);
			string text = $"In {nameof(CachePlatforms)}\n\n {ex.Message}";
			MessageBox.Show(text, caption, MessageBoxButton.OK);
		}
		catch (JsonException ex)
		{
			// utf8Json does not represent a valid single JSON value.
			string caption = nameof(ex);
			string text = $"In {nameof(CachePlatforms)}\n\n {ex.Message}";
			MessageBox.Show(text, caption, MessageBoxButton.OK);
		}
		catch (KeyNotFoundException ex)
		{
			string caption = nameof(ex);
			string text = $"In {nameof(CachePlatforms)}\n\n {ex.Message}";
			MessageBox.Show(text, caption, MessageBoxButton.OK);
		}
		catch (Exception ex)
		{
			string caption = nameof(ex);
			string text = $"In {nameof(CachePlatforms)}\n\n {ex.Message}";
			MessageBox.Show(text, caption, MessageBoxButton.OK);
		}
	}

	public async void CacheGenres()
	{
		using HttpClient client = new();

		string url = $"{baseUrl}genres?{endUrl}";
		//url = HttpUtility.UrlEncode(url);

		try
		{
			R.Data.Mbgenres.Load();

			var response = await client.GetAsync(url);
			var pageContents = await response?.Content?.ReadAsStreamAsync();

			// Get the array of platforms from the json document
			JsonElement genres = JsonDocument
				.Parse(pageContents)
				.RootElement
				.GetProperty("genres");

			// Create an array of MbPlatforms
			var genreList = genres
				.EnumerateArray()
				.Select(x => new Mbgenre
				{
					Id = x.GetProperty("genre_id").GetInt32(),
					CategoryId = x.GetProperty("genre_category_id").GetInt32(),
					Name = x.GetProperty("genre_name").GetString(),
					Description = x.GetProperty("genre_description").GetString(),
					Category = x.GetProperty("genre_category").GetString(),
				});

			// Remove any platforms that are not already in the database
			genreList = genreList
				.Where(x => !R.Data.Mbgenres.Local.Any(y => y.Id == x.Id));

			R.Data.Mbgenres.AddRange(genreList);
			R.Save();
		}

		catch (InvalidOperationException ex)
		{
			// The request failed due to an underlying issue such as network connectivity, DNS
			// failure, server certificate validation or timeout.
			string caption = nameof(ex);
			string text = $"In {nameof(CachePlatforms)}\n\n {ex.Message}";
			MessageBox.Show(text, caption, MessageBoxButton.OK);
		}
		catch (HttpRequestException ex)
		{
			// The request failed due to an underlying issue such as network connectivity, DNS
			// failure, server certificate validation or timeout.
			string caption = nameof(ex);
			string text = $"In {nameof(CachePlatforms)}\n\n {ex.Message}";
			MessageBox.Show(text, caption, MessageBoxButton.OK);
		}
		catch (JsonException ex)
		{
			// utf8Json does not represent a valid single JSON value.
			string caption = nameof(ex);
			string text = $"In {nameof(CachePlatforms)}\n\n {ex.Message}";
			MessageBox.Show(text, caption, MessageBoxButton.OK);
		}
		catch (KeyNotFoundException ex)
		{
			string caption = nameof(ex);
			string text = $"In {nameof(CachePlatforms)}\n\n {ex.Message}";
			MessageBox.Show(text, caption, MessageBoxButton.OK);
		}
		catch (Exception ex)
		{
			string caption = nameof(ex);
			string text = $"In {nameof(CachePlatforms)}\n\n {ex.Message}";
			MessageBox.Show(text, caption, MessageBoxButton.OK);
		}
	}


	public void Dispose()
	{
		throw new NotImplementedException();
	}

	public void ReportUpdates(bool detect)
	{
		throw new NotImplementedException();
	}
}

