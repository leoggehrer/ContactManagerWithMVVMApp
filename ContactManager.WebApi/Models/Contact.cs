//@CodeCopy
namespace ContactManager.WebApi.Models
{
    /// <summary>
    /// Represents an contact in the company manager.
    /// </summary>
    public class Contact : ModelObject, Common.Contracts.IContact
    {
        /// <summary>
        /// Gets or sets the first name of the contact.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name of the contact.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the company name of the contact.
        /// </summary>
        public string Company { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email of the contact.
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the phone number of the contact.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the address of the contact.
        /// </summary>
        public string Address { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the note of the contact.
        /// </summary>
        public string Note { get; set; } = string.Empty;
    }
}
