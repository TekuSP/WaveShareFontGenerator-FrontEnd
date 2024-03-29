﻿using Microsoft.Win32;

using SlavaGu.ConsoleAppLauncher;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WaveShareFontGenerator_FrontEnd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public Dictionary<string, string> KnownFonts { get; set; } //Path, name
        PrivateFontCollection PrivateFontCollection { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            PrivateFontCollection = new PrivateFontCollection();
            KnownFonts = new Dictionary<string, string>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void knownFonts_Loaded(object sender, RoutedEventArgs e)
        {
            var knownFonts = Directory.GetFiles("C:\\Windows\\Fonts", "*.ttf");
            PrivateFontCollection fonts = new PrivateFontCollection();
            foreach (var item in knownFonts)
            {
                fonts.AddFontFile(item);
                if (!PrivateFontCollection.Families.Any(x => x.Name == fonts.Families.First().Name))
                {
                    KnownFonts.Add(item, fonts.Families.First().Name);
                    PrivateFontCollection.AddFontFile(item);
                }
                fonts = new PrivateFontCollection();
            }
            this.knownFonts.SelectedIndex = 0;
        }

        private void generate_Click(object sender, RoutedEventArgs e)
        {
            previewContent.Text = "";
            generate.IsEnabled = false;
            var result = ConsoleApp.Run(System.IO.Path.GetFullPath(".\\CLI\\waveshareFontGenerator-CLI.exe"), $"/s {fontSize.Text} /f {((KeyValuePair<string, string>)knownFonts.SelectedItem).Key} /x {xOffset.Text} /y {yOffset.Text} /w {fontWidth.Text} /height {fontHeight.Text}");
            previewContent.Text = result.Output;
            generate.IsEnabled = true;
        }
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //NUMBERS ONLY
        private void VerifyNumber(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                AddExtension = true,
                Filter = "C++ file (*.cpp)|*.cpp",
                Title = "Save generated font",
                CheckPathExists = true,
                OverwritePrompt = true
            };
            if (saveFileDialog.ShowDialog().GetValueOrDefault())
            {
                File.WriteAllText(saveFileDialog.FileName, previewContent.Text);
            }
        }

        private void open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                AddExtension = true,
                Filter = "TTF Font file (*.ttf)|*.ttf",
                CheckFileExists = true,
                CheckPathExists = true,
                ReadOnlyChecked = true,
                ShowReadOnly = true,
                Title = "Select font file"
            };
            if (openFileDialog.ShowDialog().GetValueOrDefault())
            {
                PrivateFontCollection fonts = new PrivateFontCollection();
                fonts.AddFontFile(openFileDialog.FileName);
                if (!PrivateFontCollection.Families.Any(x => x.Name == fonts.Families.First().Name))
                {
                    KnownFonts.Add(openFileDialog.FileName, fonts.Families.First().Name);
                    PrivateFontCollection.AddFontFile(openFileDialog.FileName);
                    this.knownFonts.SelectedItem = KnownFonts.Last();
                }
                else
                {
                    var where = KnownFonts.Where(x => x.Value == fonts.Families.First().Name);
                    if (where.Any())
                    {
                        this.knownFonts.SelectedItem = where.First();
                    }
                }
            }
        }
    }
}
