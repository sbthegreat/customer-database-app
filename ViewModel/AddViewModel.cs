using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using Books_part2.Model;

namespace Books_part2.ViewModel
{
    public class AddViewModel : ViewModelBase
    {
        /// <summary>
        /// The currently selected state.
        /// </summary>
        private State selectedState;

        /// <summary>
        /// The currently entered last name.
        /// </summary>
        private string enteredName;

        /// <summary>
        /// The currently entered address.
        /// </summary>
        private string enteredAddress;

        /// <summary>
        /// The currently entered city.
        /// </summary>
        private string enteredCity;

        /// <summary>
        /// The currently entered zip code.
        /// </summary>
        private string enteredZipCode;

        /// <summary>
        /// The list of states gotten from the database.
        /// </summary>
        private List<State> states;

        /// <summary>
        /// Initializes a new instance of the AddViewModel class.
        /// </summary>
        public AddViewModel()
        {
            states = (from state in MMABooksEntity.dbcontext.States
                         orderby state.StateName
                         select state).ToList<State>();

            AcceptCommand = new RelayCommand<IClosable>(AcceptMethod);
            CancelCommand = new RelayCommand<IClosable>(CancelMethod);
        }

        /// <summary>
        /// The method that adds a new customer based on the input fields to the database.
        /// </summary>
        /// <param name="window">The window to close if successful.</param>
        private void AcceptMethod(IClosable window)
        {
            if (IsValidData())
            {
                try
                {
                    Customer adding = new Customer();
                    adding.Name = enteredName;
                    adding.Address = enteredAddress;
                    adding.City = enteredCity;
                    adding.State = SelectedState.StateCode;
                    adding.ZipCode = enteredZipCode;

                    MMABooksEntity.dbcontext.Customers.Add(adding);
                    MMABooksEntity.dbcontext.SaveChanges();

                    if (window != null)
                    {
                        window.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().ToString());
                }
            }
            else
            {
                MessageBox.Show("Invalid data.", "Error");
            }
        }

        /// <summary>
        /// The method that closes the add window.
        /// </summary>
        /// <param name="window">The window to close.</param>
        private void CancelMethod(IClosable window)
        {
            if (window != null)
            {
                window.Close();
            }
        }

        /// <summary>
        /// Checks if the data in the forms is valid.
        /// </summary>
        /// <returns>True if the data can be used to make a valid customer, false otherwise.</returns>
        private bool IsValidData()
        {
            return HasData(enteredName) &&
                    HasData(enteredAddress) &&
                    HasData(enteredCity) &&
                    HasData(enteredZipCode) &&
                    IsInt32(enteredZipCode) &&
                    selectedState != null;
        }

        /// <summary>
        /// Checks if a string is empty or not.
        /// </summary>
        /// <param name="check">The string to check.</param>
        /// <returns>True if check is not empty, false otherwise.</returns>
        private bool HasData(string check)
        {
            return check != null && check.Length != 0;
        }

        /// <summary>
        /// Checks if a string can be parsed as a 32-bit integer.
        /// </summary>
        /// <param name="check">The string to check.</param>
        /// <returns>True if check is an 32-bit integer, false otherwise.</returns>
        private bool IsInt32(string check)
        {
            try
            {
                Convert.ToInt32(check);
                return true;
            }
            catch (FormatException)
            {
                MessageBox.Show(check + " must be an integer.", "Invalid Data");
                return false;
            }
        }

        /// <summary>
        /// The command that triggers adding a new customer.
        /// </summary>
        public ICommand AcceptCommand { get; private set; }

        /// <summary>
        /// The command that closes the application.
        /// </summary>
        public ICommand CancelCommand { get; private set; }

        /// <summary>
        /// The currently selected state.
        /// </summary>
        public State SelectedState
        {
            get
            {
                return selectedState;
            }
            set
            {
                selectedState = value;
                RaisePropertyChanged("SelectedState");
            }
        }

        /// <summary>
        /// The list of all states from the database.
        /// </summary>
        public List<State> States
        {
            get
            {
                return states;
            }
            set
            {
                states = value;
                RaisePropertyChanged("States");
            }
        }

        /// <summary>
        /// The currently entered name.
        /// </summary>
        public string EnteredName
        {
            get
            {
                return enteredName;
            }
            set
            {
                enteredName = value;
                RaisePropertyChanged("EnteredName");
            }
        }

        /// <summary>
        /// The currently entered address.
        /// </summary>
        public string EnteredAddress
        {
            get
            {
                return enteredAddress;
            }
            set
            {
                enteredAddress = value;
                RaisePropertyChanged("EnteredAddress");
            }
        }

        /// <summary>
        /// The currently entered city.
        /// </summary>
        public string EnteredCity
        {
            get
            {
                return enteredCity;
            }
            set
            {
                enteredCity = value;
                RaisePropertyChanged("EnteredCity");
            }
        }

        /// <summary>
        /// The currently entered zip code.
        /// </summary>
        public string EnteredZipCode
        {
            get
            {
                return enteredZipCode;
            }
            set
            {
                enteredZipCode = value;
                RaisePropertyChanged("EnteredZipCode");
            }
        }
    }
}