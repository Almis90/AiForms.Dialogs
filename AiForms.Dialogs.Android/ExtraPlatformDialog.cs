﻿using System;
using AiForms.Dialogs.Abstractions;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;

namespace AiForms.Dialogs
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class ExtraPlatformDialog : Android.App.DialogFragment, IDialogInterfaceOnKeyListener
    {
        DialogView _dialogView;
        ViewGroup _contentView;

        public ExtraPlatformDialog() { }

        // System Required!
        public ExtraPlatformDialog(IntPtr handle, JniHandleOwnership transfer) :base(handle,transfer) { }

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.OnCreateDialog(savedInstanceState);

            var payload = Arguments.GetSerializable("extraDialogPayload") as ExtraDialogPayload;

            _dialogView = payload.DialogView;
            _contentView = payload.ContentView;
             
            var dialog = Dialogs.CreateFullScreenTransparentDialog(_contentView);

            // If the OverlayColor is default or transparent, the top padding of the Dialog is set.
            // Because it avoids the status bar color turning dark.
            if (_dialogView.OverlayColor.IsTransparentOrDefault())
            {
                var height = Dialogs.ContentSize.Height;

                dialog.Window.SetGravity(GravityFlags.CenterHorizontal | GravityFlags.Bottom);
                dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, height);
            }

            dialog.SetOnKeyListener(this);
            
            return dialog;
        }

        public override void OnStart()
        {
            base.OnStart();
            _dialogView.RunPresentationAnimation();
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            
            _contentView = null;
            _dialogView = null;
        }

        public bool OnKey(IDialogInterface dialog, [GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            // Back Button handling
            if (keyCode == Keycode.Back && e.Action == KeyEventActions.Up)
            {
                _dialogView.DialogNotifierInternal.Cancel();
                return true;
            }

            return false;
        }
    }
}
