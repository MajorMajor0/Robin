using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robin.Core
{
	public static class AppSettings
	{
		static DisplayOption displayChoice = (DisplayOption)Properties.Settings.Default.DisplayChoice;
		public static DisplayOption DisplayChoice
		{
			get
			{
				return displayChoice;
			}
			set
			{
				displayChoice = value;
				Properties.Settings.Default.DisplayChoice = (int)value;
			}
		}

		public enum DisplayOption
		{
			[Description("Default")]
			Default,
			[Description("Box Front")]
			BoxFront,
			[Description("Box Back")]
			BoxBack,
			[Description("Screen")]
			Screen,
			[Description("Banner")]
			Banner,
		}

	}
}
