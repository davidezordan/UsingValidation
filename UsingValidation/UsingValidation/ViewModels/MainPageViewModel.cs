using Prism.Mvvm;
using Prism.Navigation;
using UsingValidation.Models;

namespace UsingValidation.ViewModels
{
    public class MainPageViewModel : BindableBase, INavigationAware
    {
        public MainPageViewModel()
        {
            DemoItem = new Item();
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private Item _item;
        public Item DemoItem
        {
            get { return _item; }
            set { SetProperty(ref _item, value); }
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("title"))
                Title = (string)parameters["title"] + " and Prism";

            DemoItem.Description = string.Empty;
        }
    }
}
