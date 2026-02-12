using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
//
using ClientLib.STD;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using WINFORMS_VLCClient.Controls;
using WINFORMS_VLCClient.Lib.MediaInformation;
using static ClientLib.STD.StandardDefinitions;
//
using Timer = System.Windows.Forms.Timer;

namespace WINFORMS_VLCClient.Forms
{
    public partial class Viewer : Form
    {
        Landing parent;

        MediaPlayer currentPlayer = null!;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MediaPlayer CurrentPlayer
        {
            get => currentPlayer;
            private set
            {
                currentPlayer = value;

                VVMainView.MediaPlayer = currentPlayer;
                VPTMainTimeline.SetLinkedMediaViewer(currentPlayer);
            }
        }

        Timer pollTimer;

        Control[]? allTheThingsThatNeedToAppearOnHover;
        Control[] AllTheThingsThatNeedToAppearOnHover
        {
            get
            {
                if (allTheThingsThatNeedToAppearOnHover == null)
                    allTheThingsThatNeedToAppearOnHover = [VPTMainTimeline];

                return allTheThingsThatNeedToAppearOnHover ?? [];
            }
        }
        public VideoPlaybackTimeline Timeline
        {
            get => this.VPTMainTimeline;
        }

        public Viewer(Landing _parent)
        {
            InitializeComponent();
            parent = _parent;
            CurrentPlayer = parent.RequestMediaPlayer(VVMainView);

            VVMainView.MouseEnter += ShowTimeline;
            VVMainView.MouseLeave += HideTimeline;

            VPTMainTimeline.OnTimelineChanged += SetTime;
            VPTMainTimeline.PauseButtonClicked += (_, _) => CurrentPlayer.Pause();
            VPTMainTimeline.MuteButtonClicked += (_, _) => CurrentPlayer.ToggleMute();

            pollTimer = new() { Interval = 900 };
            pollTimer.Tick += HideTimeline;
            pollTimer.Tick += DoPoll;
            pollTimer.Start();
        }

        public void PlayMedia(Media media, Timestamp? startingPosition = null)
        {
            DisposePlayer();

            CurrentPlayer = parent.RequestMediaPlayer(VVMainView);
            CurrentPlayer.Media = media;

            if (startingPosition != null)
                CurrentPlayer.Time = startingPosition.ToMS();

            CurrentPlayer.Play();
        }

        void DoPoll(object? sender, EventArgs e)
        {
            if (this.parent == null)
                throw new Exception("Viewer cannot be raised with no parent??");

            if (this.parent is not Landing landingParent)
                throw new Exception("Viewer needs landing page for the API'S!");

            if (!CurrentPlayer.IsPlaying)
                return;

            landingParent.LastWatched = new MediaInformation(
                new Uri(CurrentPlayer!.Media!.Mrl),
                Timestamp.FromMS(currentPlayer.Time)
            );
        }

        void SetTime(object? sender, EventArgs e)
        {
            if (e is not TimelineChangedArgs args)
                return;

            if (!CurrentPlayer.IsPlaying)
                return;

            CurrentPlayer.Time = args.requestedPosition;
        }

        void ShowTimeline(object? sender, EventArgs e)
        {
            foreach (var control in AllTheThingsThatNeedToAppearOnHover)
            {
                control.Enabled = true;
                control.Visible = true;
            }
        }

        void HideTimeline(object? sender, EventArgs e)
        {
            if (isCursorInForm(this))
                return;

            foreach (var control in AllTheThingsThatNeedToAppearOnHover)
            {
                control.Enabled = false;
                control.Visible = false;
            }
        }

        void DisposePlayer()
        {
            Task.Run(() =>
            {
                CurrentPlayer.Stop();
                CurrentPlayer.Dispose();
            });
        }
    }
}
