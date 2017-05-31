/*This file is part of Robin.
 * 
 * Robin is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published 
 * version 3 of the License, or (at your option) any later version.
 * 
 * Robin is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU 
 * General Public License for more details. 
 * 
 * You should have received a copy of the GNU General Public License
 *  along with Robin.  If not, see<http://www.gnu.org/licenses/>.*/

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Robin
{
	public static class Reporter
	{
		public static ObservableCollection<Message> Messages { get; }

		private static string _newsFeed;
		public static string NewsFeed
		{
			get
			{
				return _newsFeed;
			}
			set
			{
				_newsFeed = value;
				NewsFeedChanged?.Invoke(null, EventArgs.Empty);
			}
		}

		static Reporter()
		{
			Messages = new ObservableCollection<Message>();
			Report("I'm not your guy, buddy.");
		}

		public static async void Report(string msg, Message.Sort sort = Message.Sort.Note)
		{
			await Task.Run(() =>
			{
				NewsFeed = msg;

				Message message = new Message(msg);
				message.MsgSort = sort;
				Messages.Add(message);
			});
		}

		public static void Warn(string message)
		{
			Report(message, Message.Sort.Warning);
		}

		public static void Error(string message)
		{
			Report(message, Message.Sort.Error);
		}

		public static async void ReportInline(string message)
		{
			await Task.Run(() =>
			{
				NewsFeed += message;
				Messages.Last().Msg += message;
			});
		}

		public class Message
		{
			public DateTime TimeStamp { get; set; }

			public string Msg { get; set; }

			public Sort MsgSort { get; set; }

			public enum Sort
			{
				[Description("Note")]
				Note = 0,
				[Description("Warning")]
				Warning = 1,
				[Description("Error")]
				Error = 2,
			}

			internal Message(string msg)
			{
				TimeStamp = DateTime.Now;
				Msg = Msg;
			}
		}

		public static event EventHandler NewsFeedChanged;
	}
}
