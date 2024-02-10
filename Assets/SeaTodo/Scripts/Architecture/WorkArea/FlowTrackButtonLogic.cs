using Architecture.Data;
using Architecture.Elements;
using HomeTools.Messenger;
using HomeTools.Source.Design;
using HTools;
using Theming;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.WorkArea
{
    // Component for animation track button
    public class FlowTrackButtonLogic
    {
        // Mask component
        private readonly Mask mask;

        // Animation components
        private readonly UIAlphaSync syncAlphaMove;
        private readonly UIAlphaSync syncAlphaTrack;
        private readonly UIAlphaSync syncAlphaDone;
        private readonly RectTransformSync syncPositionTrack;
        // Additional view for today progress by left side of button
        private readonly FlowTrackTodayView flowTrackTodayView;
        
        // Animation params
        private const float processSpeed = 0.1f;
        private float disableMaskTimer;
        private readonly Animation doneAnimation;
        private bool tracked;
        private UIAlphaSync activeTempSync;

        private Flow flowData; // Current task

        // Create and setup
        public FlowTrackButtonLogic(RectTransform rectTransform)
        {
            // Setup mask
            mask = rectTransform.gameObject.AddComponent<Mask>();
            mask.enabled = false;
            mask.showMaskGraphic = true;

            // Setup animation component for default button icon
            var arrow = rectTransform.Find("Default").GetComponent<RectTransform>();
            syncPositionTrack = new RectTransformSync();
            syncPositionTrack.SetRectTransformSync(arrow);
            syncPositionTrack.Speed = processSpeed * 2f;
            syncPositionTrack.SpeedMode = RectTransformSync.Mode.Lerp;
            SyncWithBehaviour.Instance.AddObserver(syncPositionTrack, AppSyncAnchors.WorkAreaYear);

            // Setup animation component for move mode button icon
            var move = rectTransform.Find("Move");
            syncAlphaMove = new UIAlphaSync();
            syncAlphaMove.AddElement(move.GetComponent<SVGImage>());
            syncAlphaMove.Speed = processSpeed * 8;
            syncAlphaMove.SpeedMode = UIAlphaSync.Mode.Lerp;
            syncAlphaMove.PrepareToWork(1);
            syncAlphaMove.SetAlphaByDynamic(0);
            syncAlphaMove.Run();
            SyncWithBehaviour.Instance.AddObserver(syncAlphaMove, AppSyncAnchors.WorkAreaYear);

            // Setup animation component for track mode button icon
            var track = rectTransform.Find("Track");
            syncAlphaTrack = new UIAlphaSync();
            syncAlphaTrack.AddElement(track.GetComponent<SVGImage>());
            syncAlphaTrack.Speed = processSpeed * 8;
            syncAlphaTrack.SpeedMode = UIAlphaSync.Mode.Lerp;
            syncAlphaTrack.PrepareToWork(1);
            syncAlphaTrack.SetAlphaByDynamic(0);
            syncAlphaTrack.Run();
            SyncWithBehaviour.Instance.AddObserver(syncAlphaTrack, AppSyncAnchors.WorkAreaYear);
            
            // Setup animation component for done mode button icon
            var done = rectTransform.Find("Done");
            syncAlphaDone = new UIAlphaSync();
            syncAlphaDone.AddElement(done.GetComponent<SVGImage>());
            syncAlphaDone.Speed = processSpeed * 8;
            syncAlphaDone.SpeedMode = UIAlphaSync.Mode.Lerp;
            SyncWithBehaviour.Instance.AddObserver(syncAlphaDone, AppSyncAnchors.WorkAreaYear);
            doneAnimation = done.GetComponent<Animation>();
            
            // Setup additional component for show info about today progress
            flowTrackTodayView = new FlowTrackTodayView(rectTransform.parent.Find("Tracked today"));
            MainMessenger.AddMember(NormalizeAlphaWhenThemeUpdated, string.Format(AppMessagesConst.ColorizedArea, AppTheming.AppItem.WorkArea));
            MainMessenger.AddMember(NormalizeAlphaWhenThemeUpdated, AppMessagesConst.UpdateWorkAreaFlowsViewTrack);
            
            ActivateOnCreateIcon();
        }

        // Setup default icon state
        private void ActivateOnCreateIcon()
        {
            if (tracked)
            {
                syncPositionTrack.SetDefaultT(0);
                syncPositionTrack.PrepareToWork();
                syncPositionTrack.SetT(1);
                syncPositionTrack.Enable(false);
                
                syncAlphaDone.PrepareToWork(1);
                flowTrackTodayView.AlphaSync.PrepareToWork(1);
                flowTrackTodayView.AlphaSync.Enable(true);
                UpdateTrackedDay();
            }
            else
            {
                syncPositionTrack.SetDefaultT(0);
                syncPositionTrack.PrepareToWork();
                syncPositionTrack.Enable(true);
                
                syncAlphaDone.PrepareToWork(1);
                syncAlphaDone.SetAlphaByDynamic(0);
                syncAlphaDone.Run();
                
                flowTrackTodayView.AlphaSync.PrepareToWork(1);
                flowTrackTodayView.AlphaSync.SetAlphaByDynamic(0);
                flowTrackTodayView.AlphaSync.Run();
                flowTrackTodayView.AlphaSync.Enable(false);
            }
        }

        // Update by new task
        public void UpdateFlowData(Flow flow)
        {
            flowData = flow;
            tracked = flow.HasTrackedDay(AreasLocator.Instance.WorkArea.WorkCalendar.CurrentDay);
            flowTrackTodayView.SetupByFlow(flow);
            flowTrackTodayView.SetTracked(tracked);
        }

        // Setup move mode view
        public void SetMoveMode()
        {
            activeTempSync = syncAlphaMove;
            SetCustomMode();
        }
        
        // Setup track mode view
        public void SetTrackMode()
        {
            activeTempSync = syncAlphaTrack;
            SetCustomMode();
        }
        
        // Setup custom mode view
        private void SetCustomMode()
        {
            if (tracked)
            {
                syncAlphaDone.Speed = processSpeed * 2f;
                syncAlphaDone.SetAlphaByDynamic(0);
                syncAlphaDone.Run();
                
                flowTrackTodayView.AlphaSync.Enable(true);
                flowTrackTodayView.AlphaSync.Speed = processSpeed * 1.5f;
                flowTrackTodayView.AlphaSync.SetAlphaByDynamic(0);
                flowTrackTodayView.AlphaSync.Run();
            }
            else
            {
                syncPositionTrack.Enable(true);
                syncPositionTrack.TargetPosition = new Vector2(70, 0f);
                syncPositionTrack.SetTByDynamic(0);
                syncPositionTrack.Run();
            }

            activeTempSync.Speed = processSpeed * 0.5f;
            activeTempSync.SetAlphaByDynamic(1);
            activeTempSync.Run();

            mask.enabled = true;
            disableMaskTimer = 1;
        }
        
        // Set default mode without animation
        public void SetDefaultModeImmediately()
        {
            if (tracked)
            {
                syncAlphaDone.SetAlpha(1);
                syncAlphaDone.SetDefaultAlpha(1);
                syncAlphaDone.Stop();

                flowTrackTodayView.AlphaSync.Enable(true);
                flowTrackTodayView.AlphaSync.SetAlpha(1);
                flowTrackTodayView.AlphaSync.SetDefaultAlpha(1);
                flowTrackTodayView.AlphaSync.Stop();
                
                syncPositionTrack.Enable(false);
                syncPositionTrack.TargetPosition = new Vector2(70, 0f);
                syncPositionTrack.SetT(0);
                syncPositionTrack.SetDefaultT(0);
                syncPositionTrack.Stop();
            }
            else
            {
                syncAlphaDone.SetAlpha(0);
                syncAlphaDone.SetDefaultAlpha(0);
                syncAlphaDone.Stop();
                
                flowTrackTodayView.AlphaSync.Enable(false);
                flowTrackTodayView.AlphaSync.SetAlpha(0);
                flowTrackTodayView.AlphaSync.SetDefaultAlpha(0);
                flowTrackTodayView.AlphaSync.Stop();
                
                syncPositionTrack.Enable(true);
                syncPositionTrack.TargetPosition = new Vector2(70, 0f);
                syncPositionTrack.SetT(1);
                syncPositionTrack.SetDefaultT(1);
                syncPositionTrack.Stop();
            }
        }
        
        // Set default mode with animation
        public void SetDefaultMode()
        {
            if (tracked)
            {
                syncAlphaDone.Speed = processSpeed * 1.5f;
                syncAlphaDone.SetAlphaByDynamic(1);
                syncAlphaDone.Run();

                if (activeTempSync != null && activeTempSync == syncAlphaTrack)
                    doneAnimation.Play(doneAnimation.clip.name);
                
                flowTrackTodayView.AlphaSync.Enable(true);
                flowTrackTodayView.AlphaSync.Speed = processSpeed * 1.5f;
                flowTrackTodayView.AlphaSync.SetAlphaByDynamic(1);
                flowTrackTodayView.AlphaSync.Run();
                UpdateTrackedDay();
            }
            else
            {
                syncPositionTrack.Enable(true);
                syncPositionTrack.TargetPosition = new Vector2(-70, 0f);
                syncPositionTrack.SetTByDynamic(1);
                syncPositionTrack.Run();
                
                flowTrackTodayView.AlphaSync.Enable(true);
                flowTrackTodayView.AlphaSync.Speed = processSpeed * 1.5f;
                flowTrackTodayView.AlphaSync.SetAlphaByDynamic(1);
                flowTrackTodayView.AlphaSync.Run();
            }

            if (activeTempSync != null)
            {
                activeTempSync.Speed = processSpeed * 2.5f;
                activeTempSync.SetAlphaByDynamic(0);
                activeTempSync.Run();
            }

            mask.enabled = true;
            disableMaskTimer = 1;
            activeTempSync = null;
        }

        // Update view of today tracked progress
        public void UpdateTrackedDay()
        {
            tracked = flowData.HasTrackedDay(AreasLocator.Instance.WorkArea.WorkCalendar.CurrentDay);
            flowTrackTodayView.SetTracked(tracked);
            flowTrackTodayView.UpdateByDay();
            
            flowTrackTodayView.AlphaSync.Enable(true);
            flowTrackTodayView.AlphaSync.SetAlpha(1);
            flowTrackTodayView.AlphaSync.SetDefaultAlpha(1);
        }

        public void Update()
        {
            MaskEnable();
        }
        
        // Control mask activity
        private void MaskEnable()
        {
            if (disableMaskTimer <= 0 && !mask.enabled)
                return;

            if (disableMaskTimer > 0)
            {
                disableMaskTimer -= processSpeed;
                return;
            }

            mask.enabled = false;
            
            if (activeTempSync != null)
                syncPositionTrack.Enable(false);
        }

        // Normalize alpha chanel of elements when theme of app updated
        private void NormalizeAlphaWhenThemeUpdated()
        {
            UpdateTrackedDay();
            
            syncAlphaTrack.SetAlpha(0);
            syncAlphaTrack.SetDefaultAlpha(0);
            syncAlphaTrack.Stop();
            
            syncAlphaMove.SetAlpha(0);
            syncAlphaMove.SetDefaultAlpha(0);
            syncAlphaMove.Stop();
            
            syncAlphaDone.SetAlpha(tracked ? 1 : 0);
            syncAlphaDone.SetDefaultAlpha(tracked ? 1 : 0);
            syncAlphaDone.Stop();
            
            syncPositionTrack.TargetPosition = new Vector2(70, 0f);
            syncPositionTrack.SetT(tracked ? 0 : 1);
            syncPositionTrack.SetDefaultT(tracked ? 0 : 1);
            syncPositionTrack.Stop();
            syncPositionTrack.Enable(!tracked);
        }
    }
}
