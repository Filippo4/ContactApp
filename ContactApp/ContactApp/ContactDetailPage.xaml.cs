using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContactApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactDetailPage : ContentPage
    {
        public delegate void ContactSaved(Contact c);
        public ContactSaved OnSave { get; set; } 
        public ContactDetailPage(Contact c)
        {
            BindingContext = c;
            InitializeComponent();
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            var c = BindingContext as Contact;
            if (string.IsNullOrWhiteSpace(c.FirstName) || string.IsNullOrWhiteSpace(c.LastName)) 
            {
                await DisplayAlert("Error", "Please Enter name and surname","Ok");
            }

            else
            {
                OnSave?.Invoke(c);
                await Navigation.PopAsync();
                
            }
        }
    }
}