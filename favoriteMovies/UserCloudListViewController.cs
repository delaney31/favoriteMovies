﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Contacts;
using FavoriteMovies;
using FavoriteMoviesPCL;
using Foundation;
using SQLite;
using UIKit;



namespace FavoriteMovies
{
	public class UserCloudListViewController : BaseBasicListViewController
	{
		List<ContactCard> tableItems;
		const string cellIdentifier = "UserCloudCells";
		public override  void ViewDidLoad ()
		{
			base.ViewDidLoad ();
            tableItems = GetUserContactsAsync ();
			tableSource = new UserCloudTableSource (tableItems, this);
			table.Source = tableSource;
			NavigationItem.Title = "Invite Friends";
			Add (table);

		}

		List<ContactCard> GetUserContactsAsync ()
		{

            var tcs = new TaskCompletionSource<bool> ();
			// Define fields to be searched
			var fetchKeys = new NSString [] { CNContactKey.GivenName, CNContactKey.FamilyName, CNContactKey.EmailAddresses, CNContactKey.ImageDataAvailable, CNContactKey.ThumbnailImageData };
			List<ContactCard> result = new List<ContactCard> ();
            CNContainer [] containers = null;
       
			try 
             {
				var store = new CNContactStore ();
				NSError error = null;
			    TimeSpan ts = TimeSpan.FromMilliseconds (1000);
                Task t = Task.Run (async () => 
                {
                    containers = store.GetContainers (null, out error);
                });
                if (t.Wait (ts)) 
                {
                    if (error != null)
                        tcs.SetException (new Exception (error.LocalizedDescription));
                    else 
                    {
						tcs.SetResult (true); // The true is unused
						foreach (var container in containers) 
                        {
                            var fetchPredicate = CNContact.GetPredicateForContactsInContainer (container.Identifier);

                            var containerResults = store.GetUnifiedContacts (fetchPredicate, fetchKeys, out error);
                            foreach (var contact in containerResults.OrderBy (x => x.GivenName)) 
                            {
                                if (contact.EmailAddresses.Count () > 0 && contact.GivenName.Length > 0) 
                                {
                                    var conCard = new ContactCard (UITableViewCellStyle.Default, cellIdentifier);
                                    conCard.nameLabel.Text = contact.GivenName + " " + contact.FamilyName;
                                    conCard.email = contact.EmailAddresses.FirstOrDefault ()?.Value;
                                    if (contact.ImageDataAvailable)
                                        conCard.profileImage.Image = UIImage.LoadFromData (contact.ThumbnailImageData);
                                    else
                                        conCard.profileImage.Image = UIImage.FromBundle ("1481507483_compose.png"); //default image
                                    result.Add (conCard);

                                 }
                              }

                           }
                        }
                    }
				} catch (Exception ex) {
					Console.WriteLine (ex.Message);
				    tcs.SetException (ex);
				}

            return result.ToList ();

		}
	}


	public class UserCloudTableSource : UITableViewSource
	{
		List<ContactCard> listItems;

		public UserCloudTableSource (List<ContactCard> items, UserCloudListViewController cont)
		{
			this.listItems = items;

		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{

			var cell = listItems [indexPath.Row];
			var switchView = (UISwitch)cell.AccessoryView;
			if (switchView == null) {
				switchView = new UISwitch ();
                switchView.On = ContactAlreadySentEmail (cell);
				switchView.AddTarget (async (sender, e) => {
					if (((UISwitch)sender).On) {
						//tableItems [indexPath.Row].shared = true;
						switchView.On = true;
                        //var emailMessenger = CrossMessaging.Current.EmailMessenger;
                        if (SendEmailToContact (cell))
                            await AddContactToLocalDB (cell);
						//if (emailMessenger.CanSendEmail) 
						//{
						//	// Send simple e-mail to single receiver without attachments, bcc, cc etc.
						//	//emailMessenger.SendEmail ("to.plugins@xamarin.com", "Xamarin Messaging Plugin", "Well hello there from Xam.Messaging.Plugin");
						//	//emailMessenger.SendEmail (cell.email, "Xamarin Messaging Plugin", "Well hello there from Xam.Messaging.Plugin");
						//	// Alternatively use EmailBuilder fluent interface to construct more complex e-mail with multiple recipients, bcc, attachments etc. 
						//	var email = new EmailMessageBuilder ()
						//	  .To ("tldelaney@gmail.com")
						//	  .Cc ("tldelaney@gmail.com")
						//	  //.Bcc (new [] { "bcc1.plugins@xamarin.com", "bcc2.plugins@xamarin.com" })
						//	  .Subject ("Xamarin Messaging Plugin")
						//	  .Body ("Well hello there from Xam.Messaging.Plugin")
						//	  .Build ();

						//	emailMessenger.SendEmail(email);
						//}
					}
					//tableItems [indexPath.Row].shared = false;
					//Owner.UpdateCustomAndMovieList (((CustomList)tableItems [indexPath.Row]).id, false, tableItems);

				}, UIControlEvent.ValueChanged);
			}

			cell.AccessoryView = switchView;
			return cell;
		}

        private bool ContactAlreadySentEmail (ContactCard cell)
        {
			try 
            {
				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) 
                {
                  		// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
						var query = db.Query<Contact> ("SELECT [Email] FROM [Contact] WHERE [Email] = '" + cell.email + "'");
                        if (query.Count > 0)
                            return true;
                        return false;
					};
				

			}
			 catch (Exception e) 
            {
			    Debug.WriteLine (e.Message);
			    using (var conn = new SQLite.SQLiteConnection (MovieService.Database)) 
                {
				
					conn.CreateTable<Contact> ();
                    return false;
				};
					
			}

        }

