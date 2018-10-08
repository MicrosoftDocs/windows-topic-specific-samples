/*
    Copyright (c) Microsoft Corporation. All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized
    to use this sample source code.  For the terms of the license, please see the
    license agreement between you and Microsoft.
*/

// BookstoreViewModel.cpp
#include "pch.h"
#include "BookstoreViewModel.h"

namespace winrt::Bookstore2Universal_10::implementation
{
    BookstoreViewModel::BookstoreViewModel()
    {
		// Establish the invariant of owning a collection of Author and BookSku.
		m_authors = winrt::single_threaded_observable_vector<Windows::Foundation::IInspectable>();
		m_bookSkus = winrt::single_threaded_observable_vector<Windows::Foundation::IInspectable>();

		if (Windows::ApplicationModel::DesignMode::DesignModeEnabled())
		{
			DataSource::LoadSampleData(m_authors, m_bookSkus);
		}
		else
		{
			DataSource::LoadDataFromCloudService(m_authors, m_bookSkus);
		}
    }

    Windows::Foundation::Collections::IObservableVector<Windows::Foundation::IInspectable> BookstoreViewModel::Authors()
    {
        return m_authors;
    }

	Windows::Foundation::Collections::IObservableVector<Windows::Foundation::IInspectable> BookstoreViewModel::BookSkus()
	{
		return m_bookSkus;
	}

    winrt::hstring BookstoreViewModel::AppName()
    {
        return L"BOOKSTORE2UNIVERSAL_10";
    }

    winrt::hstring BookstoreViewModel::PageTitle()
    {
        return L"classics";
    }

}