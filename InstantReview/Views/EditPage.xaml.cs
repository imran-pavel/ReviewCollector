﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstantReview.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InstantReview.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditPage : ContentPage
	{

		public EditPage (EditPageViewModel viewModel)
		{
			InitializeComponent();
		    BindingContext = viewModel;
		}
	}
}