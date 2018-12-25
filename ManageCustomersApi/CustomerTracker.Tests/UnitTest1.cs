using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CustomerTracker.ViewModel;
using System.Linq;

namespace CustomerTracker.Tests
{
    [TestClass]
    public class LockedCustomerDialogViewModelTest
    {

        [TestMethod]
        public async void LockedRecordChoiseDialog()
        {
            //Arrange
            MainViewModel mvm = new MainViewModel();
            CustomerViewModel cvm = new CustomerViewModel(new Model.CustomerModel
            {
                CityId = 1, DateOfBirth = DateTime.Today, FirstName = "sdfs", Id = 1, Name = "dsfsdf", Street = "sdfs"
            }, mvm.Cities.FirstOrDefault());
            //Act
            //string customerStatus = await mvm.GetStatusCode(cvm.Id);
            //Assert
            // Assert.AreEqual("online", customerStatus);
        }
    }
}
