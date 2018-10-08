/* 
    Copyright (c) Microsoft Corporation. All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all code Samples for Windows Store apps and Windows Phone Store apps, visit http://code.msdn.microsoft.com/windowsapps
  
*/

namespace Bookstore2Universal_10
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using Windows.ApplicationModel;
	using Windows.UI.Xaml.Media;
	using Windows.UI.Xaml.Media.Imaging;

	public class BookstoreViewModel
	{
		#region fields
		private ObservableCollection<Author> authors;
		private ObservableCollection<BookSku> bookSkus;
		#endregion fields

		#region properties
		public string AppName
		{
			get
			{
				return "BOOKSTORE2UNIVERSAL_10";
			}
		}

		public ObservableCollection<Author> Authors { get { return this.authors; } }

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
			// Establish the invariant of owning a collection of Author and BookSku.
			this.authors = new ObservableCollection<Author>();
			this.bookSkus = new ObservableCollection<BookSku>();

			if (DesignMode.DesignModeEnabled)
			{
				DataSource.LoadSampleData(ref this.authors, ref this.bookSkus);
			}
			else
			{
				DataSource.LoadDataFromCloudService(ref this.authors, ref this.bookSkus);
			}
		}
		#endregion constructors
	}

	public class Author : IEnumerable<BookSku>
	{
		#region fields
		private static Dictionary<string, Author> authorDictionary = new Dictionary<string, Author>();
		private ObservableCollection<BookSku> bookSkus = new ObservableCollection<BookSku>();
		#endregion fields

		#region properties
		public ObservableCollection<BookSku> BookSkus { get { return this.bookSkus; } }
		public string Name { get; set; }
		#endregion properties

		#region constructors
		public Author(string name)
		{
			this.Name = name;
			Author.authorDictionary.Add(this.Name, this);
		}
		#endregion constructors

		#region methods
		internal static Author GetAuthorByName(string name)
		{
			Author author;
			Author.authorDictionary.TryGetValue(name, out author);
			return author;
		}

		public void AddBookSku(BookSku bookSku)
		{
			this.BookSkus.Add(bookSku);
		}
		#endregion methods

		#region IEnumerable<BookSku>
		public IEnumerator<BookSku> GetEnumerator()
		{
			return this.BookSkus.GetEnumerator();
		}
		#endregion IEnumerable<BookSku>

		#region IEnumerable
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.BookSkus.GetEnumerator();
		}
		#endregion IEnumerable
	}

	public class BookSku
	{
		#region properties
		public Author Author { get; private set; }

		public string AuthorName { get { return this.Author != null ? this.Author.Name : "Anonymous"; } }

		public ImageSource CoverImage
		{
			get
			{
				// this.CoverImagePath contains a path of the form "/Assets/CoverImages/one.png".
				return new BitmapImage(new Uri(new Uri("ms-appx://"), this.CoverImagePath));
			}
		}

		public string CoverImagePath { get; private set; }

		public string Title { get; private set; }
		#endregion properties

		#region constructors
		public BookSku(string title, string authorName, string coverImagePath)
		{
			this.Title = title;
			this.Author = Author.GetAuthorByName(authorName);
			if (this.Author != null)
			{
				this.Author.AddBookSku(this);
			}
			this.CoverImagePath = coverImagePath;
		}
		#endregion constructors
	}

	internal static class DataSource
	{
		#region methods
		public static void LoadSampleData(ref ObservableCollection<Author> authors, ref ObservableCollection<BookSku> bookSkus)
		{
			authors.Add(new Author("Austen, Jane"));
			authors.Add(new Author("Burnett, Frances Hodgson"));
			authors.Add(new Author("Dickens, Charles"));
			authors.Add(new Author("Mitchell, Margaret"));
			authors.Add(new Author("Steinbeck, John"));
			authors.Add(new Author("Tolkien, J.R.R."));

			bookSkus.Add(new BookSku("Emma", "Austen, Jane", "/Assets/CoverImages/three.png"));
			bookSkus.Add(new BookSku("Mansfield Park", "Austen, Jane", "/Assets/CoverImages/six.png"));
			bookSkus.Add(new BookSku("Persuasion", "Austen, Jane", "/Assets/CoverImages/four.png"));
			bookSkus.Add(new BookSku("Pride and Prejudice", "Austen, Jane", "/Assets/CoverImages/five.png"));
			bookSkus.Add(new BookSku("Sense and Sensibility", "Austen, Jane", "/Assets/CoverImages/seven.png"));
			bookSkus.Add(new BookSku("The Secret Garden", "Burnett, Frances Hodgson", "/Assets/CoverImages/five.png"));
			bookSkus.Add(new BookSku("A Christmas Carol", "Dickens, Charles", "/Assets/CoverImages/one.png"));
			bookSkus.Add(new BookSku("David Copperfield", "Dickens, Charles", "/Assets/CoverImages/three.png"));
			bookSkus.Add(new BookSku("Great Expectations", "Dickens, Charles", "/Assets/CoverImages/five.png"));
			bookSkus.Add(new BookSku("Little Dorrit", "Dickens, Charles", "/Assets/CoverImages/five.png"));
			bookSkus.Add(new BookSku("Oliver Twist", "Dickens, Charles", "/Assets/CoverImages/two.png"));
			bookSkus.Add(new BookSku("Gone With The Wind", "Mitchell, Margaret", "/Assets/CoverImages/four.png"));
			bookSkus.Add(new BookSku("East of Eden", "Steinbeck, John", "/Assets/CoverImages/two.png"));
			bookSkus.Add(new BookSku("The Fellowship Of The Ring", "Tolkien, J.R.R.", "/Assets/CoverImages/one.png"));
			bookSkus.Add(new BookSku("The Hobbit", "Tolkien, J.R.R.", "/Assets/CoverImages/one.png"));
			bookSkus.Add(new BookSku("The Return Of The King", "Tolkien, J.R.R.", "/Assets/CoverImages/two.png"));
			bookSkus.Add(new BookSku("The Two Towers", "Tolkien, J.R.R.", "/Assets/CoverImages/six.png"));
		}

		public static void LoadDataFromCloudService(ref ObservableCollection<Author> authors, ref ObservableCollection<BookSku> bookSkus)
		{
			// In this simple app, we'll simulate real-world data access by loading sample data.
			DataSource.LoadSampleData(ref authors, ref bookSkus);
		}
		#endregion methods
	}
}