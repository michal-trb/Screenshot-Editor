using screenerWpf.ViewModels;
using screenerWpf.Views;

namespace screenerWpf.Factories
{
    public class OptionsWindowFactory : IOptionsWindowFactory
    {
        public OptionsWindow Create()
        {
            return new OptionsWindow(new OptionsViewModel());
        }
    }
}