        private Task AddContactToLocalDB (ContactCard cell)
        {
			try {
				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) 
                {
                    
					var task = Task.Run (() => 
                    {
                        // there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
                        var contact = new Contact ();
                        contact.Email = cell.email;
						db.InsertOrReplace (contact, typeof (Contact));

					});
					task.Wait ();
                    return Task.CompletedTask;


				}
			}  catch (Exception e) 
            {
				Debug.WriteLine (e.Message);
                using (var conn = new SQLite.SQLiteConnection (MovieService.Database)) 
                {
                    var task = Task.Run (() => {
                        conn.CreateTable<Contact> ();
                        var contact = new Contact ();
                        contact.Email = cell.email;
                        conn.InsertOrReplace (contact, typeof (Contact));
                    });
                    task.Wait ();
                    return Task.CompletedTask;
                }
			}
        }

        bool SendEmailToContact (ContactCard cell)
        {
            TimeSpan ts = TimeSpan.FromMilliseconds (1500);
            var task = Task.Run (() => {
                try {
                    MailMessage mail = new MailMessage ();
                    mail.IsBodyHtml = true;
                    SmtpClient SmtpServer = new SmtpClient ("smtp-mail.outlook.com");
                    mail.From = new MailAddress ("tim@moviefriendsapp.com");

                    mail.To.Add (cell.email);
                    mail.Subject = "Movie Friends App Invitation";
                    mail.Body = "<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\">\n <head>\n    " +
                        "\t<!-- NAME: ELEGANT -->\n <!--[if gte mso 15]>\n\t\t<xml>\n\t\t\t<o:OfficeDocumentSettings>\n\t\t\t<o:AllowPNG/>\n\t\t\t<o:PixelsPerInch>96</o:PixelsPerInch>\n\t\t\t</o:OfficeDocumentSettings>\n\t\t</xml>\n\t\t<![endif]-->\n\t\t<meta charset=\"UTF-8\">\n        " +
                        "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\n        <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\n\t\t<title>*|MC:SUBJECT|*</title>\n        \n    " +
                        "<style type=\"text/css\">\n\t\tp{\n\t\t\tmargin:10px 0;\n\t\t\tpadding:0;\n\t\t}\n\t\ttable{\n\t\t\tborder-collapse:collapse;\n\t\t}\n\t\th1,h2,h3,h4,h5,h6{\n\t\t\tdisplay:block;\n\t\t\tmargin:0;" +
                        "\n\t\t\tpadding:0;\n\t\t}\n\t\timg,a img{\n\t\t\tborder:0;\n\t\t\theight:auto;\n\t\t\toutline:none;\n\t\t\ttext-decoration:none;\n\t\t}\n\t\tbody,#bodyTable,#bodyCell{\n\t\t\theight:100%;\n\t\t\tmargin:0;\n\t\t\tpadding:0;\n\t\t\twidth:100%;\n\t\t}\n\t\t#outlook " +
                        "a{\n\t\t\tpadding:0;\n\t\t}\n\t\timg{\n\t\t\t-ms-interpolation-mode:bicubic;\n\t\t}\n\t\ttable{\n\t\t\tmso-table-lspace:0pt;\n\t\t\tmso-table-rspace:0pt;\n\t\t}\n\t\t." +
                        "ReadMsgBody{\n\t\t\twidth:100%;\n\t\t}\n\t\t.ExternalClass{\n\t\t\twidth:100%;\n\t\t}\n\t\tp,a,li,td,blockquote{\n\t\t\tmso-line-height-rule:exactly;\n\t\t}\n\t\ta[href^=tel]" +
                        ",a[href^=sms]{\n\t\t\tcolor:inherit;\n\t\t\tcursor:default;\n\t\t\ttext-decoration:none;\n\t\t}\n\t\tp,a,li,td,body,table,blockquote{\n\t\t\t-ms-text-size-adjust:100%;\n\t\t\t-webkit-text-size-adjust:100%;\n\t\t}\n\t\t.ExternalClass," +
                        ".ExternalClass p,.ExternalClass td,.ExternalClass div,.ExternalClass span,.ExternalClass font{\n\t\t\tline-height:100%;\n\t\t}\n\t\ta[x-apple-data-detectors]{\n\t\t\tcolor:inherit !important;\n\t\t\ttext-decoration:none !important;\n\t\t\tfont-size:inherit " +
                        "!important;\n\t\t\tfont-family:inherit !important;\n\t\t\tfont-weight:inherit !important;\n\t\t\tline-height:inherit !important;\n\t\t}\n\t\ta.mcnButton{\n\t\t\tdisplay:" +
                        "block;\n\t\t}\n\t\t.mcnImage{\n\t\t\tvertical-align:bottom;\n\t\t}\n\t\t.mcnTextContent{\n\t\t\tword-break:break-word;\n\t\t}\n\t\t.mcnTex" +
                        "tContent img{\n\t\t\theight:auto !important;\n\t\t}\n\t\t.mcnDividerBlock{\n\t\t\ttable-layout:fixed !important;\n\t\t}\n\t\tbody,#bodyTable{\n\t\t\tbackground-color:#ECEAD4;\n\t\t}\n\t\t#bodyCell{\n\t\t\tborder-top:0;\n\t\t}\n\t\t#templateContainer{\n\t\t\tborder:0;\n\t\t}\n\t\th1{\n\t\t\tcolor:#C52E26 !important;\n\t\t\tfont-family:Georgia;\n\t\t\tfont-size:30px;\n\t\t\tfont-style:normal;\n\t\t\tfont-weight:normal;\n\t\t\tline-height:125%;\n\t\t\tletter-spacing:0;" +
                        "\n\t\t\ttext-align:center;\n\t\t}\n\t\th2{\n\t\t\tcolor:#202020 !important;\n\t\t\tfont-family:Georgia;\n\t\t\tfont-size:26px;\n\t\t\tfont-style:normal;\n\t\t\tfont-weight:normal;\n\t\t\tline-height:125%;\n\t\t\tletter-spacing:normal;\n\t\t\ttext-align:" +
                        "left;\n\t\t}\n\t\th3{\n\t\t\tcolor:#202020 !important;\n\t\t\tfont-family:Georgia;\n\t\t\tfont-size:14px;\n\t\t\tfont-style:normal;\n\t\t\tfont-weight:" +
                        "normal;\n\t\t\tline-height:125%;\n\t\t\tletter-spacing:normal;\n\t\t\ttext-align:center;\n\t\t}\n\t\th4{\n\t\t\tcolor:#C52E26 !" +
                        "important;\n\t\t\tfont-family:Verdana;\n\t\t\tfont-size:14px;\n\t\t\tfont-style:normal;\n\t\t\tfont-weight:normal;\n\t\t\tline-height:125%;\n\t\t\tletter-spacing:normal;\n\t\t\ttext-align:left;\n\t\t}\n\t\t#templatePreheader{\n\t\t\tbackground-color:#C52E26;\n\t\t\tborder-top:0;\n\t\t\tborder-bottom:0;\n\t\t}\n\t\t.preheaderContainer .mcnTextContent,.preheaderContainer.mcnTextContent " +
                        "p{\n\t\t\tcolor:#FFFFFF;\n\t\t\tfont-family:Verdana;\n\t\t\tfont-size:10px;\n\t\t\tline-height:125%;\n\t\t\ttext-align:left;\n\t\t}\n\t\t.preheaderContainer .mcnTextContent a{\n\t\t\tcolor:#FFFFFF;\n\t\t\tfont-weight:normal;\n\t\t\ttext-decoration:underline;\n\t\t}\n\t\t#templateHeader{\n\t\t\tbackground-color:#FFFFFF;\n\t\t" +
                        "\tborder-top:0;\n\t\t\tborder-bottom:0;\n\t\t}\n\t\t.headerContainer .mcnTextContent,.headerContainer .mcnTextContent p{\n\t\t\tcolor:#404040;\n\t\t\tfont-family:Georgia;\n\t\t\tfont-size:16px;\n\t\t\tline-height:150%;\n\t\t\ttext-align:center;" +
                        "\n\t\t}\n\t\t.headerContainer .mcnTextContent a{\n\t\t\tcolor:#C52E26;\n\t\t\tfont-weight:normal;\n\t\t\ttext-decoration:none;\n\t\t}\n\t\t#templateBody{\n\t\t\tbackground-color:#FFFFFF;\n\t\t\tborder-top:0;\n\t\t\tborder-bottom:0;\n\t\t}\n\t\t." +
                        "bodyContainer .mcnTextContent,.bodyContainer .mcnTextContent p{\n\t\t\tcolor:#404040;\n\t\t\tfont-family:Verdana;\n\t\t\tfont-size:14px;\n\t\t\tline-height:150%;\n\t\t\ttext-align:left;\n\t\t}\n\t\t.bodyContainer .mcnTextContent a{\n\t\t\tcolor:" +
                        "#C52E26;\n\t\t\tfont-weight:normal;\n\t\t\ttext-decoration:none;\n\t\t}\n\t\t#" +
                        "templateColumns{\n\t\t\tbackground-color:#FFFFFF;\n\t\t\tborder-top:0;\n\t\t\tborder-bottom:0;\n\t\t}\n\t\t.leftColumnContainer .mcnTextContent,.leftColumnContainer .mcnTextContent p{\n\t\t\tcolor:#404040;\n\t\t\tfont-family:Verdana;\n\t\t\tfont-size:12px;\n\t\t\tline-height:150%;\n\t\t\ttext-align:left;\n\t\t}\n\t\t.leftColumnContainer .mcnTextContent a{\n\t\t\tcolor:#C52E26;\n\t\t\tfont-weight:normal;\n\t\t\ttext-decoration:none;\n\t\t}\n\t\t." +
                        "rightColumnContainer .mcnTextContent,.rightColumnContainer .mcnTextContent p{\n\t\t\tcolor:#404040;\n\t\t\tfont-family:Verdana;\n\t\t\tfont-size:12px;\n\t\t\tline-height:150%;\n\t\t\ttext-align:left;\n\t\t}\n\t\t.rightColumnContainer .mcnTextContent" +
                        " a{\n\t\t\tcolor:#C52E26;\n\t\t\tfont-weight:normal;\n\t\t\ttext-decoration:none;\n\t\t}\n\t\t#templateFooter{\n\t\t\tbackground-color:#ECEAD4;\n\t\t\tborder-top:0;\n\t\t\tborder-bottom:0;\n\t\t}\n\t\t.footerContainer .mcnTextContent,.footerContainer" +
                        " .mcnTextContent p{\n\t\t\tcolor:#202020;\n\t\t\tfont-family:Verdana;\n\t\t\tfont-size:10px;\n\t\t\tline-height:125%;\n\t\t\ttext-align:center;\n\t\t}\n\t\t.footerContainer .mcnTextContent a{\n\t\t\tcolor:#C52E26;\n\t\t\tfont-weight:bold;\n\t\t" +
                        "\ttext-decoration:none;\n\t\t}\n\t@media only screen and (max-width: 480px){\n\t\tbody,table,td,p,a,li,blockquote{\n\t\t\t-webkit-text-size-adjust:none !important;\n\t\t}\n\n}\t@media only screen and (max-width: 480px){\n\t\tbody{\n\t\t\twidth:100% " +
                        "!important;\n\t\t\tmin-width:100% !important;\n\t\t}\n\n}\t@media only screen and (max-width: 480px){\n\t\t.templateContainer,#templatePreheader,#templateHeader,#templateBody,#templateFooter{\n\t\t\tmax-width:600px !important;\n\t\t\twidth:100% " +
                        "!important;\n\t\t}\n\n}\t@media only screen and (max-width: 480px){\n\t\t.columnsContainer{\n\t\t\tdisplay:block!important;\n\t\t\tmax-width:600px !important;\n\t\t\tpadding-bottom:18px !important;\n\t\t\tpadding-left:0 !important;\n\t\t\twidth:100%!" +
                        "important;\n\t\t}\n\n}\t@media only screen and (max-width: 480px){\n\t\t.mcnImage{\n\t\t\theight:auto !important;\n\t\t\twidth:100% !important;\n\t\t}\n\n}\t@media only screen and (max-width: 480px){\n\t\t.mcnCartContainer,.mcnCaptionTopContent," +
                        ".mcnRecContentContainer,.mcnCaptionBottomContent,.mcnTextContentContainer,.mcnBoxedTextContentContainer,.mcnImageGroupContentContainer,.mcnCaptionLeftTextContentContainer,.mcnCaptionRightTextContentContainer,.mcnCaptionLeftImageContentContainer," +
                        ".mcnCaptionRightImageContentContainer,.mcnImageCardLeftTextContentContainer,.mcnImageCardRightTextContentContainer{\n\t\t\tmax-width:100% !important;\n\t\t\twidth:100% !important;\n\t\t}\n\n}\t@media only screen and (max-width: 480px){\n\t\t." +
                        "mcnBoxedTextContentContainer{\n\t\t\tmin-width:100% !important;\n\t\t}\n\n}\t@media only screen and (max-width: 480px){\n\t\t.mcnImageGroupContent{\n\t\t\tpadding:9px !important;\n\t\t}\n\n}\t@media only screen and (max-width: 480px){\n\t\t." +
                        "mcnCaptionLeftContentOuter .mcnTextContent,.mcnCaptionRightContentOuter .mcnTextContent{\n\t\t\tpadding-top:9px !important;\n\t\t}\n\n}\t@media only screen and (max-width: 480px){\n\t\t.mcnImageCardTopImageContent,.mcnCaptionBlockInner " +
                        ".mcnCaptionTopContent:last-child .mcnTextContent{\n\t\t\tpadding-top:18px !important;\n\t\t}\n\n}\t@media only screen and (max-width: 480px){\n\t\t.mcnImageCardBottomImageContent{\n\t\t\tpadding-bottom:9px !important;\n\t\t}\n\n}\t@media " +
                        "only screen and (max-width: 480px){\n\t\t.mcnImageGroupBlockInner{\n\t\t\tpadding-top:0 !important;\n\t\t\tpadding-bottom:0 !important;\n\t\t}\n\n}\t@media only screen and (max-width: 480px){\n\t\t.mcnImageGroupBlockOuter{\n\t\t\tpadding-" +
                        "top:9px !important;\n\t\t\tpadding-bottom:9px !important;\n\t\t}\n\n}\t@media only screen and (max-width: 480px){\n\t\t.mcnTextContent,.mcnBoxedTextContentColumn{\n\t\t\tpadding-right:18px !important;\n\t\t\tpadding-left:18px !important;\n\t\t}\n\n}" +
                        "\t@media only screen and (max-width: 480px){\n\t\t.mcnImageCardLeftImageContent,.mcnImageCardRightImageContent{\n\t\t\tpadding-right:18px !important;\n\t\t\tpadding-bottom:0 !important;\n\t\t\tpadding-left:18px !important;\n\t\t}\n\n}\t@media only " +
                        "screen and (max-width: 480px){\n\t\t.mcpreview-image-uploader{\n\t\t\tdisplay:none !important;\n\t\t\twidth:100% !important;\n\t\t}\n\n}\t@media only screen and (max-width: 480px){\n\t\th1{\n\t\t\tfont-size:24px !important;\n\t\t\tline-height:125% " +
                        "!important;\n\t\t}\n\n}\t@media only screen and (max-width: 480px){\n\t\th2{\n\t\t\tfont-size:20px " +
                        "!important;\n\t\t\tline-height:125% !important;\n\t\t}\n\n}\t@media only screen and (max-width: 480px){\n\t\th3{\n\t\t\tfont-size:18px !important;\n\t\t\tline-height:125% !important;\n\t\t}\n\n}\t@media only screen and (max-width: 480px){\n\t\th4{\n\t\t\tfont-size:16px !important;\n\t\t\tline-height:125% !important;\n\t\t}\n\n}\t@media only screen and (max-width: 480px){\n\t\t.mcnBoxedTextContentContainer " +
                        ".mcnTextContent,.mcnBoxedTextContentContainer .mcnTextContent p{\n\t\t\tfont-size:18px !important;\n\t\t\tline-height:125% !important;\n\t\t}\n\n}\t@media only screen and (max-width: 480px){\n\t\t#templatePreheader{\n\t\t\tdisplay:block !important;\n\t\t}\n\n}" +
                        "\t@media only screen and (max-width: 480px){\n\t\t.preheaderContainer .mcnTextContent,.preheaderContainer .mcnTextContent" +
                        " p{\n\t\t\tfont-size:14px !important;\n\t\t\tline-height:115% !important;\n\t\t}\n\n}\t@media only screen and (max-width: 480px){\n\t\t.headerContainer .mcnTextContent,.headerContainer .mcnTextContent p{\n\t\t\tfont-size:18px !important;\n\t\t\tline-height:125% !important;\n\t\t}\n\n}\t@media only screen and (max-width: 480px){\n\t\t.bodyContainer .mcnTextContent,.bodyContainer " +
                        ".mcnTextContent p{\n\t\t\tfont-size:18px !important;\n\t\t\tline-height:125% !important;\n\t\t}\n\n}\t@media only screen and (max-width: 480px){\n\t\t.footerContainer .mcnTextContent,.footerContainer .mcnTextContent p{\n\t\t\tfont-size:14px !important;" +
                        "\n\t\t\tline-height:115% !important;\n\t\t}\n\n}</style></head>\n    <body leftmargin=\"0\" marginwidth=\"0\" topmargin=\"0\" marginheight=\"0\" offset=\"0\" style=\"height: 100%;margin: 0;padding: 0;width: 100%;-ms-text-size-adjust: 100%;-webkit-text-" +
                        "size-adjust: 100%;background-color: #ECEAD4;\">\n        <center>\n            <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" height=\"100%\" width=\"100%\" id=\"bodyTable\" style=\"border-collapse: collapse;mso-table-lspace: " +
                        "0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;height: 100%;margin: 0;padding: 0;width: 100%;background-color: #ECEAD4;\">\n                <tr>\n                    <td align=\"center\" valign=\"top\" id=\"bodyCell\"" +
                        " style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;height: 100%;margin: 0;padding: 0;width: 100%;border-top: 0;\">\n                        <!-- BEGIN TEMPLATE // -->\n                        <table border=\"0\" " +
                        "cellpadding=\"0\" cellspacing=\"0\" width=\"600\" class=\"templateContainer\" style=\"border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                            <tr>\n" +
                        "                                <td align=\"center\" valign=\"top\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                                    <!-- BEGIN PREHEADER // -->\n " +
                        "                                   <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\" id=\"templatePreheader\" style=\"border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-" +
                        "webkit-text-size-adjust: 100%;background-color: #C52E26;border-top: 0;border-bottom: 0;\">\n                                        <tr>\n                                        \t<td valign=\"top\" class=\"preheaderContainer\" style=\"padding-top: 9px;" +
                        "padding-bottom: 9px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\"></td>\n                                        </tr>\n                                    </table>\n                                  " +
                        "  <!-- // END PREHEADER -->\n                                </td>\n                            </tr>\n                            <tr>\n                                <td align=\"center\" valign=\"top\" style=\"mso-line-height-rule: " +
                        "exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                                    <!-- BEGIN HEADER // -->\n                                    <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\" " +
                        "id=\"templateHeader\" style=\"border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;background-color: #FFFFFF;border-top: 0;border-bottom: 0;\">\n                                       " +
                        " <tr>\n                                            <td valign=\"top\" class=\"headerContainer\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\"><table border=\"0\" cellpadding=\"0\" " +
                        "cellspacing=\"0\" width=\"100%\" class=\"mcnImageBlock\" style=\"min-width: 100%;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n    " +
                        "<tbody class=\"mcnImageBlockOuter\">\n            <tr>\n                <td valign=\"top\" style=\"padding: 9px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" class=\"mcnImageBlockInner\">\n " +
                        "                   <table align=\"left\" width=\"100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"mcnImageContentContainer\" style=\"min-width: 100%;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: " +
                        "0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                        <tbody><tr>\n                            <td class=\"mcnImageContent\" valign=\"top\" style=\"padding-right: 9px;padding-left: 9px;padding-top: 0;padding-" +
                        "bottom: 0;text-align: center;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                                \n                                    <a href=\"http://www.moviefriendsapp.com\" title=\"\" " +
                        "class=\"\" target=\"_blank\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                                        " +
                        "<img align=\"center\" alt=\"\" src=\"https://gallery.mailchimp.com/43f1ca69a0601858d81c9ce09/images/046814b4-2e83-4f2e-a2ef-c2ce9df68052.png\" width=\"320\" style=\"max-width: 320px;padding-bottom: 0;display: inline !important;vertical-align: bottom;border: " +
                        "0;height: auto;outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;\" class=\"mcnImage\">\n                                    </a>\n                                \n                            </td>\n                       " +
                        " </tr>\n                    </tbody></table>\n                </td>\n            </tr>\n    </tbody>\n</table><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" class=\"mcnDividerBlock\" style=\"min-width: 100%;border-collapse: " +
                        "collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;table-layout: fixed !important;\">\n    <tbody class=\"mcnDividerBlockOuter\">\n        <tr>\n            <td class=\"mcnDividerBlockInner\" " +
                        "style=\"min-width: 100%;padding: 20px 18px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                <table class=\"mcnDividerContent\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" " +
                        "style=\"min-width: 100%;border-top: 1px solid #CCCCCC;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                    <tbody><tr>\n                       " +
                        " <td style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                            <span></span>\n                        </td>\n                    </tr>\n                </tbody></table>\n<!--           " +
                        " \n                <td class=\"mcnDividerBlockInner\" style=\"padding: 18px;\">\n                <hr class=\"mcnDividerContent\" style=\"border-bottom-color:none; border-left-color:none; border-right-color:none; border-bottom-width:0; " +
                        "border-left-width:0; border-right-width:0; margin-top:0; margin-right:0; margin-bottom:0; margin-left:0;\" />\n-->\n            </td>\n        </tr>\n    </tbody>\n</table><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" " +
                        "class=\"mcnTextBlock\" style=\"min-width: 100%;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n    <tbody class=\"mcnTextBlockOuter\">\n        <tr>\n           " +
                        " <td valign=\"top\" class=\"mcnTextBlockInner\" style=\"padding-top: 9px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n              \t<!--[if mso]>\n\t\t\t\t<table align=\"left\" border=\"0\" cellspacing=\"0\" " +
                        "cellpadding=\"0\" width=\"100%\" style=\"width:100%;\">\n\t\t\t\t<tr>\n\t\t\t\t<![endif]-->\n\t\t\t    \n\t\t\t\t<!--[if mso]>\n\t\t\t\t<td valign=\"top\" width=\"600\" style=\"width:600px;\">\n\t\t\t\t<![endif]-->\n                " +
                        "<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"max-width: 100%;min-width: 100%;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" width=\"100%\" " +
                        "class=\"mcnTextContentContainer\">\n                    <tbody><tr>\n                        \n                        <td valign=\"top\" class=\"mcnTextContent\" style=\"padding-top: 0;padding-right: 18px;padding-bottom: 9px;padding-left: 18px;mso-line-height-rule: " +
                        "exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;word-break: break-word;color: #404040;font-family: Georgia;font-size: 16px;line-height: 150%;text-align: center;\">\n                        \n                           " +
                        " <h1 style=\"display: block;margin: 0;padding: 0;font-family: Georgia;font-size: 30px;font-style: normal;font-weight: normal;line-height: 125%;letter-spacing: 0;text-align: center;color: #C52E26 !important;\">Your Invitation to Movie Friends i" +
                        "s enclosed. Activate your invite by signing up at www.moviefriendsapp.com</h1>\n\n</td>\n                    </tr>\n                </tbody></table>\n\t\t\t\t<!--[if mso]>\n\t\t\t\t</td>\n\t\t\t\t<![endif]-->\n                " +
                        "\n\t\t\t\t<!--[if mso]>\n\t\t\t\t</tr>\n\t\t\t\t</table>\n\t\t\t\t<![endif]-->\n            </td>\n        </tr>\n    </tbody>\n</table><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" class=\"mcnDividerBlock\" " +
                        "style=\"min-width: 100%;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;table-layout: fixed !important;\">\n    <tbody class=\"mcnDividerBlockOuter\">\n        " +
                        "<tr>\n            <td class=\"mcnDividerBlockInner\" style=\"min-width: 100%;padding: 20px 18px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                <table class=\"mcnDividerContent\"" +
                        "border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"min-width: 100%;border-top: 1px solid #CCCCCC;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n" +
                        "                    <tbody><tr>\n                        <td style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                            <span></span>\n                        </td>\n " +
                        "                   </tr>\n                </tbody></table>\n<!--            \n                <td class=\"mcnDividerBlockInner\" style=\"padding: 18px;\">\n                <hr class=\"mcnDividerContent\" style=\"border-bottom-color:none; " +
                        "border-left-color:none; border-right-color:none; border-bottom-width:0; border-left-width:0; border-right-width:0; margin-top:0; margin-right:0; margin-bottom:0; margin-left:0;\" />\n-->\n            </td>\n        </tr>\n    </tbody>\n</table></td>\n" +
                        "                                       </tr>\n                                    </table>\n                                    <!-- // END HEADER -->\n                                </td>\n                            </tr>\n                           " +
                        " <tr>\n                                <td align=\"center\" valign=\"top\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                                    <!-- BEGIN BODY // -->\n " +
                        "                                   <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\" id=\"templateBody\" style=\"border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-" +
                        "size-adjust: 100%;background-color: #FFFFFF;border-top: 0;border-bottom: 0;\">\n                                        <tr>\n                                            <td valign=\"top\" class=\"bodyContainer\" style=\"mso-line-height-rule: " +
                        "exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" class=\"mcnTextBlock\" style=\"min-width: 100%;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-" +
                        "rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n    <tbody class=\"mcnTextBlockOuter\">\n        <tr>\n            <td valign=\"top\" class=\"mcnTextBlockInner\" style=\"padding-top: 9px;mso-line-height-rule: exactly;-" +
                        "ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n              \t<!--[if mso]>\n\t\t\t\t<table align=\"left\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\" style=\"width:100%;\">\n\t\t\t\t<tr>\n\t\t\t\t<![endif]-->" +
                        "\n\t\t\t    \n\t\t\t\t<!--[if mso]>\n\t\t\t\t<td valign=\"top\" width=\"600\" style=\"width:600px;\">\n\t\t\t\t<![endif]-->\n                <table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"max-width: 100%;min-width: " +
                        "100%;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\" width=\"100%\" class=\"mcnTextContentContainer\">\n                    <tbody><tr>\n                        " +
                        "\n                        <td valign=\"top\" class=\"mcnTextContent\" style=\"padding-top: 0;padding-right: 18px;padding-bottom: 9px;padding-left: 18px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;" +
                        "word-break: break-word;color: #404040;font-family: Verdana;font-size: 14px;line-height: 150%;text-align: left;\">\n                        \n                            <br>\nYou have been invited by a contact to join the Movie Friends Network. " +
                        "Movie Friends is the world's first social network centered entirely around your love of movies!<br>\n<br>\nWatch the youtube video here.&nbsp;<a href=\"https://lnkd.in/eEuNXd5\" target=\"_blank\" style=\"mso-line-height-rule: exactly;-ms-text-" +
                        "size-adjust: 100%;-webkit-text-size-adjust: 100%;color: #C52E26;font-weight: normal;text-decoration: none;\">https://lnkd.in/eEuNXd5</a><br>\n<br>\n<strong>With Movie Friends you can&nbsp;</strong>\n\n<ul>\n\t<li style=\"mso-line-height-rule: " +
                        "exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">Create lists of your favorite movies</li>\n\t<li style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">Share your list with people " +
                        "you follow</li>\n\t<li style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">See movies of people you follow</li>\n\t<li style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-" +
                        "size-adjust: 100%;\">Watch&nbsp;new movie previews</li>\n\t<li style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">Chat with friends you follow</li>\n\t<li style=\"mso-line-height-rule: exactly;-ms-" +
                        "text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">Get notifications when friends add movies</li>\n\t<li style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">Review movies and see reviews of " +
                        "your friends</li>\n\t<li style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">much more!!</li>\n</ul>\nAs part of early access, you're invited to download the Movie Friends App.<br>\nSign up today" +
                        " www.moviefriendsapp.com<br>\n<br>\nSincerely,<br>\n<br>\nTimothy Delaney<br>\n<em>President, Movie Friends App</em><br>\n&nbsp;\n                        </td>\n                    </tr>\n                </tbody></table>\n\t\t\t\t<!--[if mso]>" +
                        "\n\t\t\t\t</td>\n\t\t\t\t<![endif]-->\n                \n\t\t\t\t<!--[if mso]>\n\t\t\t\t</tr>\n\t\t\t\t</table>\n\t\t\t\t<![endif]-->\n            </td>\n        </tr>\n    </tbody>\n</table><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" " +
                        "width=\"100%\" class=\"mcnDividerBlock\" style=\"min-width: 100%;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;table-layout: fixed !important;\">\n    <tbody class=\"" +
                        "mcnDividerBlockOuter\">\n        <tr>\n            <td class=\"mcnDividerBlockInner\" style=\"min-width: 100%;padding: 36px 18px 9px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                <table " +
                        "class=\"mcnDividerContent\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"min-width: 100%;border-top: 1px solid #CCCCCC;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-" +
                        "text-size-adjust: 100%;\">\n                    <tbody><tr>\n                        <td style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                            <span></span>\n                        " +
                        "</td>\n                    </tr>\n                </tbody></table>\n<!--            \n                <td class=\"mcnDividerBlockInner\" style=\"padding: 18px;\">\n                <hr class=\"mcnDividerContent\" style=\"border-bottom-color:none; " +
                        "border-left-color:none; border-right-color:none; border-bottom-width:0; border-left-width:0; border-right-width:0; margin-top:0; margin-right:0; margin-bottom:0; margin-left:0;\" />\n-->\n            </td>\n        </tr>\n    </tbody>\n</table><table " +
                        "border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" class=\"mcnDividerBlock\" style=\"min-width: 100%;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;table-layout: " +
                        "fixed !important;\">\n    <tbody class=\"mcnDividerBlockOuter\">\n        <tr>\n            <td class=\"mcnDividerBlockInner\" style=\"min-width: 100%;padding: 9px 18px 18px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: " +
                        "100%;\">\n                <table class=\"mcnDividerContent\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"min-width: 100%;border-top: 1px solid #CCCCCC;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: " +
                        "0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                    <tbody><tr>\n                        <td style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n" +
                        "                            <span></span>\n                        </td>\n                    </tr>\n                </tbody></table>\n<!--            \n                <td class=\"mcnDividerBlockInner\" style=\"padding: 18px;\">\n                " +
                        "<hr class=\"mcnDividerContent\" style=\"border-bottom-color:none; border-left-color:none; border-right-color:none; border-bottom-width:0; border-left-width:0; border-right-width:0; margin-top:0; margin-right:0; margin-bottom:0; margin-left:0;\" />\n-->\n" +
                        "            </td>\n        </tr>\n    </tbody>\n</table></td>\n                                        </tr>\n                                    </table>\n                                    <!-- // END BODY -->\n                                " +
                        "</td>\n                            </tr>\n                            <tr>\n                                <td align=\"center\" valign=\"top\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n " +
                        "                                   <!-- BEGIN COLUMNS // -->\n                                    <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" id=\"templateColumns\" style=\"border-collapse: collapse;mso-table-lspace: " +
                        "0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;background-color: #FFFFFF;border-top: 0;border-bottom: 0;\">\n                                        <tr>\n                                            " +
                        "<td align=\"center\" valign=\"top\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                                                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" " +
                        "width=\"600\" class=\"templateContainer\" style=\"border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                                                    <tr>\n " +
                        "                                                       <td align=\"left\" valign=\"top\" class=\"columnsContainer\" width=\"50%\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n " +
                        "                                                           <table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" class=\"templateColumn\" style=\"border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: " +
                        "0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                                                                <tr>\n                                                                    " +
                        "<td valign=\"top\" class=\"leftColumnContainer\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" " +
                        "class=\"mcnDividerBlock\" style=\"min-width: 100%;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;table-layout: fixed !important;\">\n    " +
                        "<tbody class=\"mcnDividerBlockOuter\">\n        <tr>\n            <td class=\"mcnDividerBlockInner\" style=\"min-width: 100%;padding: 18px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: " +
                        "100%;\">\n                <table class=\"mcnDividerContent\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"min-width: 100%;border-top: 0px;border-collapse: collapse;mso-table-lspace: 0pt;mso-" +
                        "table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                    <tbody><tr>\n                        <td style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n" +
                        "                            <span></span>\n                        </td>\n                    </tr>\n                </tbody></table>\n<!--            \n                <td class=\"mcnDividerBlockInner\" style=\"padding: 18px;\">\n" +
                        "                <hr class=\"mcnDividerContent\" style=\"border-bottom-color:none; border-left-color:none; border-right-color:none; border-bottom-width:0; border-left-width:0; border-right-width:0; margin-top:0; margin-right:0; " +
                        "margin-bottom:0; margin-left:0;\" />\n-->\n            </td>\n        </tr>\n    </tbody>\n</table></td>\n                                                                </tr>\n                                                            " +
                        "</table>\n                                                        </td>\n                                                        <td align=\"left\" valign=\"top\" class=\"columnsContainer\" width=\"50%\" style=\"mso-line-height-rule: " +
                        "exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                                                            <table align=\"right\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" class=\"templateColumn\" " +
                        "style=\"border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                                                                " +
                        "<tr>\n                                                                    <td valign=\"top\" class=\"rightColumnContainer\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: " +
                        "100%;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" class=\"mcnDividerBlock\" style=\"min-width: 100%;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: " +
                        "100%;-webkit-text-size-adjust: 100%;table-layout: fixed !important;\">\n    <tbody class=\"mcnDividerBlockOuter\">\n        <tr>\n            <td class=\"mcnDividerBlockInner\" style=\"min-width: 100%;padding: 18px;mso-line-" +
                        "height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                <table class=\"mcnDividerContent\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"min-width: 100%;border-top: " +
                        "0px;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                    <tbody><tr>\n                        <td style=\"mso-line-height-rule: " +
                        "exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n                            <span></span>\n                        </td>\n                    </tr>\n                </tbody></table>\n<!--            " +
                        "\n                <td class=\"mcnDividerBlockInner\" style=\"padding: 18px;\">\n                <hr class=\"mcnDividerContent\" style=\"border-bottom-color:none; border-left-color:none; border-right-color:none; " +
                        "border-bottom-width:0; border-left-width:0; border-right-width:0; margin-top:0; margin-right:0; margin-bottom:0; margin-left:0;\" />\n-->\n            </td>\n        </tr>\n    </tbody>\n</table></td>\n" +
                        "                                                                </tr>\n                                                            </table>\n                                                        </td>\n " +
                        "                                                   </tr>\n                                                </table>\n                                            </td>\n                                        " +
                        "</tr>\n                                    </table>\n                                    <!-- // END COLUMNS -->\n                                </td>\n                            </tr>\n " +
                        "                           <tr>\n                                <td align=\"center\" valign=\"top\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n " +
                        "                                   <!-- BEGIN FOOTER // -->\n                                    <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\" id=\"templateFooter\" style=\"border-collapse: " +
                        "collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;background-color: #ECEAD4;border-top: 0;border-bottom: 0;\">\n                                        " +
                        "<tr>\n                                            <td valign=\"top\" class=\"footerContainer\" style=\"padding-top: 9px;padding-bottom: 9px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: " +
                        "100%;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" class=\"mcnTextBlock\" style=\"min-width: 100%;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: " +
                        "100%;-webkit-text-size-adjust: 100%;\">\n    <tbody class=\"mcnTextBlockOuter\">\n        <tr>\n            <td valign=\"top\" class=\"mcnTextBlockInner\" style=\"padding-top: 9px;mso-line-height-rule: " +
                        "exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;\">\n              \t<!--[if mso]>\n\t\t\t\t<table align=\"left\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\" " +
                        "style=\"width:100%;\">\n\t\t\t\t<tr>\n\t\t\t\t<![endif]-->\n\t\t\t    \n\t\t\t\t<!--[if mso]>\n\t\t\t\t<td valign=\"top\" width=\"600\" style=\"width:600px;\">\n\t\t\t\t<![endif]-->\n                " +
                        "<table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"max-width: 100%;min-width: 100%;border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;-ms-text-size-adjust: " +
                        "100%;-webkit-text-size-adjust: 100%;\" width=\"100%\" class=\"mcnTextContentContainer\">\n                    <tbody><tr>\n                        \n                        " +
                        "<td valign=\"top\" class=\"mcnTextContent\" style=\"padding-top: 0;padding-right: 18px;padding-bottom: 9px;padding-left: 18px;mso-line-height-rule: exactly;-ms-text-size-adjust: " +
                        "100%;-webkit-text-size-adjust: 100%;word-break: break-word;color: #202020;font-family: Verdana;font-size: 10px;line-height: 125%;text-align: center;\">\n                        \n                            " +
                        "<em>Copyright © *2017* &nbsp;JadeSystems Inc, All rights reserved.</em><br>\n<br>\n<br>\n<strong>Our email address is:</strong><br>\ntim@moviefriendsapp.com<br>\n<br>\nWant to change how you " +
                        "receive these emails?<br>\nYou can <a href=\"*|UPDATE_PROFILE|*\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;color: #C52E26;font-weight: " +
                        "bold;text-decoration: none;\">update your preferences</a> or <a href=\"*|UNSUB|*\" style=\"mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;color: #C52E26;" +
                        "font-weight: bold;text-decoration: none;\">unsubscribe from this list</a><br>\n<br>\n&nbsp;\n                        </td>\n                    </tr>\n                </tbody></table>\n\t\t\t\t<!--" +
                        "[if mso]>\n\t\t\t\t</td>\n\t\t\t\t<![endif]-->\n                \n\t\t\t\t<!--[if mso]>\n\t\t\t\t</tr>\n\t\t\t\t</table>\n\t\t\t\t<![endif]-->\n            </td>\n        </tr>\n    </tbody>\n</table></td>\n" +
                        "                                        </tr>\n                                    </table>\n                                    <!-- // END FOOTER -->\n                                </td>\n " +
                        "                           </tr>\n                        </table>\n                        <!-- // END TEMPLATE -->\n                    </td>\n                </tr>\n            </table>\n" +
                        "        </center>\n    </body>\n</html>";
                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new NetworkCredential ("tdelaney@outlook.com", "Hacimdel44");
                    SmtpServer.EnableSsl = true;
                    ServicePointManager.ServerCertificateValidationCallback = delegate (object send, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) {
                        return true;
                    };
                    SmtpServer.Send (mail);
                    return true;
                } catch (Exception ex) {
                    Debug.WriteLine ("Error sending email:" + ex.Message);
                    return false;
                    // Toast.MakeText(Application.Context,ex.ToString(),ToastLength.Long);
                }

            });
            task.Wait (ts);
            return true;
        }

        public override nint RowsInSection (UITableView tableview, nint section)
		{

			return listItems != null ? listItems.Count : 0;
		}
	}
}
