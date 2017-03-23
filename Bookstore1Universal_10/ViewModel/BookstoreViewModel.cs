/* 
    Copyright (c) Microsoft Corporation. All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all code Samples for Windows Store apps and Windows Phone Store apps, visit http://code.msdn.microsoft.com/windowsapps
  
*/

namespace Bookstore1Universal_10
{
	using System;
	using System.Collections.ObjectModel;
	using Windows.ApplicationModel;
	using Windows.UI.Xaml.Media;
	using Windows.UI.Xaml.Media.Imaging;

	public class BookstoreViewModel
	{
		#region fields
		private ObservableCollection<BookSku> bookSkus;
		#endregion fields

		#region properties
		public string AppName
		{
			get
			{
				return "BOOKSTORE1UNIVERSAL_10";
			}
		}

		public ObservableCollection<BookSku> BookSkus { get { return this.bookSkus; } }

		public string PageTitle
		{
			get
			{
				{
					return "classics";
				}
			}
		}
		#endregion properties

		#region constructors
		public BookstoreViewModel()
		{
			// Establish the invariant of owning a collection of BookSku.
			this.bookSkus = new ObservableCollection<BookSku>();

			if (DesignMode.DesignModeEnabled)
			{
				DataSource.LoadSampleBookSkus(ref this.bookSkus);
			}
			else
			{
				DataSource.LoadBookSkusFromCloudService(ref this.bookSkus);
			}
		}
		#endregion constructors
	}

	public class BookSku
	{
		#region properties
		public string Author { get; internal set; }

		public ImageSource CoverImage
		{
			get
			{
				// this.CoverImagePath contains a path of the form "/Assets/CoverImages/one.png".
				return new BitmapImage(new Uri(new Uri("ms-appx://"), this.CoverImagePath));
			}
		}

		public string CoverImagePath { get; internal set; }

		public string Title { get; internal set; }
		#endregion properties
	}

	internal static class DataSource
	{
		#region methods
		public static void LoadSampleBookSkus(ref ObservableCollection<BookSku> bookSkus)
		{
			bookSkus.Add(new BookSku()
			{
				Title = "A Christmas Carol",
				Author = "Charles Dickens",
				CoverImagePath = "/Assets/CoverImages/one.png"
			});
			bookSkus.Add(new BookSku()
			{
				Title = "East of Eden",
				Author = "John Steinbeck",
				CoverImagePath = "/Assets/CoverImages/two.png"
			});
			bookSkus.Add(new BookSku()
			{
				Title = "Emma",
				Author = "Jane Austen",
				CoverImagePath = "/Assets/CoverImages/three.png"
			});
			bookSkus.Add(new BookSku()
			{
				Title = "Gone With The Wind",
				Author = "Margaret Mitchell",
				CoverImagePath = "/Assets/CoverImages/four.png"
			});
			bookSkus.Add(new BookSku()
			{
				Title = "Little Dorrit",
				Author = "Charles Dickens",
				CoverImagePath = "/Assets/CoverImages/five.png"
			});
			bookSkus.Add(new BookSku()
			{
				Title = "Mansfield Park",
				Author = "Jane Austen",
				CoverImagePath = "/Assets/CoverImages/six.png"
			});
			bookSkus.Add(new BookSku()
			{
				Title = "Sense and Sensibility",
				Author = "Jane Austen",
				CoverImagePath = "/Assets/CoverImages/seven.png"
			});
		}

		public static void LoadBookSkusFromCloudService(ref ObservableCollection<BookSku> bookSkus)
		{
			// In this simple app, we'll simulate real-world data access by loading sample data.
			DataSource.LoadSampleBookSkus(ref bookSkus);
		}
		#endregion methods
	}
}
