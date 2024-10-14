using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing;
using System.Runtime;
using System.Xml;
using ScriptPortal.Vegas;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Test_Form
{
    public partial class Form1 : Form
    {
        public Vegas myVegas;
        public Form1(Vegas vegas)
        {
            myVegas = vegas;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int i = 0;
            bool RemoveVeloEnvs = false;
            bool NoVeloEnv = false;
            bool SelectedVevent = false;
            PlugInNode effects = myVegas.VideoFX;
            PlugInNode myEff = effects.GetChildByUniqueID("{Svfx:com.borisfx.bcc_dft.ofx.mlRetimer}");
            Effect AddEff = new Effect(myEff);


            foreach (Track myTrack in myVegas.Project.Tracks)
            {
                foreach (TrackEvent evnt in myTrack.Events)
                {
                    bool HasOFXEffect = false;
                    if (evnt.Selected && evnt.IsVideo())
                    {
                        SelectedVevent = true;
                        VideoEvent vevnt = (VideoEvent)evnt;
                        Envelope vEnv = null;
                        foreach (Envelope env in vevnt.Envelopes)
                        {
                            if (env.Type == EnvelopeType.Velocity)
                            {
                                vEnv = env;
                            }
                        }

                        if (vEnv != null)
                        {
                            Timecode cursorloc = evnt.Start;
                            Timecode offset = Timecode.FromFrames(0);
                            OFXDoubleParameter Speed = null;

                            foreach (Effect eff in vevnt.Effects)
                            {
                                if (eff.IsOFX)
                                {
                                    OFXEffect ofx = eff.OFXEffect;

                                    if (eff.PlugIn.UniqueID == "{Svfx:com.borisfx.bcc_dft.ofx.mlRetimer}")
                                    {
                                        HasOFXEffect = true;
                                        Speed = (OFXDoubleParameter)ofx["speed"];
                                        Speed.IsAnimated = true;
                                    }
                                }
                            }
                            if (!HasOFXEffect)
                            {
                                PlugInNode fx = myVegas.VideoFX;
                                PlugInNode plugin = fx.GetChildByUniqueID("{Svfx:com.borisfx.bcc_dft.ofx.mlRetimer}");
                                Effect effect = new Effect(plugin);
                                vevnt.Effects.Add(effect);
                                string presetName = "Convert";
                                if (presetName != null) { effect.Preset = presetName; }
                                OFXEffect ofx = effect.OFXEffect;
                                Speed = (OFXDoubleParameter)ofx["speed"];
                                Speed.IsAnimated = true;
                            }

                            i = 0;
                            foreach (EnvelopePoint ep in vEnv.Points)
                            {
                                offset = ep.X;
                                double value = ep.Y;
                                CurveType ct = ep.Curve;

                                Speed.SetValueAtTime(offset, value * 100);
                                if (ct == CurveType.Fast) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Fast; }
                                if (ct == CurveType.Slow) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Slow; }
                                if (ct == CurveType.Linear) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Linear; }
                                if (ct == CurveType.Sharp) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Sharp; }
                                if (ct == CurveType.Smooth) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Smooth; }
                                if (ct == CurveType.None) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Hold; }
                                RemoveVeloEnvs = true;
                                i++;
                            }
                        }
                        if (vEnv == null)
                        {
                            MessageBox.Show("No Velocity Envelope on Selected Event");
                            NoVeloEnv = true;
                        }
                        if (RemoveVeloEnvs)
                        {
                            vevnt.Envelopes.Remove(vEnv);
                        }
                    }
                }
            }
            if (!SelectedVevent && !NoVeloEnv)
            {
                MessageBox.Show("No Video Event Selected");
            }
            this.Close();
            return;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int i = 0;
            bool RemoveVeloEnvs = false;
            bool NoVeloEnv = false;
            bool SelectedVevent = false;
            PlugInNode effects = myVegas.VideoFX;
            PlugInNode myEff = effects.GetChildByUniqueID("{Svfx:com.borisfx:BCC3Optical_Flow}");
            Effect AddEff = new Effect(myEff);


            foreach (Track myTrack in myVegas.Project.Tracks)
            {
                foreach (TrackEvent evnt in myTrack.Events)
                {
                    bool HasOFXEffect = false;
                    if (evnt.Selected && evnt.IsVideo())
                    {
                        SelectedVevent = true;
                        VideoEvent vevnt = (VideoEvent)evnt;
                        Envelope vEnv = null;
                        foreach (Envelope env in vevnt.Envelopes)
                        {
                            if (env.Type == EnvelopeType.Velocity)
                            {
                                vEnv = env;
                            }
                        }

                        if (vEnv != null)
                        {
                            Timecode cursorloc = evnt.Start;
                            Timecode offset = Timecode.FromFrames(0);
                            OFXDoubleParameter Speed = null;

                            foreach (Effect eff in vevnt.Effects)
                            {
                                if (eff.IsOFX)
                                {
                                    OFXEffect ofx = eff.OFXEffect;

                                    if (eff.PlugIn.UniqueID == "{Svfx:com.borisfx:BCC3Optical_Flow}")
                                    {
                                        HasOFXEffect = true;
                                        Speed = (OFXDoubleParameter)ofx["ParamID.811"];
                                        Speed.IsAnimated = true;
                                    }
                                }
                            }
                            if (!HasOFXEffect)
                            {
                                PlugInNode fx = myVegas.VideoFX;
                                PlugInNode plugin = fx.GetChildByUniqueID("{Svfx:com.borisfx:BCC3Optical_Flow}");
                                Effect effect = new Effect(plugin);
                                vevnt.Effects.Add(effect);
                                string presetName = "Convert";
                                if (presetName != null) { effect.Preset = presetName; }
                                OFXEffect ofx = effect.OFXEffect;
                                Speed = (OFXDoubleParameter)ofx["ParamID.811"];
                                Speed.IsAnimated = true;
                            }

                            i = 0;
                            foreach (EnvelopePoint ep in vEnv.Points)
                            {
                                offset = ep.X;
                                double value = ep.Y;
                                CurveType ct = ep.Curve;

                                Speed.SetValueAtTime(offset, value * 100);
                                if (ct == CurveType.Fast) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Fast; }
                                if (ct == CurveType.Slow) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Slow; }
                                if (ct == CurveType.Linear) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Linear; }
                                if (ct == CurveType.Sharp) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Sharp; }
                                if (ct == CurveType.Smooth) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Smooth; }
                                if (ct == CurveType.None) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Hold; }
                                RemoveVeloEnvs = true;
                                i++;
                            }
                        }
                        if (vEnv == null)
                        {
                            MessageBox.Show("No Velocity Envelope on Selected Event");
                            NoVeloEnv = true;
                        }
                        if (RemoveVeloEnvs)
                        {
                            vevnt.Envelopes.Remove(vEnv);
                        }
                    }
                }
            }
            if (!SelectedVevent && !NoVeloEnv)
            {
                MessageBox.Show("No Video Event Selected");
            }
            this.Close();
            return;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int i = 0;
            bool RemoveVeloEnvs = false;
            bool NoVeloEnv = false;
            bool SelectedVevent = false;
            PlugInNode effects = myVegas.VideoFX;
            PlugInNode myEff = effects.GetChildByUniqueID("{Svfx:com.genarts.sapphire.Time.S_Retime}");
            Effect AddEff = new Effect(myEff);


            foreach (Track myTrack in myVegas.Project.Tracks)
            {
                foreach (TrackEvent evnt in myTrack.Events)
                {
                    bool HasOFXEffect = false;
                    if (evnt.Selected && evnt.IsVideo())
                    {
                        SelectedVevent = true;
                        VideoEvent vevnt = (VideoEvent)evnt;
                        Envelope vEnv = null;
                        foreach (Envelope env in vevnt.Envelopes)
                        {
                            if (env.Type == EnvelopeType.Velocity)
                            {
                                vEnv = env;
                            }
                        }

                        if (vEnv != null)
                        {
                            Timecode cursorloc = evnt.Start;
                            Timecode offset = Timecode.FromFrames(0);
                            OFXDoubleParameter Speed = null;

                            foreach (Effect eff in vevnt.Effects)
                            {
                                if (eff.IsOFX)
                                {
                                    OFXEffect ofx = eff.OFXEffect;

                                    if (eff.PlugIn.UniqueID == "{Svfx:com.genarts.sapphire.Time.S_Retime}")
                                    {
                                        HasOFXEffect = true;
                                        Speed = (OFXDoubleParameter)ofx["Speed"];
                                        Speed.IsAnimated = true;
                                    }
                                }
                            }
                            if (!HasOFXEffect)
                            {
                                PlugInNode fx = myVegas.VideoFX;
                                PlugInNode plugin = fx.GetChildByUniqueID("{Svfx:com.genarts.sapphire.Time.S_Retime}");
                                Effect effect = new Effect(plugin);
                                vevnt.Effects.Add(effect);
                                string presetName = "Convert";
                                if (presetName != null) { effect.Preset = presetName; }
                                OFXEffect ofx = effect.OFXEffect;
                                Speed = (OFXDoubleParameter)ofx["Speed"];
                                Speed.IsAnimated = true;
                            }

                            i = 0;
                            foreach (EnvelopePoint ep in vEnv.Points)
                            {
                                offset = ep.X;
                                double value = ep.Y;
                                CurveType ct = ep.Curve;

                                Speed.SetValueAtTime(offset, value);
                                if (ct == CurveType.Fast) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Fast; }
                                if (ct == CurveType.Slow) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Slow; }
                                if (ct == CurveType.Linear) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Linear; }
                                if (ct == CurveType.Sharp) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Sharp; }
                                if (ct == CurveType.Smooth) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Smooth; }
                                if (ct == CurveType.None) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Hold; }
                                RemoveVeloEnvs = true;
                                i++;
                            }
                        }
                        if (vEnv == null)
                        {
                            MessageBox.Show("No Velocity Envelope on Selected Event");
                            NoVeloEnv = true;
                        }
                        if (RemoveVeloEnvs)
                        {
                            vevnt.Envelopes.Remove(vEnv);
                        }
                    }
                }
            }
            if (!SelectedVevent && !NoVeloEnv)
            {
                MessageBox.Show("No Video Event Selected");
            }
            this.Close();
            return;
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
                int i = 0;
                bool RemoveVeloEnvs = false;
                bool NoVeloEnv = false;
                bool SelectedVevent = false;
                PlugInNode effects = myVegas.VideoFX;
                PlugInNode myEff = effects.GetChildByUniqueID("{Svfx:com.revisionfx.Twixtor}");
                Effect AddEff = new Effect(myEff);


                foreach (Track myTrack in myVegas.Project.Tracks)
                {
                    foreach (TrackEvent evnt in myTrack.Events)
                    {
                        bool HasOFXEffect = false;
                        if (evnt.Selected && evnt.IsVideo())
                        {
                            SelectedVevent = true;
                            VideoEvent vevnt = (VideoEvent)evnt;
                            Envelope vEnv = null;
                            foreach (Envelope env in vevnt.Envelopes)
                            {
                                if (env.Type == EnvelopeType.Velocity)
                                {
                                    vEnv = env;
                                }
                            }

                            if (vEnv != null)
                            {
                                Timecode cursorloc = evnt.Start;
                                Timecode offset = Timecode.FromFrames(0);
                                OFXDoubleParameter Speed = null;

                                foreach (Effect eff in vevnt.Effects)
                                {
                                    if (eff.IsOFX)
                                    {
                                        OFXEffect ofx = eff.OFXEffect;

                                        if (eff.PlugIn.UniqueID == "{Svfx:com.revisionfx.Twixtor}")
                                        {
                                            HasOFXEffect = true;
                                            Speed = (OFXDoubleParameter)ofx["Speed"];
                                            Speed.IsAnimated = true;
                                        }
                                    }
                                }
                                if (!HasOFXEffect)
                                {
                                    PlugInNode fx = myVegas.VideoFX;
                                    PlugInNode plugin = fx.GetChildByUniqueID("{Svfx:com.revisionfx.Twixtor}");
                                    Effect effect = new Effect(plugin);
                                    vevnt.Effects.Add(effect);
                                    string presetName = "Convert";
                                    if (presetName != null) { effect.Preset = presetName; }
                                    OFXEffect ofx = effect.OFXEffect;
                                    Speed = (OFXDoubleParameter)ofx["Speed"];
                                    Speed.IsAnimated = true;
                                }

                                i = 0;
                                foreach (EnvelopePoint ep in vEnv.Points)
                                {
                                    offset = ep.X;
                                    double value = ep.Y;
                                    CurveType ct = ep.Curve;

                                    Speed.SetValueAtTime(offset, value * 100);
                                    if (ct == CurveType.Fast) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Fast; }
                                    if (ct == CurveType.Slow) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Slow; }
                                    if (ct == CurveType.Linear) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Linear; }
                                    if (ct == CurveType.Sharp) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Sharp; }
                                    if (ct == CurveType.Smooth) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Smooth; }
                                    if (ct == CurveType.None) { Speed.Keyframes[i].Interpolation = OFXInterpolationType.Hold; }
                                    RemoveVeloEnvs = true;
                                    i++;
                                }
                            }
                            if (vEnv == null)
                            {
                                MessageBox.Show("No Velocity Envelope on Selected Event");
                                NoVeloEnv = true;
                            }
                            if (RemoveVeloEnvs)
                            {
                                vevnt.Envelopes.Remove(vEnv);
                            }
                        }
                    }
                }
                if (!SelectedVevent && !NoVeloEnv)
                {
                    MessageBox.Show("No Video Event Selected");
                }
            this.Close();
            return;
        }
    }
}

public class EntryPoint
{
    private static Test_Form.Form1 form;

    public void FromVegas(Vegas vegas)
    {
        form = new Test_Form.Form1(vegas);
        form.ShowDialog();
    }
}