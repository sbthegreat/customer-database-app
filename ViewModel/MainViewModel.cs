using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Books_part2.View;
using System.Data.Entity.Infrastructure;
using Books_part2.Model;

namespace Books_part2.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// The currently selected customer.
        /// </summary>
        private Customer selectedCustomer;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            AddCommand = new RelayCommand(AddMethod);
            ModifyCommand = new RelayCommand(ModifyMethod);
            DeleteCommand = new RelayCommand(DeleteMethod);
            ExitCommand = new RelayCommand<IClosable>(ExitMethod);
            GetCommand = new RelayCommand<string>(GetMethod);
            Messenger.Default.Register<NotificationMessage>(this, ReloadCustomer);
        }

        /// <summary>
        /// The method that adds a new customer.
        /// </summary>
        private void AddMethod()
        {
            AddWindow add = new AddWindow();
            add.Show();
        }

        /// <summary>
        /// The method opens the modify window.
        /// </summary>
        private void ModifyMethod()
        {
            if (selectedCustomer != null)
            {
                ModifyWindow modify = new ModifyWindow();
                Messenger.Default.Send(selectedCustomer);
                modify.Show();
            }
            else
            {
                MessageBox.Show("No Customer is selected", "Error");
            }
        }

        /// <summary>
        /// The method that deletes the selected user.
        /// </summary>
        private void DeleteMethod()
        {
            if (selectedCustomer != null)
            {
                try
                {
                    MessageBoxResult choice = MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButton.YesNo);
                    if (choice == MessageBoxResult.Yes)
                    {
                        MMABooksEntity.dbcontext.Customers.Remove(selectedCustomer);
                        MMABooksEntity.dbcontext.SaveChanges();

                        SelectedCustomer = null;
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ex.Entries.Single().Reload();
                    if (MMABooksEntity.dbcontext.Entry(selectedCustomer).State == System.Data.EntityState.Detached)
                    {
                        MessageBox.Show("Another user has deleted that customer.", "Concurrency Error");
                    }
                    else
                    {
                        MessageBox.Show("Another user has updated that customer.", "Concurrency Error");
                        SelectedCustomer = (from customer in MMABooksEntity.dbcontext.Customers
                                            where customer.CustomerID == selectedCustomer.CustomerID
                                            select customer).Single();
                    }
                }
            }
            else
            {
                MessageBox.Show("No Customer is selected", "Error");
            }
        }

        /// <summary>
        /// The method that closes the application.
        /// </summary>
        /// <param name="window">The window to close.</param>
        private void ExitMethod(IClosable window)
        {
            if (window != null)
            {
                window.Close();
            }
        }

        /// <summary>
        /// The method that retrieves a customer from the database.
        /// </summary>
        /// <param name="stringID">The ID to search for.</param>
        private void GetMethod(string stringID)
        {
            try
            {
                int CustomerID = Convert.ToInt32(stringID);
                var getSelected = (from customer in MMABooksEntity.dbcontext.Customers
                                   where customer.CustomerID == CustomerID
                                   select customer).Single();

                MMABooksEntity.dbcontext.Entry(getSelected).Reload();
                SelectedCustomer = getSelected;
                
                if (!MMABooksEntity.dbcontext.Entry(selectedCustomer).Reference("State1").IsLoaded)
                {
                    MMABooksEntity.dbcontext.Entry(selectedCustomer).Reference("State1").Load();
                }
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("No customer found with this ID. Please try again.", "Customer Not Found");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }
        }

        /// <summary>
        /// Reloads the selected customer when it has been modified.
        /// </summary>
        /// <param name="msg">The message to decode.</param>
        private void ReloadCustomer(NotificationMessage msg)
        {
            if (msg.Notification == "Reload")
            {
                SelectedCustomer = (from customer in MMABooksEntity.dbcontext.Customers
                                    where customer.CustomerID == selectedCustomer.CustomerID
                                    select customer).Single();
            }
        }

        /// <summary>
        /// The command that triggers adding a new customer.
        /// </summary>
        public ICommand AddCommand { get; private set; }

        /// <summary>
        /// The command that triggers modifying the selected customer.
        /// </summary>
        public ICommand ModifyCommand { get; private set; }

        /// <summary>
        /// The command that deletes the selected customer.
        /// </summary>
        public ICommand DeleteCommand { get; private set; }

        /// <summary>
        /// The command that closes the application.
        /// </summary>
        public ICommand ExitCommand { get; private set; }

        /// <summary>
        /// The command that retrieves a customer from the database.
        /// </summary>
        public ICommand GetCommand { get; private set; }

        /// <summary>
        /// The currently loaded customer.
        /// </summary>
        public Customer SelectedCustomer
        {
            get
            {
                return selectedCustomer;
            }
            set
            {
                selectedCustomer = value;
                RaisePropertyChanged("SelectedCustomer");
            }
        }

    }
}