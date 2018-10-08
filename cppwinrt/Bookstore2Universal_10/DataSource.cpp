/*
    Copyright (c) Microsoft Corporation. All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized
    to use this sample source code.  For the terms of the license, please see the
    license agreement between you and Microsoft.
*/

#include "pch.h"
#include "DataSource.h"

namespace winrt::Bookstore2Universal_10::implementation
{
	void DataSource::LoadDataFromCloudService(Windows::Foundation::Collections::IObservableVector<Windows::Foundation::IInspectable>& authors, Windows::Foundation::Collections::IObservableVector<Windows::Foundation::IInspectable>& bookSkus)
	{
		// In this simple app, we'll simulate real-world data-access by loading sample data.
		DataSource::LoadSampleData(authors, bookSkus);
	}

	void DataSource::LoadSampleData(Windows::Foundation::Collections::IObservableVector<Windows::Foundation::IInspectable>& authors, Windows::Foundation::Collections::IObservableVector<Windows::Foundation::IInspectable>& bookSkus)
	{
		authors.Append(winrt::make<Bookstore2Universal_10::implementation::Author>(L"Austen, Jane"));
        authors.Append(winrt::make<Bookstore2Universal_10::implementation::Author>(L"Burnett, Frances Hodgson"));
        authors.Append(winrt::make<Bookstore2Universal_10::implementation::Author>(L"Dickens, Charles"));
        authors.Append(winrt::make<Bookstore2Universal_10::implementation::Author>(L"Mitchell, Margaret"));
        authors.Append(winrt::make<Bookstore2Universal_10::implementation::Author>(L"Steinbeck, John"));
        authors.Append(winrt::make<Bookstore2Universal_10::implementation::Author>(L"Tolkien, J.R.R."));

        bookSkus.Append(winrt::make<Bookstore2Universal_10::implementation::BookSku>(L"Emma", L"Austen, Jane", L"/Assets/CoverImages/three.png"));
		bookSkus.Append(winrt::make<Bookstore2Universal_10::implementation::BookSku>(L"Mansfield Park", L"Austen, Jane", L"/Assets/CoverImages/six.png"));
		bookSkus.Append(winrt::make<Bookstore2Universal_10::implementation::BookSku>(L"Persuasion", L"Austen, Jane", L"/Assets/CoverImages/four.png"));
		bookSkus.Append(winrt::make<Bookstore2Universal_10::implementation::BookSku>(L"Pride and Prejudice", L"Austen, Jane", L"/Assets/CoverImages/five.png"));
		bookSkus.Append(winrt::make<Bookstore2Universal_10::implementation::BookSku>(L"Sense and Sensibility", L"Austen, Jane", L"/Assets/CoverImages/seven.png"));
		bookSkus.Append(winrt::make<Bookstore2Universal_10::implementation::BookSku>(L"The Secret Garden", L"Burnett, Frances Hodgson", L"/Assets/CoverImages/five.png"));
		bookSkus.Append(winrt::make<Bookstore2Universal_10::implementation::BookSku>(L"A Christmas Carol", L"Dickens, Charles", L"/Assets/CoverImages/one.png"));
		bookSkus.Append(winrt::make<Bookstore2Universal_10::implementation::BookSku>(L"David Copperfield", L"Dickens, Charles", L"/Assets/CoverImages/three.png"));
		bookSkus.Append(winrt::make<Bookstore2Universal_10::implementation::BookSku>(L"Great Expectations", L"Dickens, Charles", L"/Assets/CoverImages/five.png"));
		bookSkus.Append(winrt::make<Bookstore2Universal_10::implementation::BookSku>(L"Little Dorrit", L"Dickens, Charles", L"/Assets/CoverImages/five.png"));
		bookSkus.Append(winrt::make<Bookstore2Universal_10::implementation::BookSku>(L"Oliver Twist", L"Dickens, Charles", L"/Assets/CoverImages/two.png"));
		bookSkus.Append(winrt::make<Bookstore2Universal_10::implementation::BookSku>(L"Gone With The Wind", L"Mitchell, Margaret", L"/Assets/CoverImages/four.png"));
		bookSkus.Append(winrt::make<Bookstore2Universal_10::implementation::BookSku>(L"East of Eden", L"Steinbeck, John", L"/Assets/CoverImages/two.png"));
		bookSkus.Append(winrt::make<Bookstore2Universal_10::implementation::BookSku>(L"The Fellowship Of The Ring", L"Tolkien, J.R.R.", L"/Assets/CoverImages/one.png"));
		bookSkus.Append(winrt::make<Bookstore2Universal_10::implementation::BookSku>(L"The Hobbit", L"Tolkien, J.R.R.", L"/Assets/CoverImages/one.png"));
		bookSkus.Append(winrt::make<Bookstore2Universal_10::implementation::BookSku>(L"The Return Of The King", L"Tolkien, J.R.R.", L"/Assets/CoverImages/two.png"));
        bookSkus.Append(winrt::make<Bookstore2Universal_10::implementation::BookSku>(L"The Two Towers", L"Tolkien, J.R.R.", L"/Assets/CoverImages/six.png"));
    }
}
