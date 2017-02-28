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
            DemoItem.Name = string.Empty;
            DemoItem.Description = string.Empty;
        }
    }
}
