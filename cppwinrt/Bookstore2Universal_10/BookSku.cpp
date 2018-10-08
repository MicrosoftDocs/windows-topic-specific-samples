/*
    Copyright (c) Microsoft Corporation. All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized
    to use this sample source code.  For the terms of the license, please see the
    license agreement between you and Microsoft.
*/

#include "pch.h"
#include "BookSku.h"

namespace winrt::Bookstore2Universal_10::implementation
{
    BookSku::BookSku(std::wstring const& title, std::wstring const& authorName, std::wstring const& coverImagePath) : m_coverImagePath{ coverImagePath }, m_title{ title }
    {
        m_author = Author::GetAuthorByName(authorName);
        if (m_author)
        {
            m_author.AddBookSku(*this);
        }
    }

    BookSku::BookSku(std::wstring const& title, Bookstore2Universal_10::Author const& author, std::wstring const& coverImagePath) : m_author{ author }, m_coverImagePath{ coverImagePath }, m_title{ title }
    {
        winrt::get_self<Bookstore2Universal_10::implementation::Author>(m_author)->get_container().push_back(*this);
    }

    Bookstore2Universal_10::Author BookSku::Author()
    {
        return m_author;
    }

    winrt::hstring BookSku::AuthorName()
    {
        return (m_author) ? m_author.Name() : L"Anonymous";
    }

    Windows::UI::Xaml::Media::ImageSource BookSku::CoverImage()
    {
        // m_coverImagePath contains a path of the form "/Assets/CoverImages/one.png".
        return Windows::UI::Xaml::Media::Imaging::BitmapImage{ Windows::Foundation::Uri{ L"ms-appx://", m_coverImagePath } };
    }

    winrt::hstring BookSku::CoverImagePath()
    {
        return winrt::hstring{ m_coverImagePath };
    }

    winrt::hstring BookSku::Title()
    {
        return winrt::hstring{ m_title };
    }

    winrt::event_token BookSku::PropertyChanged(Windows::UI::Xaml::Data::PropertyChangedEventHandler const& handler)
    {
        return m_propertyChanged.add(handler);
    }

    void BookSku::PropertyChanged(winrt::event_token const& token)
    {
        m_propertyChanged.remove(token);
    }
}
