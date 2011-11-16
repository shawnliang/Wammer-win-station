// -- FILE ------------------------------------------------------------------
// name       : Customer.cs
// created    : Jani Giannoudis - 2008.05.09
// language   : c#
// environment: .NET 3.0
// --------------------------------------------------------------------------

namespace Waveface.Solutions.Community.ConfigurationWindowsDemo
{

	// ------------------------------------------------------------------------
	public class Customer
	{

		// ----------------------------------------------------------------------
		public Customer()
		{
		} // Customer

		// ----------------------------------------------------------------------
		public Customer( string firstName, string lastName, string street, 
			string city, string zipCode )
		{
			this.firstName = firstName;
			this.lastName = lastName;
			this.street = street;
			this.city = city;
			this.zipCode = zipCode;
		} // Customer

		// ----------------------------------------------------------------------
		public string FirstName
		{
			get { return firstName; }
			set { firstName = value; }
		} // FirstName

		// ----------------------------------------------------------------------
		public string LastName
		{
			get { return lastName; }
			set { lastName = value; }
		} // LastName

		// ----------------------------------------------------------------------
		public string Street
		{
			get { return street; }
			set { street = value; }
		} // Street

		// ----------------------------------------------------------------------
		public string City
		{
			get { return city; }
			set { city = value; }
		} // City

		// ----------------------------------------------------------------------
		public string ZipCode
		{
			get { return zipCode; }
			set { zipCode = value; }
		} // ZipCode
		
		// ----------------------------------------------------------------------
		// members
		private string firstName;
		private string lastName;
		private string street;
		private string city;
		private string zipCode;

	} // class Customer

} // namespace Itenso.Solutions.Community.ConfigurationWindowsDemo
// -- EOF -------------------------------------------------------------------
