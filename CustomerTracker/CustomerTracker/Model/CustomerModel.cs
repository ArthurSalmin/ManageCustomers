using System;

namespace CustomerTracker.Model
{
    public class CustomerModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int CityId { get; set; }
    }
}
