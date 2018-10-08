/*
    Copyright (c) Microsoft Corporation. All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized
    to use this sample source code.  For the terms of the license, please see the
    license agreement between you and Microsoft.
*/

#pragma once

#include "BookSku.g.h"
#include "Author.h"

namespace winrt::Bookstore2Universal_10::implementation
{
    struct BookSku : BookSkuT<BookSku>
    {
        BookSku() = delete;
        BookSku(std::wstring const& title, std::wstring const& authorName, std::wstring const& coverImagePath);
        BookSku(std::wstring const& title, Bookstore2Universal_10::Author const& author, std::wstring const& coverImagePath);

		Bookstore2Universal_10::Author Author();
		winrt::hstring AuthorName();

		Windows::UI::Xaml::Media::ImageSource CoverImage();
		winrt::hstring CoverImagePath();

		winrt::hstring Title();
        winrt::event_token PropertyChanged(Windows::UI::Xaml::Data::PropertyChangedEventHandler const& value);
        void PropertyChanged(winrt::event_token const& token);

    private:
		Bookstore2Universal_10::Author m_author{ nullptr };
        std::wstring m_coverImagePath;
        std::wstring m_title;
        winrt::event<Windows::UI::Xaml::Data::PropertyChangedEventHandler> m_propertyChanged;
    };
}
