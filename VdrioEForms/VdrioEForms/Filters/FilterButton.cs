using System;
using System.Collections.Generic;
using System.Text;
using VdrioEForms.Edit;
using VdrioEForms.Export;
using Xamarin.Forms;

namespace VdrioEForms.Filters
{
    public class FilterButton : StackLayout
    {
        public Label filtersLabel;
        public Button filterButton;
        public static List<FilterButton> filterButtons;
        public static List<FilterButton> filterButtonsAnalyze;

        public FilterButton()
        {
            Spacing = 0;
            if (filterButtons == null)
                filterButtons = new List<FilterButton>();
            if (filterButtonsAnalyze == null)
            {
                filterButtonsAnalyze = new List<FilterButton>();
            }
            Orientation = StackOrientation.Horizontal;
            Grid grid = new Grid();
            grid.BackgroundColor = Color.Transparent;
            Color mainColor = new Button().BackgroundColor;
            BackgroundColor = Color.Transparent;//new Button().BackgroundColor;
            grid.Margin = 0;
            grid.Padding = 0;
            Margin = 0;
            Padding = 0;
            grid.RowDefinitions.Add(new RowDefinition { Height = 2 });
            grid.RowDefinitions.Add(new RowDefinition { Height = 40 });
            grid.RowDefinitions.Add(new RowDefinition { Height = 2 });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = 110 });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = 2 });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = 40 });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = 2 });
            
            grid.ColumnSpacing = 0;
            grid.RowSpacing = 0;
            BoxView bv = new BoxView { HeightRequest = 2, Margin = 0, BackgroundColor = mainColor };
            Grid.SetRow(bv, 0);
            Grid.SetColumn(bv, 1);
            Grid.SetColumnSpan(bv, 3);
            grid.Children.Add(bv);
            bv = new BoxView { HeightRequest = 2, Margin = 0, BackgroundColor = mainColor };
            Grid.SetRow(bv,2);
            Grid.SetColumn(bv, 1);
            Grid.SetColumnSpan(bv, 3);
            grid.Children.Add(bv);
            bv = new BoxView { WidthRequest = 2, Margin = 0, BackgroundColor = mainColor };
            Grid.SetColumn(bv, 3);
            Grid.SetRow(bv, 0);
            Grid.SetRowSpan(bv, 3);
            grid.Children.Add(bv);
            bv = new BoxView { WidthRequest = 2, Margin = 0, BackgroundColor = mainColor };
            Grid.SetColumn(bv, 1);
            Grid.SetRow(bv, 0);
            Grid.SetRowSpan(bv, 3);
            grid.Children.Add(bv);
            //bv = new BoxView { WidthRequest = 2, Margin = 0 };
            //grid.Children.Add(bv, 2,0);
            //Grid.SetRowSpan(bv, 3);
            filtersLabel = new Label
            {
                Text = "0",
                TextColor = mainColor,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                VerticalTextAlignment = TextAlignment.Center
            };
            if (FiltersPage.currentFilters != null)
            {
                filtersLabel.Text = FiltersPage.currentFilters.filters.Count.ToString();
            }
            Grid.SetRow(filtersLabel, 1);
            Grid.SetColumn(filtersLabel, 2);
            grid.Children.Add(filtersLabel);
            filterButton = new Button { Text = "Filters", HorizontalOptions = LayoutOptions.FillAndExpand, Margin = 0, VerticalOptions= LayoutOptions.FillAndExpand, Padding = 0 };
            filterButton.Clicked += AddFiltersClicked;
            Grid.SetRow(filterButton, 0);
            Grid.SetRowSpan(filterButton, 3);
            Grid.SetColumn(filterButton, 0);
            grid.Children.Add(filterButton);
            //Grid.SetRowSpan(filterButton, 3);
            //Children.Add(filterButton);
            Children.Add(grid);
            filterButtons.Add(this);

        }

        public FilterButton(bool analyze = false)
        {
            if (filterButtons == null)
                filterButtons = new List<FilterButton>();
            if (filterButtonsAnalyze == null)
            {
                filterButtonsAnalyze = new List<FilterButton>();
            }
            Orientation = StackOrientation.Horizontal;
            Grid grid = new Grid();
            grid.BackgroundColor = Color.White;
            BackgroundColor = new Button().BackgroundColor;
            grid.Margin = 0;
            grid.Padding = 0;
            Margin = 0;
            Padding = 0;
            grid.RowDefinitions.Add(new RowDefinition { Height = 2 });
            grid.RowDefinitions.Add(new RowDefinition { Height = 25 });
            grid.RowDefinitions.Add(new RowDefinition { Height = 2 });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = 2 });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = 25 });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = 2 });
            BoxView bv = new BoxView { HeightRequest = 2, Margin = 0 };
            grid.Children.Add(bv, 0, 0);
            Grid.SetColumnSpan(bv, 3);
            bv = new BoxView { HeightRequest = 2, Margin = 0 };
            grid.Children.Add(bv, 0, 2);
            Grid.SetColumnSpan(bv, 3);
            bv = new BoxView { WidthRequest = 2, Margin = 0 };
            grid.Children.Add(bv, 0, 0);
            Grid.SetRowSpan(bv, 3);
            //bv = new BoxView { WidthRequest = 2, Margin = 0 };
            //grid.Children.Add(bv, 2,0);
            //Grid.SetRowSpan(bv, 3);
            filtersLabel = new Label
            {
                Text = "0",
                TextColor = new Button().BackgroundColor,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                VerticalTextAlignment = TextAlignment.Center
            };
            if (FiltersPage.currentFilters != null)
            {
                filtersLabel.Text = FiltersPage.currentFilters.filters.Count.ToString();
            }
            grid.Children.Add(filtersLabel, 1, 1);
            filterButton = new Button { Text = "Filters", HorizontalOptions = LayoutOptions.FillAndExpand, HeightRequest = 29, Margin = 0, Padding = 0 };
            filterButton.Clicked += AddFiltersClicked;
            grid.Children.Add(filterButton, 3, 0);
            Grid.SetRowSpan(filterButton, 3);
            Children.Add(grid);
            Children.Add(filterButton);
            if (analyze)
            {
                filterButtonsAnalyze.Add(this);
            }
            else
            {
                filterButtons.Add(this);
            }
        }


        public void AddFiltersClicked(object sender, EventArgs e)
        {
            if (((EFMasterPage.MainPage.Detail as NavigationPage).RootPage is EditSubmissionsPage))
                        EFMasterPage.MainPage.Detail.Navigation.PushAsync(new FiltersPage((EFMasterPage.MainPage.Detail as NavigationPage).RootPage as EditSubmissionsPage));
            else if (((EFMasterPage.MainPage.Detail as NavigationPage).RootPage is ExportDataPage))
            {
                EFMasterPage.MainPage.Detail.Navigation.PushAsync(new FiltersPage((EFMasterPage.MainPage.Detail as NavigationPage).RootPage as ExportDataPage));
            }

        }


    }
}
