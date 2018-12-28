using System;

namespace CustomerTracker.Model
{
    /// <summary>
    /// Customer database model
    /// </summary>
    public class CustomerModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? CityId { get; set; }
        public int? IdUser { get; set; }
    }
}
