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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Robin
{
    /// <summary>
    /// Interaction logic for AboutBox.xaml
    /// </summary>
    public partial class AboutBox : Window
    {
        List<string> references;

        Assembly assembly => Assembly.GetExecutingAssembly();

        public string Company => ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyCompanyAttribute), false)).Company;

        public string Copyright => ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute), false)).Copyright;

        public string Version => assembly.GetName().Version.ToString();

        public string SoftwareTitle => ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyTitleAttribute), false)).Title;

        public AboutBox()
        {
            InitializeComponent();
            List<string> files = Directory.GetFiles(FileLocation.Folder, "*", searchOption: SearchOption.TopDirectoryOnly).Where(x => x.EndsWith(".dll") || x.EndsWith(".exe")).ToList();

            references = files.Select(x => FileVersionInfo.GetVersionInfo(x).ProductName + " " + FileVersionInfo.GetVersionInfo(x).ProductVersion + " " + FileVersionInfo.GetVersionInfo(x).LegalCopyright).Distinct().Where(x => !String.IsNullOrEmpty(x)).ToList();

            string robin = references.FirstOrDefault(x => x.StartsWith("Robin"));

            references.Remove(robin);

            ReferencesListbox.ItemsSource = references;
            Show();
            Activate();
        }
    }
}
