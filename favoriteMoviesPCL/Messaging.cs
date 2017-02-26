using System;
using Plugin.Messaging;

namespace FavoriteMoviesPCL
{
	public interface IEmailTask
	{
		bool CanSendEmail { get; }
		bool CanSendEmailAttachments { get; }
		bool CanSendEmailBodyAsHtml { get; }
		void SendEmail (IEmailMessage email);
		void SendEmail (string to, string subject, string message);
	}

	public interface ISmsTask
	{
		bool CanSendSms { get; }
		void SendSms (string recipient, string message);
	}

	public interface IPhoneCallTask
	{
		bool CanMakePhoneCall { get; }
		void MakePhoneCall (string number, string name = null);
	}
}
