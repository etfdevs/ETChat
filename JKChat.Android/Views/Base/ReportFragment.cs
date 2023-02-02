﻿using Android.OS;
using Android.Views;

using JKChat.Android.Helpers;
using JKChat.Core.ViewModels.Base;
using JKChat.Core.ViewModels.Base.Items;

using MvvmCross.ViewModels;

namespace JKChat.Android.Views.Base {
	public abstract class ReportFragment<TViewModel, TItem> : BaseFragment<TViewModel> where TItem : class, ISelectableItemVM where TViewModel : ReportViewModel<TItem>, IMvxViewModel, IBaseViewModel {
		private TItem selectedItem;
		public virtual TItem SelectedItem {
			get => selectedItem;
			set { selectedItem = value; CheckSelection(); }
		}
		protected virtual IMenuItem ReportItem { get; set; }

		public ReportFragment(int layoutId, int menuId) : base(layoutId, menuId) {}

		public override void OnViewCreated(View view, Bundle savedInstanceState) {
			base.OnViewCreated(view, savedInstanceState);
			CheckSelection(false);

			var set = this.CreateBindingSet();
			set.Bind(this).For(v => v.SelectedItem).To(vm => vm.SelectedItem);
			set.Apply();
		}

		public override void OnResume() {
			base.OnResume();
			CheckSelection();
		}

		public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater) {
			base.OnCreateOptionsMenu(menu, inflater);
		}

		public override bool OnOptionsItemSelected(IMenuItem item) {
			if (SelectedItem != null) {
				if (item == ReportItem) {
					ViewModel.ReportCommand?.Execute(SelectedItem);
				}
				CloseSelection();
				return true;
			}
			return base.OnOptionsItemSelected(item);
		}

		public override bool OnBackPressed() {
			if (SelectedItem != null) {
				CloseSelection();
				return true;
			}
			return base.OnBackPressed();
		}

		protected override void CreateOptionsMenu() {
			base.CreateOptionsMenu();

			ReportItem = Menu.FindItem(Resource.Id.report_item);
			ReportItem.SetClickAction(() => {
				this.OnOptionsItemSelected(ReportItem);
			});
			CheckSelection();
		}

		protected virtual void CheckSelection(bool animated = true) {
			if (SelectedItem == null) {
				CloseSelection(animated);
			} else {
				ShowSelection(animated);
			}
		}

		protected virtual void ShowSelection(bool animated = true) {
			ReportItem?.SetVisible(true, animated);
			BackArrow?.SetRotation(1.0f, animated);
		}

		protected virtual void CloseSelection(bool animated = true) {
			ViewModel.SelectCommand?.Execute(null);
			ReportItem?.SetVisible(false, animated);
			BackArrow?.SetRotation(0.0f, animated);
		}

		protected override void BackNavigationClick(object sender, AndroidX.AppCompat.Widget.Toolbar.NavigationClickEventArgs ev) {
			if (!OnBackPressed()) {
				base.BackNavigationClick(sender, ev);
			}
		}
	}
}