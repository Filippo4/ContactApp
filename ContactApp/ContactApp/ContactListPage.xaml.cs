using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using System.IO;
using System.Collections.ObjectModel;

namespace ContactApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactListPage : ContentPage
    {
        private SQLiteAsyncConnection db;
        private ObservableCollection<Contact> contacts;

        public ContactListPage()
        {
            InitializeComponent();

            var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var file = Path.Combine(docPath, "Contacts.db3");
            db = new SQLiteAsyncConnection(file);
            db.CreateTableAsync<Contact>();
            LoadContacts();
        }

        private async void LoadContacts()
        {
            var list = await db.Table<Contact>()
                .OrderBy(c => c.LastName)
                .OrderBy(c => c.FirstName)
                .ToListAsync();
            // Create a collection that notifies events
            contacts = new ObservableCollection<Contact>(list);
            contactListView.ItemsSource = contacts;
        }

        private async void Add_Clicked(object sender, EventArgs e)
        {
            var c = new Contact();
            var page = new ContactDetailPage(c);
           
            page.OnSave += (c2) =>
            {
                db.InsertAsync(c2);
                contacts.Add(c2);
            };
            await Navigation.PushAsync(page);
        }

        private async void ContactListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var c = contactListView.SelectedItem as Contact;
            var page = new ContactDetailPage(c);
            
            page.OnSave += (c2) =>
            {
                db.UpdateAsync(c2);
            };
            await Navigation.PushAsync(page);
        }

        private async void Delete_Clicked(object sender, EventArgs e)
        {
            var c = ((MenuItem)sender).CommandParameter as Contact;
            if (await DisplayAlert(
                title: "Delete contact", 
                message: $"Are you sure to delete {c.FullName}?",
                accept: "OK",
                cancel: "Cancel"))
            {
                contacts.Remove(c);
                await db.DeleteAsync(c);
            }
        }
    }
}