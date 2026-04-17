using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection; // Added for dynamic pathing
using System.IO; // Added for path combining
using ScriptPortal.Vegas;

namespace VelocityConverterExtension
{
    // EXTENSION MODULE
    public class VelocityConverterModule : ICustomCommandModule
    {
        Vegas myVegas;
        CustomCommand cmdView;
        CustomCommand cmdTools;

        public void InitializeModule(Vegas vegas)
        {
            myVegas = vegas;

            // find the folder where this .dll is installed
            string dllLocation = typeof(VelocityConverterModule).Assembly.Location;
            string extensionDirectory = Path.GetDirectoryName(dllLocation);
            string iconPath = Path.Combine(extensionDirectory, "VelocityConverter.png");

            // View > Extensions
            cmdView = new CustomCommand(CommandCategory.View, "VelocityConverter_View");
            cmdView.DisplayName = "Velocity Converter";
            cmdView.MenuItemName = "Velocity Converter";
            if (File.Exists(iconPath)) { cmdView.IconFile = iconPath; }
            cmdView.Invoked += HandleInvoked;
            cmdView.MenuPopup += HandleMenuPopup;

            // Tools > Extensions
            cmdTools = new CustomCommand(CommandCategory.Tools, "VelocityConverter_Tools");
            cmdTools.DisplayName = "Velocity Converter";
            cmdTools.MenuItemName = "Velocity Converter";
            if (File.Exists(iconPath)) { cmdTools.IconFile = iconPath; }
            cmdTools.Invoked += HandleInvoked;
            cmdTools.MenuPopup += HandleMenuPopup;
        }

        public System.Collections.ICollection GetCustomCommands()
        {
            // Return both commands so they appear in both menus
            return new CustomCommand[] { cmdView, cmdTools };
        }

        private void HandleMenuPopup(object sender, EventArgs e)
        {
            CustomCommand clickedCommand = sender as CustomCommand;
            if (clickedCommand != null)
            {
                clickedCommand.Checked = myVegas.FindDockView("VelocityConverterDock");
            }
        }

        private void HandleInvoked(object sender, EventArgs e)
        {
            if (!myVegas.ActivateDockView("VelocityConverterDock"))
            {
                VelocityDockControl dockControl = new VelocityDockControl(myVegas);
                dockControl.AutoLoadCommand = sender as CustomCommand;
                dockControl.PersistDockWindowState = true;
                myVegas.LoadDockView(dockControl);
            }
        }
    }

    // DOCKABLE CONTROL
    public class VelocityDockControl : DockableControl
    {
        private Vegas _vegas;
        private VelocityUI uiControl;

        public VelocityDockControl(Vegas vegas) : base("VelocityConverterDock")
        {
            _vegas = vegas;
            DisplayName = "Velocity Converter";
            DefaultFloatingSize = new Size(350, 440);
        }

        public override DockWindowStyle DefaultDockWindowStyle
        {
            get { return DockWindowStyle.Docked; }
        }

        protected override void OnLoad(EventArgs args)
        {
            uiControl = new VelocityUI(_vegas);
            uiControl.Dock = DockStyle.Fill;
            this.Controls.Add(uiControl);
        }
    }

    // PLUGIN PROFILE STRUCTURE
    public class PluginProfile
    {
        public string DisplayName { get; set; }
        public string PluginID { get; set; }
        public string SpeedParameterName { get; set; }
        public double ValueMultiplier { get; set; }

        public PluginProfile(string name, string id, string paramName, double multiplier)
        {
            DisplayName = name; PluginID = id; SpeedParameterName = paramName; ValueMultiplier = multiplier;
        }
    }

    // UI AND LOGIC
    public class VelocityUI : UserControl
    {
        Vegas myVegas;

        CheckBox chkRemoveEnv;
        Label lblStatus;
        Panel pnlBorder;
        PictureBox picIcon;
        Label lblHeader;

        PluginProfile profTwixtor = new PluginProfile("Twixtor", "{Svfx:com.revisionfx.Twixtor}", "Speed", 100.0);
        PluginProfile profBCCML = new PluginProfile("BCC+ ReTimer ML", "{Svfx:com.borisfx.bcc.retimerml}", "Speed", 100.0);
        PluginProfile profBCCFlow = new PluginProfile("BCC Optical Flow", "{Svfx:com.borisfx.bcc.opticalflow}", "Velocity", 100.0);
        PluginProfile profSapphire = new PluginProfile("S_Retime", "{Svfx:com.genarts.sapphire.time.Retime}", "Speed", 1.0);

