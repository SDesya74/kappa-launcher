﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using KappaLauncher.Apps;
using KappaLauncher.Misc;
using KappaLauncher.Widgets.AppGroup;

using System.Collections.Generic;

namespace KappaLauncher.Views {
	public static partial class Launcher {
		public static Handler Handler = new Handler();
		public static Context Context { get; private set; }
		public static FrameLayout Parent { get; private set; }
		public static ScrollView Scroll { get; private set; }
		public static LinearLayout Main { get; private set; }

		public static void Init(Context context) {
			Context = context;
			Widgets = new List<WidgetData>();

			Parent = new FrameLayout(Context);

			Scroll = new ScrollView(Context);
			Scroll.OverScrollMode = OverScrollMode.Never;
			Scroll.VerticalScrollBarEnabled = false;
			Parent.AddView(Scroll);

			Main = new LinearLayout(Context);
			Main.Orientation = Orientation.Vertical;
			Scroll.AddView(Main);
		}

		public static void Show(Activity activity) {
			ViewGroup parentGroup = (ViewGroup) Parent.Parent;
			parentGroup?.RemoveView(Parent);
			activity.SetContentView(Parent);
		}

		public static void Load() {
			LoadingScreen screen = new LoadingScreen(Context);
			screen.ProgressBar.LayerCount = 3;
			screen.ProgressBar.StrokeWidth = 8.Dip();
			Parent.AddView(screen);

			AppManager.Load(progress => {
				screen.ProgressBar.SetProgress(0, (float) progress);

				if (progress < 1) return;
				Handler.Post(new Java.Lang.Runnable(() => {
					Widgets.Clear();

					AppGroupData data = new AppGroupData(AppManager.Apps);
					Widgets.Add(data);

					Build();
					Parent.RemoveView(screen);
				}));
			});
		}

		public static void Save() {

		}

		public static void Build() {
			Main.RemoveAllViews();
			Widgets.ForEach(widget => {
				if (widget is AppGroupData) {
					AppGroupWidget view = new AppGroupWidget(Context, (AppGroupData) widget);
					Main.AddView(view);
				}
			});
		}


		public static void Message(string text) => Toast.MakeText(Context, text, ToastLength.Short).Show();
	}
}