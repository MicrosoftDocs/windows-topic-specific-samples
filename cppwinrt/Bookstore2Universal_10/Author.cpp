﻿/*
    Copyright (c) Microsoft Corporation. All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized
    to use this sample source code.  For the terms of the license, please see the
    license agreement between you and Microsoft.
*/

#include "pch.h"
#include "Author.h"

namespace winrt::Bookstore2Universal_10::implementation
{
    std::map<std::wstring, Author const*> Author::s_authorDictionary;
    std::mutex Author::s_authorDictionary_mutex;

    Author::Author(std::wstring const& name) : m_name{ name }
    {
        std::scoped_lock<std::mutex> scoped_lock{ Author::s_authorDictionary_mutex };
        Author::s_authorDictionary[m_name] = this;
    }

    winrt::hstring Author::Name()
    {
        return winrt::hstring{ m_name };
    }

    Windows::Foundation::Collections::IObservableVector<Windows::Foundation::IInspectable> Author::BookSkus()
    {
        return *this;
    }

    void Author::AddBookSku(Bookstore2Universal_10::BookSku const& bookSku)
    {
        get_container().push_back(bookSku);
    }

    Bookstore2Universal_10::Author Author::GetAuthorByName(std::wstring name)
    {
        std::scoped_lock<std::mutex> scoped_lock{ Author::s_authorDictionary_mutex };
        return static_cast<Bookstore2Universal_10::Author>(*Author::s_authorDictionary.at(name));
    }
}
