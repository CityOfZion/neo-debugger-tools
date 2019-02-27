﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Legacy;
using System.Reactive.Linq;
using NeoDebuggerUI.Models;
using NeoDebuggerUI.Views;
using System.Reactive;
using NeoDebuggerCore.Utils;

namespace NeoDebuggerUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ReactiveList<string> ProjectFiles { get; } = new ReactiveList<string>();
        public delegate void SelectedFileChanged(string selectedFilename);
        public event SelectedFileChanged EvtFileChanged;

        public delegate void FileToCompileChanged();
        public event FileToCompileChanged EvtFileToCompileChanged;

        //Mega anti pattern
        public Window MainWindow { get; set; }

        private string _selectedFile;
		public string SelectedFile
		{
			get => _selectedFile;
			set => this.RaiseAndSetIfChanged(ref _selectedFile, value);
		}

		private string _log;
		public string Log
		{
			get => _log;
			set => this.RaiseAndSetIfChanged(ref _log, value);
		}

        private string _consumedGas;
        public string ConsumedGas
        {
            get => _consumedGas;
            set => this.RaiseAndSetIfChanged(ref _consumedGas, value);
        }

        private string _fileFolder;
		private DateTime _lastModificationDate;

		public MainWindowViewModel()
		{
			Log = "Debugger started\n";
			Neo.Emulation.API.Runtime.OnLogMessage = SendLogToPanel;
			DebuggerStore.instance.manager.SendToLog += (o, e) => { SendLogToPanel(e.Message); };

            var fileChanged = this.WhenAnyValue(vm => vm.SelectedFile);
			fileChanged.Subscribe(file => LoadSelectedFile());
		}

        private Unit LoadSelectedFile()
        {
            EvtFileChanged?.Invoke(_selectedFile);
            return Unit.Default;
        }

        public void SaveCurrentFileWithContent(string content)
        {
            File.WriteAllText(this.SelectedFile, content);
        }

        private bool LoadContract(string avmFilePath)
        {
            if (!DebuggerStore.instance.manager.LoadContract(avmFilePath))
            {
                return false;
            }

            _lastModificationDate = File.GetLastWriteTime(DebuggerStore.instance.manager.AvmFilePath);
            DebuggerStore.instance.manager.Emulator.ClearAssignments();

            if (DebuggerStore.instance.manager.IsMapLoaded)
            {
                ProjectFiles.Clear();
                _fileFolder = Path.GetDirectoryName(avmFilePath);
                ProjectFiles.Add(Path.GetFileName(avmFilePath));
                foreach (var path in DebuggerStore.instance.manager.Map.FileNames)
                {
                    DebuggerStore.instance.manager.LoadAssignmentsFromContent(path);
                    ProjectFiles.Add(path);
                }

                SelectedFile = avmFilePath;
            }

            return true;
        }

        internal void ResetWithNewFile(string result)
        {
            if (!result.EndsWith(".cs", StringComparison.Ordinal))
            {
                SendLogToPanel("File not supported. Please use .cs extension.");
                return;
            }

            if (!File.Exists(result))
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Resources");
                var fullFilePath = Path.Combine(path, "ContractTemplate.cs");
                var sourceCode = File.ReadAllText(fullFilePath);
                File.WriteAllText(result, sourceCode);
            }

            this.ProjectFiles.Clear();
            this.ProjectFiles.Add(result);
            this.SelectedFile = ProjectFiles[0];
            CompileCurrentFile();
        }


        //Current compiler does not support multiple files
        public void CompileCurrentFile()
        {
            EvtFileToCompileChanged?.Invoke();
            var sourceCode = File.ReadAllText(this.SelectedFile);
            var compiled = DebuggerStore.instance.manager.CompileContract(sourceCode, Neo.Debugger.Core.Data.SourceLanguage.CSharp ,this.SelectedFile);
            if(compiled)
            {
                DebuggerStore.instance.manager.LoadContract(this.SelectedFile.Replace(".cs", ".avm"));
            }
        }

        public void SendLogToPanel(string s)
        {
            Log += s + "\n";
        }

        public void ClearLog()
        {
            Log = "";
        }

        public async Task Open()
		{
			var dialog = new OpenFileDialog();
			var filters = new List<FileDialogFilter>();
			var filteredExtensions = new List<string>(new string[] { "avm" });
			var filter = new FileDialogFilter { Extensions = filteredExtensions, Name = "NEO AVM files" };
			filters.Add(filter);
			dialog.Filters = filters;
			dialog.AllowMultiple = false;

			var result = await dialog.ShowAsync(this.MainWindow);

			if (result != null && result.Length > 0)
			{
				LoadContract(result[0]);
			}
		}

		public async Task OpenRunDialog()
		{
            CompileCurrentFile();
			var modalWindow = new InvokeWindow();
			var task = modalWindow.ShowDialog(this.MainWindow);
			await Task.Run(()=> task.Wait());

            ConsumedGas = DebuggerStore.instance.UsedGasCost;
        }
    }
}
