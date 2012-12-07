
namespace Waveface.Stream.WindowsClient
{
	public interface IStreamLoginable
	{
		UserSession LoginWithFacebook();
		UserSession Login(string email, string password);
		void ForgotPassword();
	}
}