        public VelocityUI(Vegas vegas)
        {
            myVegas = vegas;
            this.DoubleBuffered = true;

            Color controlDark = Color.FromArgb(60, 60, 60);
            Color textLight = Color.FromArgb(220, 220, 220);
            Color accentColor = Color.FromArgb(255, 0, 0); // Pure Red Accent

            this.ForeColor = textLight;

            // Icon Header
            string iconBase64 = "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";
            picIcon = new PictureBox()
            {
                Left = 15,
                Top = 13,
                Width = 32,
                Height = 32,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            try { using (System.IO.MemoryStream ms = new System.IO.MemoryStream(Convert.FromBase64String(iconBase64))) { picIcon.Image = Image.FromStream(ms); } } catch { }

            lblHeader = new Label()
            {
                Left = 54,
                Top = 13,
                Width = 380,
                Height = 32,
                Text = "Velocity Converter",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };

            chkRemoveEnv = new CheckBox() { Left = 15, Top = 60, Width = 300, Text = "Remove Original Velocity Envelope", Checked = true, BackColor = Color.Transparent, ForeColor = textLight };

            pnlBorder = new Panel() { Top = 90, Width = 270, Height = 220, BackColor = Color.Transparent };
            pnlBorder.Paint += (s, e) => {
                using (Pen wp = new Pen(Color.White, 1f))
                {
                    e.Graphics.DrawRectangle(wp, 0, 0, pnlBorder.Width - 1, pnlBorder.Height - 1);
                }
            };

            // Helper to style buttons uniformly
            Button CreatePluginButton(string pluginName)
            {
                Button btn = new Button()
                {
                    Left = 10,
                    Width = 250,
                    Height = 40,
                    Text = pluginName,
                    BackColor = controlDark,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btn.FlatAppearance.BorderColor = accentColor;
                btn.Cursor = Cursors.Hand;

                btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(80, 80, 80);
                btn.MouseLeave += (s, e) => btn.BackColor = controlDark;

                return btn;
            }

            Button btnTwixtor = CreatePluginButton(profTwixtor.DisplayName);
            Button btnBCCML = CreatePluginButton(profBCCML.DisplayName);
            Button btnBCCFlow = CreatePluginButton(profBCCFlow.DisplayName);
            Button btnSapphire = CreatePluginButton(profSapphire.DisplayName);

            btnTwixtor.Click += (s, e) => ProcessVelocityConversion(profTwixtor);
            btnBCCML.Click += (s, e) => ProcessVelocityConversion(profBCCML);
            btnBCCFlow.Click += (s, e) => ProcessVelocityConversion(profBCCFlow);
            btnSapphire.Click += (s, e) => ProcessVelocityConversion(profSapphire);

            pnlBorder.Controls.AddRange(new Control[] { btnTwixtor, btnBCCML, btnBCCFlow, btnSapphire });
            Button[] pluginButtons = new Button[] { btnTwixtor, btnBCCML, btnBCCFlow, btnSapphire };

            lblStatus = new Label() { Left = 15, Top = 325, Width = 300, Height = 40, Text = "Ready to convert.", ForeColor = Color.DarkGray, BackColor = Color.Transparent };

            // Dynamic UI elements and text wrapping for buttons
            this.SizeChanged += (sender, e) => {
                int cw = this.ClientSize.Width;
                int margin = 15;
                int maxUIWidth = 320;

                // max width of UI at 320px
                int uiWidth = Math.Max(80, Math.Min(maxUIWidth, cw - margin * 2));

                // left-offset to keep the constrained UI block centered
                int centerLeft = Math.Max(margin, (cw - uiWidth) / 2);

                lblStatus.Width = uiWidth;
                lblStatus.Left = centerLeft;

                chkRemoveEnv.Width = uiWidth;
                chkRemoveEnv.Left = centerLeft;

                // icon/header spacing while keeping them centered together
                picIcon.Left = centerLeft;
                lblHeader.Width = uiWidth - 39;
                lblHeader.Left = centerLeft + 39;

                // Update Panel bounds
                pnlBorder.Left = centerLeft;
                pnlBorder.Width = uiWidth;

                int currentTop = 10;

                // Dynamically resize and stack buttons inside window
                foreach (Button btn in pluginButtons)
                {
                    btn.Left = 10;
                    int safeBtnWidth = pnlBorder.Width - 20;
                    btn.Width = safeBtnWidth > 0 ? safeBtnWidth : 10;

                    // Measure text to adjust height for wrapping if scaled down
                    Size textSize = TextRenderer.MeasureText(btn.Text, btn.Font, new Size(btn.Width - 10, int.MaxValue), TextFormatFlags.WordBreak);

                    btn.Height = Math.Max(40, textSize.Height + 15);
                    btn.Top = currentTop;

                    currentTop += btn.Height + 10;
                }

                pnlBorder.Height = currentTop;
                lblStatus.Top = pnlBorder.Bottom + 15;

                pnlBorder.Invalidate();
            };

            this.Controls.AddRange(new Control[] { picIcon, lblHeader, chkRemoveEnv, pnlBorder, lblStatus });
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using (System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                this.ClientRectangle, Color.FromArgb(55, 55, 55), Color.FromArgb(20, 20, 20), 90F))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        private void SetStatus(string msg, bool isError)
        {
            lblStatus.Text = msg;
            lblStatus.ForeColor = isError ? Color.Salmon : Color.LightGreen;
            lblStatus.Refresh();
        }

        // Conversion
        private void ProcessVelocityConversion(PluginProfile activeProfile)
        {
            try
            {
                SetStatus($"Processing {activeProfile.DisplayName}...", false);

                PlugInNode effects = myVegas.VideoFX;
                PlugInNode targetPluginNode = effects.GetChildByUniqueID(activeProfile.PluginID);

                if (targetPluginNode == null)
                {
                    SetStatus($"Error: {activeProfile.DisplayName} plugin not found.", true);
                    return;
                }

                int processedEvents = 0;
                bool selectionFound = false;

                using (UndoBlock undo = new UndoBlock($"Convert Velocity to {activeProfile.DisplayName}"))
                {
                    foreach (Track track in myVegas.Project.Tracks)
                    {
                        if (!track.IsVideo()) continue;

                        foreach (TrackEvent evnt in track.Events)
                        {
                            if (!evnt.Selected) continue;

                            selectionFound = true;
                            VideoEvent vevnt = (VideoEvent)evnt;
                            Envelope vEnv = null;

                            foreach (Envelope env in vevnt.Envelopes)
                            {
                                if (env.Type == EnvelopeType.Velocity)
                                {
                                    vEnv = env;
                                    break;
                                }
                            }

                            if (vEnv != null)
                            {
                                Effect targetEff = null;
                                bool pluginExists = false;

                                // Check to see if OFX is already on event
                                foreach (Effect eff in vevnt.Effects)
                                {
                                    if (eff.IsOFX && eff.PlugIn.UniqueID == activeProfile.PluginID)
                                    {
                                        pluginExists = true;
                                        targetEff = eff;
                                        break;
                                    }
                                }

                                if (!pluginExists)
                                {
                                    targetEff = new Effect(targetPluginNode);
                                    vevnt.Effects.Insert(0, targetEff);

                                    // Look for 'convert' preset, and apply
                                    foreach (EffectPreset prst in targetEff.Presets)
                                    {
                                        if (prst.Name == "Convert")
                                        {
                                            targetEff.Preset = "Convert";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    // If OFX already exists on event, then move to front of FX Chain
                                    if (vevnt.Effects[0] != targetEff)
                                    {
                                        vevnt.Effects.Remove(targetEff);
                                        vevnt.Effects.Insert(0, targetEff);
                                    }
                                }

                                // obtaining speed
                                OFXDoubleParameter speedParam = null;
                                try
                                {
                                    speedParam = (OFXDoubleParameter)targetEff.OFXEffect[activeProfile.SpeedParameterName];
                                    speedParam.IsAnimated = true;
                                }
                                catch
                                {
                                    SetStatus($"Error: Param '{activeProfile.SpeedParameterName}' not found.", true);
                                    return;
                                }

                                // Apply velo envs with correct scaler multiplier for each OFX
                                int i = 0;
                                foreach (EnvelopePoint ep in vEnv.Points)
                                {
                                    speedParam.SetValueAtTime(ep.X, ep.Y * activeProfile.ValueMultiplier);

                                    switch (ep.Curve)
                                    {
                                        case CurveType.Fast: speedParam.Keyframes[i].Interpolation = OFXInterpolationType.Fast; break;
                                        case CurveType.Slow: speedParam.Keyframes[i].Interpolation = OFXInterpolationType.Slow; break;
                                        case CurveType.Linear: speedParam.Keyframes[i].Interpolation = OFXInterpolationType.Linear; break;
                                        case CurveType.Sharp: speedParam.Keyframes[i].Interpolation = OFXInterpolationType.Sharp; break;
                                        case CurveType.Smooth: speedParam.Keyframes[i].Interpolation = OFXInterpolationType.Smooth; break;
                                        case CurveType.None: speedParam.Keyframes[i].Interpolation = OFXInterpolationType.Hold; break;
                                    }
                                    i++;
                                }

                                // remove envs if checked
                                if (chkRemoveEnv.Checked) { vevnt.Envelopes.Remove(vEnv); }
                                processedEvents++;
                            }
                        }
                    }
                }

                if (!selectionFound) { SetStatus("No video events are selected.", true); }
                else if (processedEvents == 0) { SetStatus("No Velocity Envelopes found.", true); }
                else { SetStatus($"Success! Converted {processedEvents} event(s).", false); }
            }
            catch (Exception ex)
            {
                SetStatus("Fatal Error: Check VEGAS logs.", true);
                MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "Script Error");
            }
        }
    }
}