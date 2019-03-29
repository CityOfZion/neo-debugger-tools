using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using NeoDebuggerUI.ViewModels;
using ReactiveUI;
using System.IO;
using System.Linq;
using Avalonia.Layout;
using LunarLabs.Parser;
using Neo.Debugger.Core.Utils;
using Neo.Emulation;
using Neo.Lux.Utils;
using NeoDebuggerUI.Models;
using Neo.Emulation.API;
using System.Numerics;

namespace NeoDebuggerUI.Views
{
    public class InvokeWindow : ReactiveWindow<InvokeWindowViewModel>
    {
        public InvokeWindow(bool stepping, bool useOffset)
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            ViewModel.Stepping = stepping;
            ViewModel.UseOffset = useOffset;
            RenderTestCaseParams(ViewModel.SelectedTestCaseParams);
            RegisterInteraction();
            RegisterEventListeners();
            if(DebuggerStore.instance.manager.IsSteppingOrOnBreakpoint)
            {
                ViewModel.RunOrStep();
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void RenderTestCaseParams(DataNode tparams)
        {
            var grid = this.FindControl<Grid>("InputParametersGrid");
            grid.Children.Clear();
            grid.RowDefinitions.Clear();

            var rowHeader = new RowDefinition { Height = new GridLength(20) };
            grid.RowDefinitions.Add(rowHeader);

            var paramHeader = new TextBlock { Text = "Parameter", FontWeight = FontWeight.Bold, TextAlignment = TextAlignment.Center };
            Grid.SetRow(paramHeader, 0);
            Grid.SetColumn(paramHeader, 0);
            grid.Children.Add(paramHeader);

            var valueHeader = new TextBlock { Text = "Value", FontWeight = FontWeight.Bold, TextAlignment = TextAlignment.Center };
            Grid.SetRow(valueHeader, 0);
            Grid.SetColumn(valueHeader, 1);
            grid.Children.Add(valueHeader);

            var p1 = tparams != null ? DebuggerUtils.ParseNode(tparams[0], 0) : "";
            var p2 = tparams != null ? DebuggerUtils.ParseNode(tparams[1], 1) : "";

            RenderLine(grid, 1, "operation", p1);
            RenderLine(grid, 2, "args", p2);
        }

        private void RenderLine(Grid grid, int rowCount, string v1, string v2)
        {
            var rowView = new RowDefinition {Height = new GridLength(30)};
            grid.RowDefinitions.Add(rowView);
                
            var v1View = new TextBlock
            {
                Text = v1,
                TextAlignment = TextAlignment.Right, 
                VerticalAlignment = VerticalAlignment.Center, 
                Margin = Thickness.Parse("5")
            };
            Grid.SetRow(v1View, rowCount);
            Grid.SetColumn(v1View, 0);
            grid.Children.Add(v1View);

            var v2View = new TextBox {Text = v2};
            Grid.SetRow(v2View, rowCount);
            Grid.SetColumn(v2View, 1);
            grid.Children.Add(v2View);
        }

        private string ExtractValueFromGrid(int rowIndex, int colIndex)
        {
            var g = this.FindControl<Grid>("InputParametersGrid");

            foreach (var e in g.Children)
            {
                if (Grid.GetRow((AvaloniaObject) e) != rowIndex ||
                    Grid.GetColumn((AvaloniaObject) e) != colIndex) continue;
                
                if (e is TextBox box)
                {
                    return box.Text;
                }

                return "";
            }

            return "";
        }

        private void RegisterInteraction()
        {
            this.FindControl<Button>("DebugBtn").Click += (_,__) =>
            {
                var op = ExtractValueFromGrid(1, 1);
                var args = ExtractValueFromGrid(2, 1);

                if (!UseTestSequence())
                {
                    if (!SaveTransactionInfo())
                    {
                        return;
                    }
                    SaveOptions();
                }
                DebugPressed(op, args);
                Close();
            };
        }

        public void RegisterEventListeners()
        {
            this.ViewModel.EvtSelectedTestCaseChanged += (fileName) => RenderTestCaseParams(ViewModel.SelectedTestCaseParams);
            this.FindControl<Button>("AddPrivateKey").Click += (_, __) => ViewModel.AddPrivateKey();
            this.FindControl<Button>("RemovePrivateKey").Click += (_, __) => ViewModel.RemovePrivateKey();
        }

        public void DebugPressed(string field1, string field2)
        {
            ViewModel.DebugParams.ArgList = DebuggerUtils.GetArgsListAsNode(string.Concat(field1, ",", field2));
            ViewModel.RunOrStep();
        }
        
        public void SaveOptions()
        {
            //Get the witness mode
            CheckWitnessMode witnessMode;
            var selectedWitness = ViewModel.SelectedWitness;

            if (!Enum.TryParse<CheckWitnessMode>(selectedWitness, out witnessMode))
            {
                return;
            }
            ViewModel.DebugParams.WitnessMode = witnessMode;

            //Get the trigger type
            TriggerType type;
            var selectedTrigger = ViewModel.SelectedTrigger;
            
            if (!Enum.TryParse<TriggerType>(selectedTrigger, out type))
            {
                return;
            }
            ViewModel.DebugParams.TriggerType = type;
            //Get the timestamp
            ViewModel.DebugParams.Timestamp = ViewModel.Timestamp;

            //Get raw script
            var rawScriptText = this.FindControl<TextBox>("RawScriptText");
            var HasRawScript = rawScriptText.Text?.Length > 0;

            ViewModel.DebugParams.RawScript = HasRawScript ? rawScriptText.Text.HexToBytes() : null;
        }

        public bool SaveTransactionInfo()
        {
            var assetBox = this.FindControl<DropDown>("AssetBox");
            if (assetBox.SelectedIndex > 0)
            {
                foreach (var entry in Asset.Entries)
                {
                    if (entry.name == assetBox.SelectedItem.ToString())
                    {
                        BigInteger amount;
                        BigInteger.TryParse(ViewModel.AssetAmount, out amount);
                        if (amount > 0)
                        {
                            amount *= Asset.Decimals; // fix decimals

                            //Add the transaction info
                            ViewModel.DebugParams.Transaction.Add(entry.id, amount);
                        }
                        else
                        {
                            ViewModel.OpenGenericSampleDialog(entry.name + " amount must be greater than zero", "OK", "", false);
                            return false;
                        }

                        break;
                    }
                }
            }

            //Get the private key used
            var privateKey = ViewModel.SelectedPrivateKey;

            if (privateKey == "None")
            {
                privateKey = "";
            }
            ViewModel.DebugParams.PrivateKey = privateKey;

            return true;
        }

        public bool UseTestSequence()
        {
            var selectedTab = this.FindControl<TabControl>("TabPages").SelectedItem;
            var testSequencesTab = this.FindControl<TabItem>("TestSequencesTab");

            if(selectedTab == testSequencesTab)
            {
                return ViewModel.SelectedTestSequence != null;
            }

            ViewModel.SelectedTestSequence = null;
            return false;
        }
    }
}